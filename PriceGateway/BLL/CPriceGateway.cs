using CommonLib.Interfaces;
using Microsoft.AspNetCore.SignalR;
using PriceGateway.Hubs;
using PriceGateway.Interfaces;
using StackExchange.Redis;
using SystemCore.Entities;
using SystemCore.Temporaries;

namespace PriceGateway.BLL
{
    /// <summary>
    /// CPriceGateway chịu trách nhiệm quản lý và lắng nghe các kênh Redis - khanhnv
    /// Đăng ký và nhận thông tin từ các kênh Redis đã đc cấu hình và gửi thông báo đến các client SignalR
    /// </summary>
    public class CPriceGateway : IPriceGateway
    {
        public readonly IS6GApp _s6GApp;  //Đối tượng ứng dụng S6G để ghi Log
        private readonly ConnectionMultiplexer _redis;
        private readonly ConnectionMultiplexer _redis_Sentinel;
        private IConfiguration _configuration; //Cấu hình ứng dụng
        private readonly IClientConnectionStore _clientStore;

        private readonly IHubContext<Hub_HSX, IHubClient> _hubClient_HSX;
        private readonly IHubContext<Hub_HNX, IHubClient> _hubClient_HNX;
        private readonly IHubContext<ChannelHub,IHubClient> _hubChannel; // HubContext cho các kênh Redis

        private Timer _clientCountTimer;
        /// <summary>
        /// constructor CPriceGateway - khanhnv
        /// </summary>
        /// <param name="s6GApp"></param>
        /// <param name="redis"></param>
        /// <param name="redis_Sentinel"></param>
        /// <param name="configuration"></param>
        /// <param name="hubClient_HSX"></param>
        /// <param name="hubClient_HNX"></param>
        /// <param name="hubChannel"></param>
        /// <param name="clientStore"></param>
        public CPriceGateway(IS6GApp s6GApp, Lazy<ConnectionMultiplexer> redis, Lazy<ConnectionMultiplexer> redis_Sentinel, IConfiguration configuration, IHubContext<Hub_HSX, IHubClient> hubClient_HSX, IHubContext<Hub_HNX, IHubClient> hubClient_HNX, IHubContext<ChannelHub, IHubClient> hubChannel, IClientConnectionStore clientStore) 
        {
            this._s6GApp = s6GApp;
            this._redis = redis.Value;
            this._redis_Sentinel = redis_Sentinel.Value;
            this._configuration = configuration;
            this._clientStore = clientStore;

            this._hubClient_HSX = hubClient_HSX;
            this._hubClient_HNX = hubClient_HNX;
            this._hubChannel = hubChannel;

            // Khởi tạo Timer để đếm số lượng client mỗi 30 giây
            _clientCountTimer = new Timer(CountClientsAndLog, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }
        /// <summary>
        /// Bắt đầu sub channel Redis - async(subscriber)
        /// </summary>
        /// <returns></returns>
        public async Task StartListeningToRedisChannel()
        {
            try
            {
                var channelNames = _configuration.GetSection(CConfig.__REDIS_CHANNEL).Get<List<string>>();

                this._s6GApp.InfoLogger.LogInfo("StartListeningToRedisChannel");

                var subscriber = _redis.GetSubscriber();
                
                var task = new List<Task>();

                foreach (var channel in channelNames) 
                {
                    task.Add(SubscribeToChannelAsync(subscriber, channel));
                }
            }
            catch(Exception ex)
            {
                this._s6GApp.ErrorLogger.LogError(ex);
            }           
        }
        /// <summary>
        /// Sub vào 1 kênh channel cụ thể và xử lý các msg từ đó
        /// Khi có msg mới, gửi msg đến các client trong group signalR tương ứng với channel.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private async Task SubscribeToChannelAsync(ISubscriber subscriber, string channel)
        {
            try
            {
                TExecutionContext ec = this._s6GApp.DebugLogger.WriteBufferBegin($"{EGlobalConfig.__STRING_BEFORE} {channel}", true);
                // Subscribe to the channel asynchronously
                await subscriber.SubscribeAsync(channel, async (channel, message) =>
                {
                    try
                    {
                        var msg = message.ToString();

                        //có thể biến đổi msg trước khi send cho client ở đây....

                        await _hubChannel.Clients.Group(channel).ReceiveMessage(channel, msg);
                    }
                    catch (Exception ex)
                    {
                        this._s6GApp.ErrorLogger.LogErrorContext(ex, ec);
                    }
                });

                this._s6GApp.InfoLogger.LogInfo($"Subscribed to channel: {channel}");
            }
            catch (Exception ex)
            {
                this._s6GApp.ErrorLogger.LogError(ex);
            }
        }

        private void CountClientsAndLog(object state)
        {
            try
            {
                // Lấy tất cả các client từ ClientConnectionStore
                var clients = _clientStore.GetAll();
                var clientCount = clients.Count;

                // Ghi số lượng client vào log
                _s6GApp.InfoLogger.LogInfo($"Total clients connected: {clientCount}");

                // Ghi thông tin chi tiết về các client vào log
                foreach (var client in clients)
                {
                    var clientInfo = $"ConnectionID: {client.ConnectionID}, " +
                                     $"TransportName: {client.TransportName}, " +
                                     $"ServerIP: {client.ServerIP}, " +
                                     $"ClientPublicIP: {client.ClientPublicIP}, " +
                                     $"HttpUserAgent: {client.HttpUserAgent}";

                    _s6GApp.InfoLogger.LogInfo(clientInfo);
                }
            }
            catch (Exception ex)
            {
                _s6GApp.ErrorLogger.LogError(ex);
            }
        }

    }
}
