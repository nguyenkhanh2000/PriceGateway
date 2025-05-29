namespace PriceGateway.Interfaces
{
    public interface IHubClient
    {
        Task ReceiveMessage(string user , string message);    
    }
}
