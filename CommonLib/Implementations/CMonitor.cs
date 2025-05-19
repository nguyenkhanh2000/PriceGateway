using CommonLib.Interfaces;
using Microsoft.Extensions.Configuration;
using MonitorCore.Enums;
using MonitorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Implementations
{
    /// <summary>
    /// monitor toan bo he thong Stock6G
    /// </summary>
    public class CMonitor : IMonitor
    {
        // vars
        private readonly IErrorLogger _errorLogger;
        private readonly IDebugLogger _debugLogger;
        private readonly ICommon _common;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <param name="debugLogger"></param>
        public CMonitor(IErrorLogger errorLogger, IDebugLogger debugLogger, ICommon common, IConfiguration configuration)
        {
            this._errorLogger = errorLogger;
            this._debugLogger = debugLogger;
            this._common = common;
            this._configuration = configuration;
        }

        /// <summary>
        /// 2019-01-15 09:16:42 ngocta2
        /// tu enum lay ra app name
        /// </summary>
        /// <param name="appList"></param>
        /// <returns></returns>
        public string GetAppName(AppList appList)
        {
            switch (appList)
            {
                case AppList.DSCacheApiService: return "DSCacheApiService";
                case AppList.DSMarketWatchApiService: return "DSMarketWatchApiService";
                case AppList.DSReportApiService: return "DSReportApiService";
                default: return "";
            }
        }

        /// <summary>
        /// 2019-01-15 09:42:37 ngocta2
        /// send tinh trang cho center monitor
        /// </summary>
        /// <param name="appList"></param>
        /// <param name="statusData"></param>
        /// <returns></returns>
        public bool SendStatusToMonitor(AppList appList, string statusData)
        {
            try
            {
                // process ....

                // send ...
                this.SendStatusToMonitor(this._common.GetLocalDateTime(), this._common.GetLocalIp(), appList, statusData);

                return true;
            }
            catch (Exception ex)
            {
                this._errorLogger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 2019-01-15 10:05:09 ngocta2
        /// private function, send status cho monitor
        /// </summary>
        /// <param name="localDateTime">datetime cua server host app, ko phai datetime cua server host monitor</param>
        /// <param name="localIp"></param>
        /// <param name="appList"></param>
        /// <param name="statusData"></param>
        private void SendStatusToMonitor(string localDateTime, string localIp, AppList appList, string statusData)
        {
            try
            {
                // su dung ZeroMQ de send status ve monitor

            }
            catch (Exception ex)
            {
                this._errorLogger.LogError(ex);
            }
        }

    }
}
