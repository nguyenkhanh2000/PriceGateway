using CommonLib.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PriceGateway.Hubs;
using PriceGateway.Interfaces;
using StackExchange.Redis;

namespace PriceGateway.BLL
{
    public class CPriceGateway : IPriceGateway
    {
        public readonly IS6GApp _s6GApp;
        private readonly ConnectionMultiplexer _redis;
        private readonly ConnectionMultiplexer _redis_Sentinel;
        private IConfiguration _configuration;
        private readonly IHubContext<Hub_HSX, IHubClient> _hubClient_HSX;
        private readonly IHubContext<Hub_HNX, IHubClient> _hubClient_HNX;

        private readonly IHubContext<ChannelHub,IHubClient> _hubChannel;
        
        public CPriceGateway(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, Lazy<ConnectionMultiplexer> redis_Sentinel, IConfiguration configuration, IHubContext<Hub_HSX, IHubClient> hubClient_HSX, IHubContext<Hub_HNX, IHubClient> hubClient_HNX, IHubContext<ChannelHub, IHubClient> hubChannel) 
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._redis_Sentinel = redis_Sentinel.Value;
            this._configuration = configuration;
            this._hubClient_HSX = hubClient_HSX;
            this._hubClient_HNX = hubClient_HNX;
            this._hubChannel = hubChannel;
        }
        public async Task StartListeningToRedisChannel()
        {
            try
            {

                var channelNames = _configuration.GetSection("Redis:RedisChannels").Get<List<string>>();

                this._s6GApp.InfoLogger.LogInfo("StartListeningToRedisChannel");

                var subscriber = _redis.GetSubscriber();

                foreach (var channel in channelNames) 
                {
                    await subscriber.SubscribeAsync(channel, async (channel, message) =>
                    {
                        try
                        {
                            // Gửi tin nhắn tới nhóm tương ứng với channel
                            var msg = message.ToString();
                            //_hubChannel.Clients.Group(channel).SendAsync("ReceiveMessage", msg);

                            await _hubChannel.Clients.Group(channel).ReceiveMessage(channel, msg);
                        }
                        catch (Exception ex) 
                        {
                            this._s6GApp.ErrorLogger.LogError(ex);
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                this._s6GApp.ErrorLogger.LogError(ex);
            }           
        }
    }
}
