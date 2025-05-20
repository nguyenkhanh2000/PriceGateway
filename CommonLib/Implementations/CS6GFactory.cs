using CommonLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonitorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Implementations
{
    /// <summary>
    /// factory class de tao ra cac singleton object (1 project chi duoc tao 1 instance)
    /// 1 project chỉ duoc co 1 S6GApp instance
    /// S6GApp = common + debug logger + error logger + sql logger + monitor + config (la file config rieng cua tung project => appsettings.json)
    /// </summary>
    public static class CS6GFactory
    {
        static private IConfiguration _configuration = null;
        static private CErrorLogger _cErrorLogger = null;
        static private CDebugLogger _cDebugLogger = null;
        static private CSqlLogger _cSqlLogger = null;
        static private CInfoLogger _cInfoLogger = null;
        static private CCommon _cCommon = null;
        static private CMonitor _cMonitor = null;
        static private CS6GApp _s6GApp = null;
        static private object _locker = new object(); // locker, tranh error voi multi-thread code
        static private string _configFolderPath = null;
        static private string _configFileName = null;

        // hiendv 
        private static IHttpContextAccessor _httpContextAccessor;

        private const string __JSON_CONFIG_FILE = "appsettings.json";

        /// <summary>
        /// 2019-01-09 09:07:33 ngocta2
        /// fixed config
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// 
        /// CS6GFactory.SetConfigPath(Directory.GetCurrentDirectory()); trong console app thi ra dung path nhung web app pool thi sai
        /// 
        /// https://stackoverflow.com/questions/43709657/how-to-get-root-directory-of-project-in-asp-net-core-directory-getcurrentdirect
        /// Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //using (FileStream stream = new FileStream($"D:\\temp4\\GetConfigInstance.txt", FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
        //using (StreamWriter sw = new StreamWriter(stream))
        //{
        //sw.WriteLine($"{EGlobalConfig.DateTimeNow} -- GetConfigInstance() -- _configFolderPath={_configFolderPath}");
        //}
        /// </summary>
        /// <returns></returns>
        static public IConfiguration GetConfigInstance()
        {
            lock (_locker)
            {
                if (_configuration == null)
                {
                    if (_configFolderPath == null)
                    {
                        //_configFolderPath = Directory.GetCurrentDirectory(); //_configFolderPath=c:\windows\system32\inetsrv
                        _configFolderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); //_configFolderPath=D:\temp\TAChartRealtimeService
                    }

                    if (_configFileName == null)
                        _configFileName = __JSON_CONFIG_FILE;

                    //if (File.Exists(_configFolderPath + "\\" + _configFileName))
                    _configuration = new ConfigurationBuilder()
                        .SetBasePath(_configFolderPath)
                        .AddJsonFile(_configFileName, optional: true, reloadOnChange: true)
                        .Build();
                }

                return _configuration;
            }
        }

        /// <summary>
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        static public CErrorLogger GetErrorLoggerInstance()
        {
            lock (_locker)
            {
                if (_cErrorLogger == null)
                {
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();

                    _cErrorLogger = new CErrorLogger(configuration, _httpContextAccessor);
                }

                return _cErrorLogger;
            }
        }


        /// <summary>
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        static public CDebugLogger GetDebugLoggerInstance()
        {
            lock (_locker)
            {
                if (_cDebugLogger == null)
                {
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();

                    _cDebugLogger = new CDebugLogger(configuration);
                }

                return _cDebugLogger;
            }
        }

        /// <summary>
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static CSqlLogger GetSqlLoggerInstance()
        {
            lock (_locker)
            {
                if (_cSqlLogger == null)
                {
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();

                    _cSqlLogger = new CSqlLogger(configuration, _httpContextAccessor);
                }

                return _cSqlLogger;
            }
        }

        /// <summary>
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static CInfoLogger GetInfoLoggerInstance()
        {
            lock (_locker)
            {
                if (_cInfoLogger == null)
                {
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();

                    _cInfoLogger = new CInfoLogger(configuration, _httpContextAccessor);
                }

                return _cInfoLogger;
            }
        }

        /// <summary>
        /// 2019-01-09 14:46:12 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <param name="cErrorLogger"></param>
        /// <param name="cDebugLogger"></param>
        /// <returns></returns>
        public static CCommon GetCommonInstance()
        {
            lock (_locker)
            {
                if (_cCommon == null)
                {
                    CErrorLogger cErrorLogger = CS6GFactory.GetErrorLoggerInstance();
                    CDebugLogger cDebugLogger = CS6GFactory.GetDebugLoggerInstance();
                    _cCommon = new CCommon(cErrorLogger, cDebugLogger);
                }

                return _cCommon;
            }
        }

        /// <summary>
        /// 2019-01-15 09:10:52 ngocta2
        /// 1. chi duoc tao 1 instance
        /// 2. locker chong multi-thread tao nhieu instance
        /// </summary>
        /// <returns></returns>
        public static CMonitor GetMonitorInstance()
        {
            lock (_locker)
            {
                if (_cMonitor == null)
                {
                    CErrorLogger cErrorLogger = CS6GFactory.GetErrorLoggerInstance();
                    CDebugLogger cDebugLogger = CS6GFactory.GetDebugLoggerInstance();
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();
                    CCommon cCommon = CS6GFactory.GetCommonInstance();
                    _cMonitor = new CMonitor(cErrorLogger, cDebugLogger, cCommon, configuration);

                }

                return _cMonitor;
            }
        }

        /// <summary>
        /// 2019-01-09 09:27:32 ngocta2
        /// singleton, ko duoc tao new instance moi lan call
        /// tao 2 instance S6GApp tuong duong tao 2 instance CDebugLogger khac nhau
        /// 2 instance khac nhau cung access 1 file thi error
        /// day la nguyen nhan neu tao new instance thi ghi log ko du data
        /// instance tao sau ko duoc access vao file log de append data
        /// </summary>
        /// <returns></returns>
        public static CS6GApp GetS6GAppInstance()
        {
            lock (_locker)
            {
                if (_s6GApp == null)
                {
                    CErrorLogger cErrorLogger = CS6GFactory.GetErrorLoggerInstance();
                    CDebugLogger cDebugLogger = CS6GFactory.GetDebugLoggerInstance();
                    CSqlLogger cSqlLogger = CS6GFactory.GetSqlLoggerInstance();
                    CInfoLogger cInfoLogger = CS6GFactory.GetInfoLoggerInstance();
                    CCommon cCommon = CS6GFactory.GetCommonInstance();
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();
                    CMonitor cMonitor = CS6GFactory.GetMonitorInstance();
                    _s6GApp = new CS6GApp(cErrorLogger, cDebugLogger, cSqlLogger, cInfoLogger, cCommon, configuration, cMonitor);
                }

                return _s6GApp;
            }
        }

        /// <summary>
        /// Thêm service provider để sử dụng các service
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static CS6GApp GetS6GAppInstance(IHttpContextAccessor httpContextAccessor)
        {
            lock (_locker)
            {
                if (_s6GApp == null)
                {
                    _httpContextAccessor = httpContextAccessor;

                    CErrorLogger cErrorLogger = CS6GFactory.GetErrorLoggerInstance();
                    CDebugLogger cDebugLogger = CS6GFactory.GetDebugLoggerInstance();
                    CSqlLogger cSqlLogger = CS6GFactory.GetSqlLoggerInstance();
                    CInfoLogger cInfoLogger = CS6GFactory.GetInfoLoggerInstance();
                    CCommon cCommon = CS6GFactory.GetCommonInstance();
                    IConfiguration configuration = CS6GFactory.GetConfigInstance();
                    CMonitor cMonitor = CS6GFactory.GetMonitorInstance();
                    _s6GApp = new CS6GApp(cErrorLogger, cDebugLogger, cSqlLogger, cInfoLogger, cCommon, configuration, cMonitor);
                }

                return _s6GApp;
            }
        }

        /// <summary>
        /// Register CS6GApp as singleton
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppLogger(this IServiceCollection services)
        {
            //services.AddDistributedMemoryCache();
            //services.AddSession();
            services.AddHttpContextAccessor();
            services.AddSingleton<IInfoLogger, CInfoLogger>();
            services.AddSingleton<IErrorLogger, CErrorLogger>();
            services.AddSingleton<IDebugLogger, CDebugLogger>();
            services.AddSingleton<ISqlLogger, CSqlLogger>();
            services.AddSingleton<ICommon, CCommon>();
            services.AddSingleton<IMonitor, CMonitor>();
            services.AddSingleton<IS6GApp, CS6GApp>();
        }
    }
}
