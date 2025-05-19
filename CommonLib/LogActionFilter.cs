using CommonLib.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Entities;

namespace CommonLib
{
    public class LogActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // ghi log global khi request được gửi lên
            var s6GApp = context.HttpContext.RequestServices.GetService(typeof(IS6GApp)) as IS6GApp;
            var request = context.HttpContext.Request;
            var logMessage = @$"
Token = {request.Cookies[EGlobalConfig.SESSION_TOKEN]},
UserAgent = {request.Headers["User-Agent"]}
URL = {string.Concat(
                request.Scheme,
                "://",
                request.Host.ToUriComponent(),
                request.PathBase.ToUriComponent(),
                request.Path.ToUriComponent(),
                request.QueryString.ToUriComponent())} 
            ";
            s6GApp?.InfoLogger.LogInfo(logMessage);
        }
    }
}
