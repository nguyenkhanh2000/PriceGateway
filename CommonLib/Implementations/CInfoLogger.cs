using CommonLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Entities;
using SystemCore.Temporaries;

namespace CommonLib.Implementations
{
    /// <summary>
	/// 2019-01-04 14:58:37 ngocta2
	/// class log script truy van SQL Server / Oracle / Redis
	/// </summary>
	public class CInfoLogger : CBaseLogger, IInfoLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        private const string __TEMPLATE = @"=================
Source  = {0}
Data    = {1}";

        private const string __TEMPLATE_CONTEXT = @"{0} = {1} ({2}) [{3}] <<{4}>> => {5}";

        private const string __TYPE_FOLDER = "INFO";

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        public CInfoLogger(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, bool randomFileName = false)
            : base(configuration, __TYPE_FOLDER, randomFileName)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 2019-01-03 15:55:18 ngocta2
        /// ghi log error
        /// </summary>
        /// <param name="ex"></param>
        public void LogInfo(string data)
        {
            var fileLogName = GetLogFileName(_httpContextAccessor);

            this.LogInfo(fileLogName, GetDeepCaller(), data);
        }


        /// <summary>
        /// 2019-01-24 11:21:42 ngocta2
        /// them managedThreadId de phan biet code run giua cac thread khac nhau
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ec"></param>
        public void LogInfoContext(TExecutionContext ec, string data)
        {
            // Tạo file log name từ http context
            var httpContext = _httpContextAccessor.HttpContext;
            // lay ip client
            var IPClient = "";
            var headers = httpContext?.Request.Headers;//ip client
            if (headers == null)
            {
                IPClient = httpContext?.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            else
            {
                if (headers.ContainsKey("X-Forwarded-For"))
                {

                    var checkIP = IPAddress.Parse(headers["X-Forwarded-For"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0]);
                    if (checkIP != null)
                    {
                        IPClient = checkIP.ToString();
                    }
                }
                else
                {
                    IPClient = httpContext?.Connection.RemoteIpAddress.MapToIPv4().ToString();
                }
            }
            // Lấy client code từ request header nếu là api, ds còn lấy client code từ session nếu là webapp
            var clientCode = httpContext.Request.Headers[EGlobalConfig.SESSION_CLIENT_CODE].ToString() ??
                             httpContext.Session.GetString(EGlobalConfig.SESSION_CLIENT_CODE) ?? " ";
            var fileLogName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}_{clientCode}_{IPClient}";

            this.LogInfoContext(fileLogName, GetDeepCaller(), ec, data);
        }


        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public void LogInfo(string fileName, string caller, string data)
        {
            string formatData = EGlobalConfig.DateTimeNow + __TEMPLATE
                .Replace("{0}", caller)
                .Replace("{1}", data);

            this.LogToFile(fileName, formatData);
        }

        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="executionContext"></param>
        /// <param name="data"></param>
        public void LogInfoContext(string fileName, string caller, TExecutionContext executionContext, string data)
        {
            string formatData = EGlobalConfig.DateTimeNow + __TEMPLATE_CONTEXT
                .Replace("{0}", caller)
                .Replace("{1}", data)
                .Replace("{2}", ThreadId.ToString())
                .Replace("{3}", TaskId.ToString())
                .Replace("{4}", executionContext.ElapsedMilliseconds.ToString())
                .Replace("{5}", executionContext.Id);
            this.LogToFile(fileName, formatData);
        }
    }
}
