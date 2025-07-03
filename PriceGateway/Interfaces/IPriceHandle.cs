using SystemCore.Entities;

namespace PriceGateway.Interfaces
{
    public interface IPriceHandle
    {
        Task<EResponseResult> fnc_Get_Full_Quote(string keyName, string TypeMsg, string Board, string Symbol);
        Task<EResponseResult> fnc_Get_String_Seq(string keyName);
        Task<EResponseResult> fnc_Get_Full_Price(string Symbol, string Board);
        Task<EResponseResult> fnc_Get_Session(string TypeMsg, string Exchange);
    }
}
