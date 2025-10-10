using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PriceGateway.BLL;
using PriceGateway.Interfaces;
using SystemCore.Entities;

namespace PriceGateway.Controllers
{
    [Route(CPriceConfig.__ROUTE_API_GET_SESSION)]  //Định nghĩa route của api
    [ApiController]
    public class ApiGetSession : Controller
    {
        public readonly IS6GApp _cS6GApp;
        private readonly IPriceHandle _handle;
        public ApiGetSession(IPriceHandle handler, IS6GApp cS6GApp)
        {
            this._handle = handler;
            this._cS6GApp = cS6GApp;
        }
        [HttpGet]
        public async Task<IActionResult> Api_Get_Session(string typemsg)
        {
            try
            {
                string Exchange = HttpContext.Request.Query["exchange"];
                //1.handle
                EResponseResult responseResult = await this._handle.fnc_Get_Session(typemsg, Exchange);
                string json = JsonConvert.SerializeObject(responseResult);
                // 2. return response (code 200)
                return Content(json);
            }
            catch (Exception ex)
            {
                // log error + buffer data
                this._cS6GApp.ErrorLogger.LogError(ex);
                EResponseResult responseResult = new EResponseResult() { Code = EGlobalConfig.__CODE_ERROR_IN_LAYER_GUI, Message = ex.Message, Data = null };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(responseResult);
                // return null
                return Content(json);
            }
        }
    }
}
