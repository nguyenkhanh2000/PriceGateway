using PriceGateway.Interfaces;

namespace PriceGateway.BLL
{
    public class PriceGatewayListenerService : IHostedService
    {
        private readonly IPriceGateway _priceGateway;

        public PriceGatewayListenerService(IPriceGateway priceGateway)
        {
            _priceGateway = priceGateway;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _priceGateway.StartListeningToRedisChannel();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Nếu cần dọn dẹp tài nguyên, xử lý tại đây
            return Task.CompletedTask;
        }
    }
}
