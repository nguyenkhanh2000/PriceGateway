using Microsoft.AspNetCore.SignalR;
using PriceGateway.Interfaces;
using PriceGateway.Models;
using System.Collections.Concurrent;

namespace PriceGateway.Hubs
{
    /// <summary>
    /// 2025-05-20 09:59:01 khanhnv
    /// Quản lý kết nối và giao tiếp giữa client và server qua signalR
    /// </summary>
    public sealed class ChannelHub : Hub<IHubClient>
    {
        private readonly IClientConnectionStore _clientStore;
        private readonly ConcurrentDictionary<string, HashSet<string>> ChannelClients = new();    // Dictionary lưu trữ các client theo channel
        public ChannelHub(IClientConnectionStore clientStore) 
        {
            this._clientStore = clientStore;
        }
        
        //Connected
        public override Task OnConnectedAsync()
        {
            //lưu thông tin Client connect đến signalR ở đây
            var httpContext = Context.GetHttpContext();
            var connectionId = Context.ConnectionId;
            var counter = new CSignalRCounter
            {
                ConnectionID = connectionId,
                TransportName = httpContext?.Request.Query["transportType"],
                InitHubTime = DateTime.Now.Ticks.ToString(),

                // Lấy từ ServerVariables (giả lập qua Headers nếu bạn không có server classic)
                ServerIP = httpContext?.Connection.LocalIpAddress?.ToString(),
                ClientPublicIP = httpContext?.Connection.RemoteIpAddress?.ToString(),
                HttpUserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                HttpCookie = httpContext?.Request.Headers["Cookie"].ToString(),
            };
            this._clientStore.Add(counter);
            
            return base.OnConnectedAsync(); 
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            this._clientStore.Remove(Context.ConnectionId);
            //lưu thông tin Client disconnect đến signalR ở đây
            return base.OnDisconnectedAsync(exception); 
        }

        // Kết nối client vào một channel
        public async Task SubscribeToChannel(string channelName)
        {
            if (!ChannelClients.ContainsKey(channelName))
            {
                ChannelClients[channelName] = new HashSet<string>();
            }

            ChannelClients[channelName].Add(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);
            //await Clients.Caller.SendAsync("Subscribed", channelName);
            await Clients.Caller.Subscribed(channelName);
        }
        // Hủy đăng ký một client khỏi channel
        public async Task UnsubscribeFromChannel(string channelName)
        {
            if (ChannelClients.ContainsKey(channelName))
            {
                ChannelClients[channelName].Remove(Context.ConnectionId);
                if (ChannelClients[channelName].Count == 0)
                {
                    ChannelClients.TryRemove(channelName, out _);
                }
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
            //await Clients.Caller.SendAsync("Unsubscribed", channelName);
            await Clients.Caller.Unsubscribed(channelName);
        }
        public string sendClientInfoToServer(string strConnectionID, CALR objCALR, CClientInfo objCCI, string strClientPublicIP, string strClientIPv6, string strInitHubTime)
        {
            return strConnectionID;
        }
        // Phương thức này sẽ được gọi khi có dữ liệu mới từ pool
        //public async Task SendMessageToChannel(string channelName, string message)
        //{
        //    if (ChannelClients.ContainsKey(channelName))
        //    {
        //        //await Clients.Group(channelName).SendAsync("ReceiveMessage", message);
        //        await Clients.Group(channelName).ReceiveMessage(channelName, message);
        //    }
        //}
    }
}
