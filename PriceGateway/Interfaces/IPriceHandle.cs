using SystemCore.Entities;

namespace PriceGateway.Interfaces
{
    public interface IPriceHandle
    {
        Task<EResponseResult> fnc_Get_Full_Quote(string Exchange);
    }
}
