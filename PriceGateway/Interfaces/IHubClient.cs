namespace PriceGateway.Interfaces
{
    public interface IHubClient
    {
        Task ReceiveMessage(string user , string message);
        Task Subscribed(string channelName);
        Task Unsubscribed(string channelName);
    }
}
