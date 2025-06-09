using Microsoft.AspNetCore.SignalR;
using PriceGateway.Interfaces;

namespace PriceGateway.Hubs
{
    
    public sealed class ChannelHub: Hub<IHubClient>
    {
        
        private static readonly Dictionary<string, HashSet<string>> ChannelClients = new();    // Dictionary lưu trữ các client theo channel
        //Connected
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync(); 
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
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
                    ChannelClients.Remove(channelName);
                }
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);
            //await Clients.Caller.SendAsync("Unsubscribed", channelName);
            await Clients.Caller.Unsubscribed(channelName);
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
