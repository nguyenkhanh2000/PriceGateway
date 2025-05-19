using CommonLib.Interfaces;
using Microsoft.Extensions.Configuration;
using MonitorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemCore.SharedKernel;

namespace CommonLib.Implementations
{
    /// <summary>
    /// bat cu app nao trong 6G cung phai co 1 instance (singleton) nay => 
    ///     LOG: ghi 3 loai log ERROR/DEBUG/SQL
    ///     MONITOR: send status cho monitor + nhan lenh tu monitor
    /// </summary>
    public class CS6GApp : CInstance, IS6GApp, IDisposable
    {

        // vars
        private readonly IErrorLogger _errorLogger;
        private readonly ISqlLogger _sqlLogger;
        private readonly IInfoLogger _infoLogger;
        private readonly IDebugLogger _debugLogger;
        private readonly ICommon _common;
        private readonly IConfiguration _configuration;
        private readonly IMonitor _monitor;

        // props
        public IErrorLogger ErrorLogger { get { return this._errorLogger; } }

        public ISqlLogger SqlLogger { get { return this._sqlLogger; } }
        public IInfoLogger InfoLogger { get { return this._infoLogger; } }

        public IDebugLogger DebugLogger { get { return this._debugLogger; } }

        public ICommon Common { get { return this._common; } }

        public IConfiguration Configuration { get { return this._configuration; } }

        public IMonitor Monitor { get { return this._monitor; } }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <param name="debugLogger"></param>
        /// <param name="sqlLogger"></param>
        /// <param name="common"></param>
        public CS6GApp(IErrorLogger errorLogger, IDebugLogger debugLogger, ISqlLogger sqlLogger, IInfoLogger infoLogger, ICommon common, IConfiguration configuration, IMonitor monitor)
        {
            this._errorLogger = errorLogger;
            this._debugLogger = debugLogger;
            this._sqlLogger = sqlLogger;
            this._infoLogger = infoLogger;
            this._common = common;
            this._configuration = configuration;
            this._monitor = monitor;
        }



        /// <summary>
        /// 2019-01-04 17:24:55 ngocta2
        /// giai phong tai nguyen
        /// </summary>
        public void Dispose()
        {
            this._errorLogger.Dispose();
            this._debugLogger.Dispose();
            this._sqlLogger.Dispose();
        }
    }
}
