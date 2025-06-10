namespace PriceGateway.Interfaces
{
    public interface IPriceGateway
    {
        Task StartListeningToRedisChannel();
    }
}
