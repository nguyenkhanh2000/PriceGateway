using CommonLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemCore.Entities;
using SystemCore.Temporaries;

namespace CommonLib.Implementations
{
    /// <summary>
    /// class log script truy van SQL Server / Oracle / Redis
    /// </summary>
    public class CSqlLogger : CBaseLogger, ISqlLogger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string __TEMPLATE = @"=================
Source  = {0}
Data    = {1}";

        private const string __TEMPLATE_CONTEXT = @"=================
Source  = {0} => {1} ({2}) [{3}]
Data    = {4}";

        private const string __TYPE_FOLDER = __TYPE_FOLDER_SQL;//"SQL";

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        public CSqlLogger(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, bool randomFileName = false)
            : base(configuration, __TYPE_FOLDER, randomFileName)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 2019-01-03 15:55:18 ngocta2
        /// ghi log error
        /// </summary>
        /// <param name="data"></param>
        public void LogSql(string data)
        {
            //this._logger.Information(__TEMPLATE, GetDeepCaller(), data);

            var fileLogName = GetLogFileName(_httpContextAccessor);

            this.LogSql(fileLogName, GetDeepCaller(), data);
        }

        /// <summary>
        /// 2022-05-16 LinhNH
        /// ghi log sql cho authen_redis theo  stk KH
        /// </summary>
        /// <param name="data"></param>
        public void LogSql(string Stk, string data)
        {
            //this._logger.Information(__TEMPLATE, GetDeepCaller(), data);

            //var fileLogName = GetLogFileName(Stk);
            var fileLogName = GetLogFileName(_httpContextAccessor, Stk);

            this.LogSql(fileLogName, GetDeepCaller(), data);
        }

        /// <summary>
        /// 2019-01-24 11:21:42 ngocta2
        /// them managedThreadId de phan biet code run giua cac thread khac nhau
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ec"></param>
        public void LogSqlContext(TExecutionContext ec, string data)
        {
            // this._logger.Information(
            //     __TEMPLATE_CONTEXT,
            //     GetDeepCaller(),
            //     ec.Id,
            //     ThreadId,
            //     TaskId,
            //     data);
            //
            // // 2. log vao buffer
            // WriteBufferMid(ec, data, true);
            var fileLogName = GetLogFileName(_httpContextAccessor);

            this.LogSqlContext(fileLogName, GetDeepCaller(), ec, data);
        }

        public void LogSqlContext2(string stk, TExecutionContext ec, string data)
        {
            // this._logger.Information(
            //     __TEMPLATE_CONTEXT,
            //     GetDeepCaller(),
            //     ec.Id,
            //     ThreadId,
            //     TaskId,
            //     data);
            //
            // // 2. log vao buffer
            // WriteBufferMid(ec, data, true);
            var fileLogName = GetLogFileName2(stk, _httpContextAccessor);

            this.LogSqlContext(fileLogName, GetDeepCaller(), ec, data);
        }

        /// <summary>
        /// 2022-05-16 LinhNH
        /// ghi log sql cho authen_redis theo  stk KH, IPServer
        /// </summary>
        /// <param name="data"></param>
        public void LogSqlIPServer(string Stk, string data)
        {
            var fileLogName = GetLogFileNameLogIP(Stk, _httpContextAccessor);

            this.LogSql(fileLogName, GetDeepCaller(), data);
        }


        /// <summary>
        /// Log mới, lưu theo từng ngày tháng năm
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public void LogSql(string fileName, string caller, string data)
        {
            data = Regex.Replace(data, "(p_aOldPass|p_aNewPass|p_aOldTradePass|p_aNewTradePass|p_apassword)='(.*?)'", "$1='******'");
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
        public void LogSqlContext(string fileName, string caller, TExecutionContext executionContext, string data)
        {
            executionContext.Data = Regex.Replace(executionContext.Data, "(\")(p_aOldPass|p_aNewPass|p_aOldTradePass|p_aNewTradePass|p_apassword|TradingPassword|OTP)(\":\")(.*?)(\")", "$1$2$3******$5");
            string formatData = EGlobalConfig.DateTimeNow + __TEMPLATE_CONTEXT
                .Replace("{0}", caller)
                .Replace("{1}", executionContext.Data)
                .Replace("{2}", ThreadId.ToString())
                .Replace("{3}", TaskId.ToString())
                .Replace("{4}", data);
            this.LogToFile(fileName, formatData);
        }

        /// <summary>
        /// 2019-09-26 09:24:34 ngocta2
        /// edit: hiendv , lưu log theo từng thư mục ngày tháng năm
        /// </summary>
        /// <param name="ec"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public void LogSqlSub(TExecutionContext ec, string fileName, string data)
        {
            //LogSub(__TEMPLATE_CONTEXT, __TYPE_FOLDER, GetDeepCaller(), ec, fileName, data);

            LogToFile(__TEMPLATE_CONTEXT, __TYPE_FOLDER, GetDeepCaller(), ec, fileName, data);
        }

        public void LogToFile(TExecutionContext ec, string fileName, string data)
        {
            string fileNameIP = GetLogIP(fileName, _httpContextAccessor);
            LogToFile(__TEMPLATE_CONTEXT, __TYPE_FOLDER, GetDeepCaller(), ec, fileNameIP, data);
        }

    }
}
