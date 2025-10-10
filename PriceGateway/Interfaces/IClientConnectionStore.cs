using PriceGateway.Models;

namespace PriceGateway.Interfaces
{
    public interface IClientConnectionStore
    {
        void Add(CSignalRCounter counter);
        void Remove(string ConnectionID);
        List<CSignalRCounter> GetAll();
    }
}
