using CommonLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemCore.Entities;
using SystemCore.SharedKernel;
using SystemCore.Temporaries;
using ILogger = Serilog.ILogger;

namespace CommonLib.Implementations
{
    /// <summary>
    /// 2019-01-03 16:00:47 ngocta2
    /// base class cho cac logger class khac
    /// 
    /// app config json file phai co format
    //{
    //  "Serilog": {
    //    "Using": [ "Serilog.Sinks.Async" ],
    //    "MinimumLevel": "Debug",
    //    "WriteTo": [
    //      {
    //        "Name": "File",
    //        "Args": {
    //          "path": "D:\\WebLog\\S6G\\CommonLib.Tests"
    //        }
    //      }
    //    ],
    //    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    //    "AllowedHosts": "*"
    //  }
    //}
    /// </summary>
    public abstract class CBaseLogger : CInstance, IDisposable
    {
        // const
        public const string __CONFIG_PATH_POS = "Serilog:WriteTo:0:Args:path"; // 2019-08-23 09:06:40 ngocta2 can public de lay sau nay // 2019-09-25 13:10:09 ngocta2 "Serilog:WriteTo:0:Args:appPath" => "Serilog:WriteTo:0:Args:path"
        public const string __CONFIG_FILE_NAME = "appsettings.json"; // 2019-08-23 09:06:40 ngocta2 can public de lay sau nay // 2019-09-25 13:10:09 ngocta2 "Serilog:WriteTo:0:Args:appPath" => "Serilog:WriteTo:0:Args:path"
        protected const string __DATE_FORMAT = "yyyyMMdd";
        protected const string __LOG_EXT = "js";
        protected const string __TYPE_FOLDER_SQL = "SQL";
        protected const string __TYPE_FOLDER_ERROR = "ERROR";
        protected const string __TYPE_FOLDER_INFO = "INFO";
        protected const string __TYPE_FOLDER_DEBUG = "DEBUG";
        protected const string __REGEX_FIX_FILENAME = @"[\\/:*?""<>|]";

        // var
        protected bool _debugFlag = false;
        protected ILogger _logger;
        private string _fullLogPath;
        private readonly bool _randomFileName;
        private readonly string _logDirPath; // 2019-08-23 09:08:57 ngocta2 => D:\WebLog\S6G\CommonLib.Tests\DEBUG
        private readonly string _logRootPath; // 2019-08-23 09:08:57 ngocta2, 2019-09-25 13:21:11 ngocta2 => D:\WebLog\S6G\CommonLib.Tests

        // prop (public)
        public ILogger Logger { get { return this._logger; } }
        public string FullLogPath { get { return this._fullLogPath; } } // public ra de sau dung cho unit test, read lai file, check data
        public string Timestamp { get { return DateTime.Now.ToString(EGlobalConfig.__DATETIME_REDIS_SCORE); ; } } // moc time exec code

        public int ThreadId { get { return Thread.CurrentThread.ManagedThreadId; } } // thread info
        public string ThreadIdS { get { return ThreadId.ToString(EGlobalConfig.__LEADING_ZERO_THREAD_ID); } } // thread info

        public int TaskId { get { return Task.CurrentId == null ? 0 : Convert.ToInt32(Task.CurrentId); } } // task info
        public string TaskIdS { get { return TaskId.ToString(EGlobalConfig.__LEADING_ZERO_TASK_ID); } } // task info
        public string LogDirPath { get { return this._logDirPath; } } // public ra de sau dung cho unit test, read lai file, check data
        public string LogRootPath { get { return this._logRootPath; } } // public ra de sau dung cho unit test, read lai file, check data

        /// <summary>
        /// // func info
        /// </summary>
        public string FunctionName
        {
            get
            {
                return (new StackTrace().GetFrame(2).GetMethod()).Name;
                //return GetCaller(3);
            }
        }


        /// <summary>
        /// constructor
        /// init path folder ghi log, chu y la phai set quyen everyone full control
        /// neu set sai path dir ghi log thi se ghi lung tung vao root cua disk, co khi D:\ C:\
        //
        //using (FileStream stream = new FileStream($"D:\\temp4\\GetConfigInstance.txt", FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
        //using (StreamWriter sw = new StreamWriter(stream))
        //{
        //	sw.WriteLine($"{EGlobalConfig.DateTimeNow} -- GetConfigInstance() -- this._appPath={this._appPath}; this._logDirPath={this._logDirPath}; typeFolder={typeFolder}; fullLogPath={fullLogPath}");
        //}
        //.WriteTo.File(fullLogPath, Serilog.Events.LogEventLevel.Debug, "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}",null,10000000)
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="typeFolder">DEBUG/SQL/ERROR</param>
        /// <param name="randomFileName">true/false</param>
        public CBaseLogger(IConfiguration configuration, string typeFolder, bool randomFileName = false)
        {
            this._randomFileName = randomFileName;


            if (configuration != null)
            {
                // 2019-08-23 09:12:55 ngocta2 -> can lay log dir path
                this._logRootPath = configuration.GetSection(__CONFIG_PATH_POS).Value;
                this._logDirPath = $"{this._logRootPath}\\{typeFolder}";

                // D:\WebLog\StockGateway.NETCore\CommonLib.Tests (file, ko phai dir)
                string fullLogPath = GetFullPath(configuration.GetSection(__CONFIG_PATH_POS).Value, typeFolder);

                // D:\WebLog\StockGateway.NETCore\CommonLib.Tests\ERROR\20191114.js
                string dirPath = Path.GetDirectoryName(fullLogPath);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);


            }
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~CBaseLogger()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// free resource
        /// Dispose() calls Dispose(true)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources			
                this.FreeResource();
            }
            // free native resources if there are any.			
        }

        /// <summary>
        /// destructor
        /// </summary>
        public void FreeResource()
        {
            // At application shutdown (results in monitors getting StopMonitoring calls)
            Log.CloseAndFlush();
            this._logger = null; // dong nay ko can thiet
        }

        /// <summary>
        /// 2019-01-03 15:54:32 ngocta2
        /// get folder ghi log, tuy vao loai log ma folder khac nhau, nhung deu la folder con cua appPath
        /// </summary>
        /// <param name="rootFolder">D:\WebLog\S6G\CommonLib.Tests</param>
        /// <param name="subFolder">DEBUG/SQL/ERROR</param>
        /// <returns></returns>
        private string GetFullPath(string rootFolder, string subFolder)
        {
            string random = this._randomFileName ? "_" + Guid.NewGuid().ToString().Substring(0, 8) : "";
            this._fullLogPath = $"{rootFolder}\\{subFolder}\\{DateTime.Now.ToString(__DATE_FORMAT)}{random}.{__LOG_EXT}";
            return this._fullLogPath;
        }

        /// <summary>
        /// 2019-01-03 16:16:35 ngocta2
        /// ke thua tu 5G: xac dinh vi tri function dang run
        /// </summary>
        /// <returns></returns>
        protected string GetDeepCaller()
        {
            string strCallerName = "";
            for (int i = 3; i >= 3; i--)
                strCallerName += GetCaller(i);//

            //returns a composite of the namespace, class and method name.
            return strCallerName;
        }

        /// <summary>
        /// 2019-01-03 16:16:35 ngocta2
        /// ke thua tu 5G: xac dinh vi tri function dang run
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetCaller(int level = 2)
        {
            var m = new StackTrace().GetFrame(level).GetMethod();

            if (m.DeclaringType == null) return ""; //9:33 AM 6/18/2014 Exception Details: System.NullReferenceException: Object reference not set to an instance of an object.

            // .Name is the name only, .FullName includes the namespace
            var className = m.DeclaringType.FullName;

            //the method/function name you are looking for.
            var methodName = m.Name;

            // temp
            var temp = className + "->" + methodName;

            //returns a composite of the namespace, class and method name.
            return temp;
        }

        /// <summary>
        /// 2019-01-31 11:00:41 ngocta2
        /// </summary>
        /// <param name="ec"></param>
        /// <param name="data"></param>
        public void WriteBufferMid(TExecutionContext ec, string data, bool force = false)
        {
            // chi write vao buffer khi _flag set = true
            // xu ly string nhieu gay cham
            if (this._debugFlag || force)
            {
                string datetime = DateTime.Now.ToString(EGlobalConfig.__DATETIME_LOG);
                string dataToLog = $"\t{datetime} => {data}";
                ec.Buffer.AppendLine(dataToLog);
                ec.LastContext = dataToLog;
            }
        }

        /// <summary>
        /// 2019-09-26 10:10:00 ngocta2
        /// log chia nho thanh nhieu file tai 1 folder
        /// ap dung cho tat ca log: INFO, ERROR, DEBUG, SQL
        /// </summary>
        /// <param name="__TEMPLATE_CONTEXT"></param>
        /// <param name="ec"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public void LogSub(string __TEMPLATE_CONTEXT, string __TYPE_FOLDER, string caller, TExecutionContext ec, string fileName, string data)
        {
            try
            {
                fileName = $"{Regex.Replace(fileName, __REGEX_FIX_FILENAME, "_", RegexOptions.IgnoreCase | RegexOptions.Multiline)}.{__LOG_EXT}";
                string fullFilePath = $"{LogDirPath}\\{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}\\{fileName}";
                string message = "";
                if (ec == null)
                {
                    /*
					2019-09-26 09:42:32.511=================
					BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow
					e3b6a216-df70-48b0-832e-348b12acb8f6
					 */
                    message = $"{EGlobalConfig.DateTimeNow}=================\r\n{caller}\r\n{data}";
                }
                else
                {
                    if (__TYPE_FOLDER == __TYPE_FOLDER_SQL)
                    {
                        /*
						2019-09-26 09:41:47.934=================
						Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow => 2019-01-24 11:30:44 ngocta2 (14) [1]
						Data    = fee6ae7a-543b-4770-88a9-e650a835a187
						 */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Id)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.Data + EGlobalConfig.__STRING_RETURN_NEW_LINE + data);
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";
                    }

                    if (__TYPE_FOLDER == __TYPE_FOLDER_DEBUG)
                    {
                        /*
						2019-09-26 10:39:21.310=================
						Source  = CommonLib.Implementations.CDebugLogger->LogDebugSub
						EId     = 2019-01-24 11:30:44 ngocta2 (13) [1] BeginF
						EId     = 2019-01-24 11:30:44 ngocta2 (13) [1] EndF <<320>>
						Finally = 569e68e5-5ab3-4b55-8393-47edee04ac15
						 */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Id)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.Buffer.ToString())
                            .Replace("{5}", ec.Id)
                            .Replace("{6}", ThreadId.ToString())
                            .Replace("{7}", TaskId.ToString())
                            .Replace("{8}", ec.ElapsedMilliseconds.ToString())
                            .Replace("{9}", data)
                            ;
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";
                    }
                }

                // ko duoc nhieu thread cung write vao 1 file => error
                CCommon.WriteFileStatic(fullFilePath, message);
            }
            catch (Exception)
            {
                // error thi ko ghi log
            }
        }

        /// <summary>
        /// Ghi log chia thư mục theo năm, ngày , tháng
        /// 
        /// </summary>
        public void LogToFile(string __TEMPLATE_CONTEXT, string __TYPE_FOLDER, string caller, TExecutionContext ec, string fileName, string data)
        {
            try
            {
                fileName = $"{Regex.Replace(fileName, __REGEX_FIX_FILENAME, "_", RegexOptions.IgnoreCase | RegexOptions.Multiline)}.{__LOG_EXT}";

                // Lưu chia thư mục theoo từng năm, từng tháng, từng ngày
                string fullFilePath = $"{LogDirPath}\\{DateTime.Now.Year}\\{DateTime.Now.Month.ToString("00")}\\{DateTime.Now.Day.ToString("00")}\\{fileName}";

                string message = "";
                if (ec == null)
                {
                    /*
					2019-09-26 09:42:32.511=================
					BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow
					e3b6a216-df70-48b0-832e-348b12acb8f6
					 */
                    message = $"{EGlobalConfig.DateTimeNow}=================\r\n{caller}\r\n{data}";
                }
                else
                {
                    if (__TYPE_FOLDER == __TYPE_FOLDER_SQL)
                    {
                        /*
						2019-09-26 09:41:47.934=================
						Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow => 2019-01-24 11:30:44 ngocta2 (14) [1]
						Data    = fee6ae7a-543b-4770-88a9-e650a835a187
						 */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Id)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.Data + EGlobalConfig.__STRING_RETURN_NEW_LINE + data);
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";
                    }

                    if (__TYPE_FOLDER == __TYPE_FOLDER_ERROR)
                    {
                        /*
                        2019-09-26 09:41:47.934=================
                        Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow => 2019-01-24 11:30:44 ngocta2 (14) [1]
                        Data    = fee6ae7a-543b-4770-88a9-e650a835a187
                         */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Id)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.Data + EGlobalConfig.__STRING_RETURN_NEW_LINE + data);
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";

                    }

                    if (__TYPE_FOLDER == __TYPE_FOLDER_INFO)
                    {
                        /*
                        2019-09-26 09:41:47.934=================
                        Source  = BaseSERedisLib.Implementations.CRedisRepository->ZSet_UpdateRow => 2019-01-24 11:30:44 ngocta2 (14) [1]
                        Data    = fee6ae7a-543b-4770-88a9-e650a835a187
                         */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Data + EGlobalConfig.__STRING_RETURN_NEW_LINE + data)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.ElapsedMilliseconds.ToString())
                            .Replace("{5}", ec.Id);
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";


                    }

                    if (__TYPE_FOLDER == __TYPE_FOLDER_DEBUG)
                    {
                        /*
						2019-09-26 10:39:21.310=================
						Source  = CommonLib.Implementations.CDebugLogger->LogDebugSub
						EId     = 2019-01-24 11:30:44 ngocta2 (13) [1] BeginF
						EId     = 2019-01-24 11:30:44 ngocta2 (13) [1] EndF <<320>>
						Finally = 569e68e5-5ab3-4b55-8393-47edee04ac15
						 */
                        string formatData = __TEMPLATE_CONTEXT
                            .Replace("{0}", caller)
                            .Replace("{1}", ec.Id)
                            .Replace("{2}", ThreadId.ToString())
                            .Replace("{3}", TaskId.ToString())
                            .Replace("{4}", ec.Buffer.ToString())
                            .Replace("{5}", ec.Id)
                            .Replace("{6}", ThreadId.ToString())
                            .Replace("{7}", TaskId.ToString())
                            .Replace("{8}", ec.ElapsedMilliseconds.ToString())
                            .Replace("{9}", data)
                            ;
                        message = $"{EGlobalConfig.DateTimeNow}{formatData}";
                    }
                }

                // ko duoc nhieu thread cung write vao 1 file => error
                CCommon.WriteFileStatic(fullFilePath, message);
            }
            catch (Exception)
            {
                // error thi ko ghi log
            }
        }

        public void LogToFile(string fileName, string data)
        {
            try
            {
                fileName = $"{Regex.Replace(fileName, __REGEX_FIX_FILENAME, "_", RegexOptions.IgnoreCase | RegexOptions.Multiline)}.{__LOG_EXT}";

                // Lưu chia thư mục theoo từng năm, từng tháng, từng ngày
                string fullFilePath = $"{LogDirPath}\\{DateTime.Now.Year}\\{DateTime.Now.Month.ToString("00")}\\{DateTime.Now.Day.ToString("00")}\\{fileName}";
                // ko duoc nhieu thread cung write vao 1 file => error
                CCommon.WriteFileStatic(fullFilePath, data);
            }
            catch (Exception)
            {
                // error thi ko ghi log
            }
        }

        protected string GetLogFileName(IHttpContextAccessor httpContextAccessor, string Stk = "")
        {
            // Tạo file log name từ http context
            var httpContext = httpContextAccessor?.HttpContext;
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
            //var clientCode =
            //    string.IsNullOrEmpty(httpContext?.Request.Headers[EGlobalConfig.SESSION_CLIENT_CODE].ToString())
            //        ? httpContext?.Session?.GetString(EGlobalConfig.SESSION_CLIENT_CODE)
            //        : httpContext.Request.Headers[EGlobalConfig.SESSION_CLIENT_CODE].ToString();
            var clientCode = httpContext?.Request?.Headers[EGlobalConfig.SESSION_CLIENT_CODE].ToString()
                 ?? httpContext?.Session?.GetString(EGlobalConfig.SESSION_CLIENT_CODE);

            //lay log cho mobile
            if (clientCode == null && httpContext != null)
            {
                clientCode = httpContext.Request.Headers["clientCodeMobile"].ToString();
                var logmobile = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}__{Stk}__{IPClient}";
                return logmobile;
            }
            // Lấy session token từ request header nếu là api, ds còn lấy client code từ cookies nếu là webapp
            var sessionToken =
                string.IsNullOrEmpty(httpContext?.Request.Headers[EGlobalConfig.SESSION_TOKEN].ToString())
                    ? httpContext?.Request.Cookies[EGlobalConfig.SESSION_TOKEN]
                    : httpContext.Request.Headers[EGlobalConfig.SESSION_TOKEN].ToString();

            //var logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}_{clientCode}_{IPClient}_{sessionToken}";
            var logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}__{Stk}__{IPClient}";
            if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(sessionToken))
                logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}__{IPClient}_NotLogin";
            return logFileName;

        }

        protected string GetLogFileName2(string stk, IHttpContextAccessor httpContextAccessor)
        {
            // Tạo file log name từ http context
            var httpContext = httpContextAccessor?.HttpContext;
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
            var logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDDHHMMSS)}__{stk}__{IPClient}";
            return logFileName;

        }

        protected string GetLogFileNameLogReader(IHttpContextAccessor httpContextAccessor)
        {
            // Tạo file log name từ http context
            var httpContext = httpContextAccessor?.HttpContext;
            // lay ipCLiet
            var headers = httpContext.Request.Headers;//ip client
            var IPClient = "";
            if (headers.ContainsKey("X-Forwarded-For"))
            {

                var checkIP = IPAddress.Parse(headers["X-Forwarded-For"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0]);
                if (checkIP != null)
                {
                    IPClient = checkIP.ToString();
                }
            }
            if (headers == null)
            {
                IPClient = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            string logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}__{IPClient}_LogReader5";
            return logFileName;

        }

        /// <summary>
        /// 2022-05-16 LinhNH
        /// log chia nho thanh nhieu file tai 1 folder
        /// ap dung cho tat ca log: INFO, ERROR, DEBUG, SQL của authen_redis ghilog theo stk KH
        /// </summary>
        /// <param name="Stk">Số tài khoan khách hàng</param>
        protected string GetLogFileName(string Stk)
        {
            var logFileName = $@"{Stk}";

            return logFileName;

        }

        /// <summary>
        /// 2022-05-16 NgocNT3
        /// log chia nho thanh nhieu file tai 1 folder
        /// ap dung cho tat ca log: INFO, ERROR, DEBUG, SQL của authen_redis ghilog theo stk KH
        /// </summary>
        /// <param name="Stk">Số tài khoan khách hàng</param>
        protected string GetLogFileNameLogIP(string Stk, IHttpContextAccessor httpContextAccessor)
        {
            // Tạo file log name từ http context
            var httpContext = httpContextAccessor?.HttpContext;
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
            string logFileName = $@"{DateTime.Now.ToString(EGlobalConfig.__DATETIME_YYYYMMDD)}__{Stk}__{IPClient}";
            return logFileName;
        }

        /// <summary>
        /// 2022-05-16 NgocNT3
        /// log chia nho thanh nhieu file tai 1 folder
        /// ap dung cho tat ca log: INFO, ERROR, DEBUG, SQL của authen_redis ghilog theo stk KH
        /// Tên file : Key Redis + IP
        /// </summary>
        /// <param name="Stk">Số tài khoan khách hàng</param>
        protected string GetLogIP(string key, IHttpContextAccessor httpContextAccessor)
        {
            // Tạo file log name từ http context
            var httpContext = httpContextAccessor?.HttpContext;
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
            var logFileName = $@"{key}__{IPClient}";
            return logFileName;

        }
    }
}
