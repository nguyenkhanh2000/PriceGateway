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

        private readonly IHubContext<ChannelHub> _hubChannel;
        
        public CPriceGateway(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, Lazy<ConnectionMultiplexer> redis_Sentinel, IConfiguration configuration, IHubContext<Hub_HSX, IHubClient> hubClient_HSX, IHubContext<Hub_HNX, IHubClient> hubClient_HNX, IHubContext<ChannelHub> hubChannel) 
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._redis_Sentinel = redis_Sentinel.Value;
            this._configuration = configuration;
            this._hubClient_HSX = hubClient_HSX;
            this._hubClient_HNX = hubClient_HNX;
            this._hubChannel = hubChannel;
        }
        public void StartListeningToRedisChannel()
        {
            try
            {
                string channel_HSX = _configuration.GetSection(CConfig.__REDIS_CHANNEL_HSX).Value;
                string channel_HNX = _configuration.GetSection(CConfig.__REDIS_CHANNEL_HNX).Value;

                var channelNames = _configuration.GetSection("RedisChannels").Get<List<string>>();

                this._s6GApp.InfoLogger.LogInfo("StartListeningToRedisChannel");

                var subscriber = _redis.GetSubscriber();

                foreach (var channel in channelNames) 
                {
                    subscriber.Subscribe(channel, (channel, message) =>
                    {
                        // Gửi tin nhắn tới nhóm tương ứng với channel
                        var msg = message.ToString();
                        _hubChannel.Clients.Group(channel).SendAsync("ReceiveMessage", msg);
                    });
                }

                //subscriber.Subscribe(channel_HSX, (channel, message) =>
                //{
                //    // Gửi tin nhắn tới tất cả client đã kết nối thông qua SignalR
                //    var msg = message.ToString();
                //    //_hubClient_HSX.Clients.All.ReceiveMessage("KHANHDZ", message);

                //    _hubChannel.Clients.Group(channel_HSX).SendAsync("ReceiveMessage", msg);
                //    //SendMessageToAllClients(msg);

                //});
                //subscriber.Subscribe(channel_HNX, (channel, message) =>
                //{
                //    // Gửi tin nhắn tới tất cả client đã kết nối thông qua SignalR
                //    var msg = message.ToString();
                //    _hubClient_HNX.Clients.All.ReceiveMessage("KHANHDZ2", message);
                //});
            }
            catch(Exception ex)
            {
                this._s6GApp.ErrorLogger.LogError(ex);
            }           
        }
        private async void SendMessageToAllClients(string message)
        {
            // Gửi thông điệp tới tất cả các client
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            await _hubClient_HSX.Clients.All.ReceiveMessage("KhanhDZ", message);
        }
    }
}
