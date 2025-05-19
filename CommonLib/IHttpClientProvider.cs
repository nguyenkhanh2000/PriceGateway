using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Tạo client đã có header sessionToken
        /// </summary>
        /// <returns></returns>
        public HttpClient CreateClient();

        /// <summary>
        /// Serialize object sang Json để gửi đi
        /// </summary>
        /// <param name="contentValue">object cần gửi</param>
        /// <returns></returns>
        public StringContent SerializeContent(object contentValue);
    }
}
