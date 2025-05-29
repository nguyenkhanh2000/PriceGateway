using Microsoft.AspNetCore.SignalR;
using PriceGateway.Interfaces;
using PriceGateway.Models;

namespace PriceGateway.Hubs
{
    public class Hub_HNX:Hub<IHubClient>
    {
        public override Task OnConnectedAsync()
        {

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

                // Các trường còn lại như ClientPrivateIP, OS, Browser,... bạn cần JS client gửi lên qua `SendMessage` hoặc một hàm khác
            };

            // Lưu vào dictionary
            SignalRConnections.Connections.TryAdd(connectionId, counter);

            Console.WriteLine($"Client connected: {connectionId}");
            SendMessage(counter.ConnectionID, counter.TransportName);

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"Received from {user}: {message}");
            //await Clients.All.SendAsync("ReceiveMessage", user, message);

            await Clients.All.ReceiveMessage(user, message);
        }
        public async Task SendClientInfo(string os, string browser, string mobile, string screen, string page, string clientPrivateIP, string clientIPv6)
        {
            if (SignalRConnections.Connections.TryGetValue(Context.ConnectionId, out var counter))
            {
                counter.OS = os;
                counter.Browser = browser;
                counter.Mobile = mobile;
                counter.Screen = screen;
                counter.Page = page;
                counter.ClientPrivateIP = clientPrivateIP;
                counter.ClientIPv6 = clientIPv6;
            }
            var ss = SignalRConnections.Connections;
            foreach (var kvp in ss)
            {
                var connId = kvp.Key;
                var info = kvp.Value;
                Console.WriteLine($"ConnectionID: {connId}");
                Console.WriteLine($"  OS: {info.OS}");
                Console.WriteLine($"  Browser: {info.Browser}");
                Console.WriteLine($"  Mobile: {info.Mobile}");
                Console.WriteLine($"  Screen: {info.Screen}");
                Console.WriteLine($"  Page: {info.Page}");
                Console.WriteLine($"  PrivateIP: {info.ClientPrivateIP}");
                Console.WriteLine($"  IPv6: {info.ClientIPv6}");
                Console.WriteLine("-----------------------------------");
            }
            await Task.CompletedTask;
        }
    }
}
