using PriceGateway.Interfaces;
using PriceGateway.Models;
using System.Collections.Concurrent;

namespace PriceGateway.BLL
{
    public class ClientConnectionStore : IClientConnectionStore
    {
        private readonly ConcurrentDictionary<string, CSignalRCounter> _clients = new();

        public void Add(CSignalRCounter client) 
        {
            _clients[client.ConnectionID] = client;
        }
        public void Remove(string connectionID) 
        {
            _clients.TryRemove(connectionID, out _);
        }
        public List<CSignalRCounter> GetAll()
        {
            return _clients.Values.ToList();    
        }
    }
}
