using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class RequestHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHandler(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Thêm header session và clientCode vào mỗi request từ httpclient
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var sessionId = _httpContextAccessor.HttpContext.Request.Cookies[EGlobalConfig.SESSION_TOKEN];
            //var clientCode = _httpContextAccessor.HttpContext.Session.GetString(EGlobalConfig.SESSION_CLIENT_CODE);
            //request.Headers.Add(EGlobalConfig.SESSION_TOKEN, sessionId); // Getting correlationid from request context. 
            //request.Headers.Add(EGlobalConfig.SESSION_CLIENT_CODE, clientCode); // Getting correlationid from request context. 
            return base.SendAsync(request, cancellationToken);
        }

    }
}
