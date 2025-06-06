using Microsoft.AspNetCore.SignalR;
using PriceGateway.Interfaces;
using PriceGateway.Models;

namespace PriceGateway.Hubs
{
    public sealed class GenericHub : Hub<IHubClient>
    {
        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var connectionId = Context.ConnectionId;

            var counter = new CSignalRCounter
            {
                ConnectionID = connectionId,
                InitHubTime = DateTime.Now.Ticks.ToString(),
                ServerIP = httpContext?.Connection.LocalIpAddress?.ToString(),
                ClientPublicIP = httpContext?.Connection.RemoteIpAddress?.ToString(),
                HttpUserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                HttpCookie = httpContext?.Request.Headers["Cookie"].ToString(),
            };

            SignalRConnections.Connections.TryAdd(connectionId, counter);

            Console.WriteLine($"Client connected: {connectionId}");
            Clients.Caller.ReceiveMessage(counter.ConnectionID, counter.ServerIP);

            return base.OnConnectedAsync();
        }
    }

}
