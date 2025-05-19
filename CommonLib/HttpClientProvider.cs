using Newtonsoft.Json;
using StockCore.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpClientProvider(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public HttpClient CreateClient()
        {
            return _clientFactory.CreateClient(LogHeaderMiddleware.CLIENT_NAME);
        }

        public StringContent SerializeContent(object contentValue)
        {
            return new StringContent(JsonConvert.SerializeObject(contentValue), Encoding.UTF8,
                ResponseModel.RESPONSE_CONTENT_TYPE_JSON);
        }
    }
}
