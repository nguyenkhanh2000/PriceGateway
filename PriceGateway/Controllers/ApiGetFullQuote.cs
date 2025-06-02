using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceGateway.BLL;
using PriceGateway.Interfaces;
using SystemCore.Entities;

namespace PriceGateway.Controllers
{
    [Route(CPriceConfig.__ROUTE_API_GET_FULL_ROW_QUOTE)]
    [ApiController]
    public class ApiGetFullQuote : Controller
    {
        public readonly IS6GApp _cS6GApp;
        private readonly IPriceHandle _handle;
        public ApiGetFullQuote(IPriceHandle handler, IS6GApp cS6GApp) 
        {
            this._handle = handler;
            this._cS6GApp = cS6GApp;
        }
        [HttpGet]
        public async Task<IActionResult> Api_Get_Full_Quote(string exchange, string typemsg)
        {
            try
            {
                string Board = HttpContext.Request.Query["board"];
                string Symbol = HttpContext.Request.Query["symbol"];
                //1.handle
                EResponseResult responseResult = await this._handle.fnc_Get_Full_Quote(exchange, typemsg, Board, Symbol);
                string json = JsonConvert.SerializeObject(responseResult);
                // 2. return response (code 200)
                return Content(json);
            }
            catch (Exception ex) 
            {
                this._cS6GApp.ErrorLogger.LogError(ex);
                return Content("json");
            }
        }  
        
    }
}
