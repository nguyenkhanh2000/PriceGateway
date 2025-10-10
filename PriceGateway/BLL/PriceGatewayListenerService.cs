using PriceGateway.Interfaces;

namespace PriceGateway.BLL
{
    /// <summary>
    /// Dịch vụ hosted chạy trong ứng dụng - khanhnv
    /// chịu trách nhiệm khởi động và duy trì sub channel Redis thông qua IPriceGateway
    /// </summary>
    public class PriceGatewayListenerService : IHostedService
    {
        private readonly IPriceGateway _priceGateway;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="priceGateway"></param>
        public PriceGatewayListenerService(IPriceGateway priceGateway)
        {
            _priceGateway = priceGateway;
        }
        /// <summary>
        /// func này được gọi khi service khởi động
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _priceGateway.StartListeningToRedisChannel();
            return Task.CompletedTask;
        }
        /// <summary>
        /// func này được gọi khi service dừng lại
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Nếu cần dọn dẹp tài nguyên, xử lý tại đây
            return Task.CompletedTask;
        }
    }
}
