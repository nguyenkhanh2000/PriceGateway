using CommonLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Entities;
using SystemCore.Temporaries;

namespace CommonLib.Implementations
{
    /// <summary>
    /// class ghi log error
    /// </summary>
    public class CErrorLogger : CBaseLogger, IErrorLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string __TEMPLATE = @"=================
Message     = {0}
StackTrace  = {1}
TargetSite  = {2}";


        private const string __TEMPLATE_CONTEXT = @"=================
ContextId   = {0} ({1}) [{2}]
LastContext = {3}
Message     = {4}
StackTrace  = {5}
TargetSite  = {6}";

        private const string __LOG_FOLDER = "ERROR";

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        public CErrorLogger(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, bool randomFileName = false)
            : base(configuration, __LOG_FOLDER, randomFileName)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 2019-01-03 15:55:18 ngocta2
        /// ghi log error
        /// </summary>
        /// <param name="ex"></param>
        public void LogError(Exception ex)
        {
            // this._logger.Error(
            //     __TEMPLATE,
            //     ex.Message,
            //     ex.StackTrace?.TrimStart(),
            //     ex.TargetSite);

            // Tạo file log name từ http context

            var fileLogName = GetLogFileName(_httpContextAccessor);
            this.LogError(fileLogName, ex);
        }

        public void LogErrorLogReader(Exception ex)
        {
            // this._logger.Error(
            //     __TEMPLATE,
            //     ex.Message,
            //     ex.StackTrace?.TrimStart(),
            //     ex.TargetSite);

            // Tạo file log name từ http context

            var fileLogName = GetLogFileNameLogReader(_httpContextAccessor);
            this.LogError(fileLogName, ex);
        }


        /// <summary>
        /// 2019-01-08 14:37:47 ngocta2
        /// log them contextData de chi tiet hon cac nguyen nhan gay ra error
        /// 2019-01-24 11:21:42 ngocta2
        /// them managedThreadId de phan biet code run giua cac thread khac nhau
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="executionContext"> id + data </param>
        public void LogErrorContext(Exception ex, TExecutionContext ec)
        {
            //         // 1. log vao file cua Serilog
            //         this._logger.Error(
            //             __TEMPLATE_CONTEXT,
            //             ec.Id,
            //             ThreadId,
            //             TaskId,
            //             ec.LastContext+ $" ; executionContext.Data={ec.Data}", // 2019-11-20 17:19:25 ngocta2 bo, nhung bo thi run Test_LogError_Context ra failed
            // ex.Message,
            //             ex.StackTrace?.TrimStart(),
            //             ex.TargetSite);
            //
            //         // 2. log vao buffer
            //         WriteBufferMid(ec, ex.Message, true);



            var fileLogName = GetLogFileName(_httpContextAccessor);

            this.LogErrorContext(fileLogName, ec, ex);
        }

        /// <summary>
        /// 2022-05-16 LinhNH
        /// log them contextData de chi tiet hon cac nguyen nhan gay ra error
        /// Ghi log theo số tài khoản KH
        /// them managedThreadId de phan biet code run giua cac thread khac nhau
        /// </summary>
        /// <param name="Stk">Số tài khoản khách hàng</param>
        /// <param name="ex"></param>
        /// <param name="executionContext"> id + data </param>
        public void LogErrorContext(string Stk, Exception ex, TExecutionContext ec)
        {
            //         // 1. log vao file cua Serilog
            //         this._logger.Error(
            //             __TEMPLATE_CONTEXT,
            //             ec.Id,
            //             ThreadId,
            //             TaskId,
            //             ec.LastContext+ $" ; executionContext.Data={ec.Data}", // 2019-11-20 17:19:25 ngocta2 bo, nhung bo thi run Test_LogError_Context ra failed
            // ex.Message,
            //             ex.StackTrace?.TrimStart(),
            //             ex.TargetSite);
            //
            //         // 2. log vao buffer
            //         WriteBufferMid(ec, ex.Message, true);



            var fileLogName = GetLogFileName(Stk);

            this.LogErrorContext(fileLogName, ec, ex);
        }

        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="Stk"></param>
        /// <param name="ex"></param>
        public void LogErrorContextIP(string Stk, Exception ex, TExecutionContext ec)
        {
            var fileLogName = GetLogFileNameLogIP(Stk, _httpContextAccessor);

            this.LogErrorContext(fileLogName, ec, ex);
        }

        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ex"></param>
        public void LogError(string fileName, Exception ex)
        {

            string formatData = EGlobalConfig.DateTimeNow + __TEMPLATE
                .Replace("{0}", ex.Message)
                .Replace("{1}", ex.StackTrace?.TrimStart())
                .Replace("{2}", ex.TargetSite?.ToString());

            this.LogToFile(fileName, formatData);
        }


        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="executionContext"></param>
        /// <param name="exception"></param>
        public void LogErrorContext(string fileName, TExecutionContext executionContext, Exception exception)
        {
            string formatData = EGlobalConfig.DateTimeNow + __TEMPLATE_CONTEXT
                .Replace("{0}", executionContext.Id)
                .Replace("{1}", ThreadId.ToString())
                .Replace("{2}", TaskId.ToString())
                .Replace("{3}", executionContext.LastContext + $" ; executionContext.Data={executionContext.Data}")
                .Replace("{4}", exception.Message)
                .Replace("{5}", exception.StackTrace?.TrimStart())
                .Replace("{6}", exception.TargetSite?.ToString());
            this.LogToFile(fileName, formatData);
        }
    }
}
