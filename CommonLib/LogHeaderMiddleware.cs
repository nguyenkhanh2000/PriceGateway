using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public static class LogHeaderMiddleware
    {
        public const string CLIENT_NAME = "EzTradeClient";
        public static void AddLogHeader(this IServiceCollection services)
        {
            services.AddTransient<RequestHandler>();
            // sử dụng HttpclientFactory để thêm header sessionId vào mỗi request gửi đi tới api
            services.AddHttpClient(CLIENT_NAME).AddHttpMessageHandler<RequestHandler>();
            services.AddSingleton<IHttpClientProvider, HttpClientProvider>();
        }
    }
}
