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
        
        public CPriceGateway(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, Lazy<ConnectionMultiplexer> redis_Sentinel, IConfiguration configuration, IHubContext<Hub_HSX, IHubClient> hubClient_HSX, IHubContext<Hub_HNX, IHubClient> hubClient_HNX) 
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._redis_Sentinel = redis_Sentinel.Value;
            this._configuration = configuration;
            this._hubClient_HSX = hubClient_HSX;
            this._hubClient_HNX = hubClient_HNX;
        }
        public void StartListeningToRedisChannel()
        {
            try
            {
                this._s6GApp.InfoLogger.LogInfo("StartListeningToRedisChannel");

                var subscriber = _redis.GetSubscriber();
                subscriber.Subscribe("KHANHDZ", (channel, message) =>
                {
                    // Gửi tin nhắn tới tất cả client đã kết nối thông qua SignalR
                    var msg = message.ToString();
                    _hubClient_HSX.Clients.All.ReceiveMessage("KHANHDZ", message);
                    //SendMessageToAllClients(msg);

                });
                subscriber.Subscribe("KHANHDZ2", (channel, message) =>
                {
                    // Gửi tin nhắn tới tất cả client đã kết nối thông qua SignalR
                    var msg = message.ToString();
                    _hubClient_HNX.Clients.All.ReceiveMessage("KHANHDZ2", message);
                });
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
