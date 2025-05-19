using CommonLib.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SystemCore.Entities;
using SystemCore.SharedKernel;
using SystemCore.Temporaries;

namespace CommonLib.Implementations
{
    public class CCommon : CInstance, ICommon
    {
        // const 
        public const string __NULL = "null";// hello after cry
        private const string __STRING_BLANK_OBJECT = "{}";
        private const string BLANK_STIRNG = "";
        public const string TEMPLATE_JS_CODE = "<script language=javascript><!JsCode></script>";

        // vars
        private readonly IErrorLogger _errorLogger;
        private readonly IDebugLogger _debugLogger;
        private static readonly HttpClient _httpClient = new HttpClient();
        private static object locker = new Object();


        //================================================================================

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="errorLogger"></param>
        /// <param name="debugLogger"></param>
        public CCommon(IErrorLogger errorLogger, IDebugLogger debugLogger)
        {
            this._errorLogger = errorLogger;
            this._debugLogger = debugLogger;
        }

        /// =============================== static, no interface ==================================
        /// <summary>
        /// 2019-09-09 10:39:15 ngocta2
        /// tuong tu WriteFile nhung la static method de co the su dung ma ko can init instance class truoc 
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <param name="message"></param>
        public static bool WriteFileStatic(string fullFilePath, string message)
        {
            try
            {
                // kiem tra folder trong full path, neu ko co thi tao folder
                // System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'M:\WebLog\S6G\CommonLib.Tests'.'
                FileInfo fileInfo = new FileInfo(fullFilePath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();



                lock (locker)
                {
                    using (FileStream stream = new FileStream(fullFilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, true))
                    using (StreamWriter sw = new StreamWriter(stream))
                    {
                        sw.WriteLine(message);
                    }
                }
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }
        /// <summary>
        /// get folder hien tai cua Assembly
        /// </summary>
        /// <returns></returns>
        static public string GetCurrDir()
        {
            try
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            catch
            {
                return "";
            }
        }
        /// =============================== /static, no interface ==================================
        /// 

        //https://www.arclab.com/en/kb/csharp/string-operations-left-mid-right-comparision-csharp-mfc.html
        public string Left(string input, int leftCount)
        {
            if (string.IsNullOrEmpty(input))
                return BLANK_STIRNG;
            if (input.Length < leftCount)
                return input;
            else
                return input.Substring(0, leftCount);
        }

        //https://www.arclab.com/en/kb/csharp/string-operations-left-mid-right-comparision-csharp-mfc.html
        public string Right(string input, int rightCount)
        {
            if (string.IsNullOrEmpty(input))
                return BLANK_STIRNG;
            if (input.Length < rightCount)
                return input;
            else
                return input.Substring(input.Length - rightCount, rightCount);
        }

        //https://www.arclab.com/en/kb/csharp/string-operations-left-mid-right-comparision-csharp-mfc.html
        public string Mid(string input, int begin, int count)
        {
            if (string.IsNullOrEmpty(input))
                return BLANK_STIRNG;
            if (input.Length < begin)
                return BLANK_STIRNG;
            if (begin <= 0)
                begin = 1;

            return input.Substring(begin - 1, count);
        }

        /// <summary>
        /// 2019-01-15 10:03:16 ngocta2
        /// ke thua S5G func
        /// lay datetime hien tai theo format
        /// </summary>
        /// <returns></returns>
        public string GetLocalDateTime()
        {
            return DateTime.Now.ToString(EGlobalConfig.__DATETIME_MONITOR);
        }

        /// <summary>
        /// 2019-01-15 10:03:16 ngocta2
        /// ke thua function cua Stock5G de lay ip local
        /// ------------------------
        /// To get the local IP address 
        ///  lay ip hien tai
        ///  "192.168.2.18,172.16.0.18" => chi lay 172.16.0.18
        /// </summary>
        /// <returns></returns>
        public string GetLocalIp()
        {
            try
            {
                string hostName = Dns.GetHostName();
                System.Net.IPHostEntry ipE = Dns.GetHostEntry(hostName);
                IPAddress[] ipAddresses = ipE.AddressList;
                foreach (IPAddress ip in ipAddresses)
                {
                    string localIP = ip.ToString();
                    foreach (string ipPrefix in EGlobalConfig.__PREFIX_IP_LIST)
                        if (localIP.IndexOf(ipPrefix) != -1)
                            return localIP;
                }
                return EGlobalConfig.__STRING_UNKNOWN_IP;
            }
            catch (Exception ex)
            {
                this._errorLogger.LogError(ex);
                return EGlobalConfig.__STRING_UNKNOWN_IP;
            }
        }

        /// <summary>
        /// 2019-01-07 10:34:12 ngocta2
        /// ReadAllText ko dung duoc khi read file log cua serilog provider => error process khac hold file
        /// phai dung cach nay de read file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string ReadFileNoLock(string fullPath)
        {
            try
            {
                string fileContents;
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContents = reader.ReadToEnd();
                    }
                }
                return fileContents;
            }
            catch (Exception ex)
            {
                this._errorLogger.LogError(ex);
                return "";
            }
        }
        /// <summary>
        /// 2019-01-10 13:42:13 ngocta2
        /// check null string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CheckNullString(string data)
        {
            if (data == null)
                return __NULL;
            else
                return data;
        }

        /// <summary>
        /// phai luu static de so sanh xem co trung voi so cu ko
        /// </summary>
        static long lastZScore = 0;
        /// <summary>
        /// 
        /// cach cu "yyyyMMddHHmmssfff" toan bi trung, ngay ca trong 1 ms cung van trung 4,5 row
        /// trung score gay ra insert data vao redis that bai => result = true la success nhung check redis van ko co data
        /// them 3 so sau cung la tran so kieu int64
        /// them 2 so 0 o sau cung 
        /// them 2 so thi luu redis thanh 2.0190226130143498e+18 => KHONG DUOC
        /// ===================================
        /// cach nay du xai trong 100 nam
        /// tinh dateDiff giua now va 1/1/2019 00:00:00 tinh theo milliseconds * 1000
        /// neu trung nhau thi +1 cho den khi khac nhau
        /// </summary>
        /// <returns></returns>
        public long CreateZScore()
        {
            long zScore;
            DateTime begin = new DateTime(2019, 1, 1, 0, 0, 0, 0);
            DateTime now = DateTime.Now;
            long diffInMs = Convert.ToInt64((now - begin).TotalMilliseconds);
            zScore = diffInMs * 1000;
            while (zScore <= lastZScore)
                zScore++; // +1 cho den khi khac so score truoc
            lastZScore = zScore;
            return zScore;
        }

        /// <summary>
        /// luu last zScore tung symbol
        /// </summary>
        readonly static ConcurrentDictionary<string, long> _lastZScoreDic = new ConcurrentDictionary<string, long>();
        /// <summary>
        /// 2019-03-06 09:52:32 ngocta2
        /// chart data dang co 2 loai chinh
        /// + data tung phut: score can ngan gon, de dang update row khi chi can so sanh da co row voi score cu thi delete / add row co score do
        /// + data chi tiet: score can dai de ko bi trung nhau, chi add row chu ko update
        /// ca 2 loai data tren (neu la HSX) thi can xoa sach luc start app, de tranh bi add row double
        /// 
        /// CreateZScore4Insert => tao score luon phai distinct, ko duoc trung nhau, chi dung cho insert row
        /// CreateZScore4Update => tao score ko phai distinct, duoc trung nhau, se dung cho update row => (delete row + insert row where score = score dang co)
        /// 
        /// phai su dung dic vi score rieng cua moi symbol la khac nhau
        /// 
        /// CHU Y: moi lan goi la +1 vao value , ko goi thua`
        /// CHU Y: phai dung zkey, ko the dung symbol vi sai logic, 1 symbol co nhieu zKey khac nhau
        /// </summary>
        /// <param name="zKey"> "INTRADAY:S6G__FPT" / "INTRADAYDETAIL:S6G__FPT" </param>
        /// <param name="dateTimeInput"> "2019-03-05 09:15:36.123" / "2019-03-05 09:15:36" / "2019-03-05 09:15" </param>
        /// <returns></returns>
        public long CreateZScore4Insert(string zKey, string dateTimeInput)
        {
            long STEP = 1;
            long zScore, lastZScore = 0;
            //string dateTimeString = dateTimeInput;
            // "2019-03-05 09:15:36" => #2019-03-05 09:15:36#
            string dateTimeString = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(dateTimeInput)).ToString();
            DateTime dateTime = DateTime.Parse(dateTimeString);
            //  #2019-03-05 09:15:36# => "20190305091536"
            dateTimeString = dateTime.ToString(EGlobalConfig.__DATETIME_REDIS_SCORE_YY_US); //yyyyMMddHHmmss
                                                                                            // "20190305091536" => 20190305091536
            zScore = Convert.ToInt64(dateTimeString);
            // 20190305091536 => 20190305091536000 
            zScore *= 1000;
            //if (symbol == "ACB") WriteFile(@"D:\ACB.js", $"(A) zScore={zScore}");
            // dic chua co thi add value 0, co roi thi lay ra
            if (_lastZScoreDic.ContainsKey(zKey))
                lastZScore = _lastZScoreDic[zKey];
            else
                _lastZScoreDic.TryAdd(zKey, zScore);
            //if (symbol == "ACB") WriteFile(@"D:\ACB.js", $"(B) lastZScore={lastZScore}; _lastZScoreDic[{symbol}]={_lastZScoreDic[symbol]}");
            while (zScore <= lastZScore)
                zScore += STEP; // +1 cho den khi khac so score truoc
                                //if (symbol == "ACB") WriteFile(@"D:\ACB.js", $"(C) zScore={zScore}");
                                // update dic
            _lastZScoreDic.TryUpdate(zKey, zScore, lastZScore);
            //if (symbol == "ACB") WriteFile(@"D:\ACB.js", $"(D) lastZScore={zScore}; _lastZScoreDic[{symbol}]={_lastZScoreDic[symbol]}");
            return zScore;
        }

        /// <summary>
        /// 2019-03-06 14:04:21 ngocta2
        /// dau vao chi co den phut, ko co giay
        /// ko can check trung nhau
        /// </summary>
        /// <param name="dateTimeInput"> "2019-03-05 09:15:36.123" / "2019-03-05 09:15:36" / "2019-03-05 09:15" </param>
        /// <returns></returns>
        public long CreateZScore4Update(string dateTimeInput)
        {
            return CreateZScoreByTemplate(dateTimeInput, EGlobalConfig.__DATETIME_REDIS_SCORE_YY_UM, 100000);

            //long zScore = 0;
            //string dateTimeString = dateTimeInput;
            //// "2019-03-05 09:15:36" => #2019-03-05 09:15:36#
            //DateTime dateTime = DateTime.Parse(dateTimeString);
            ////  #2019-03-05 09:15:36# => "20190305091536"
            //dateTimeString = dateTime.ToString(EGlobalConfig.DATETIME_REDIS_SCORE_UM); //yyyyMMddHHmm
            //// "201903050915" => 201903050915
            //zScore = Convert.ToInt64(dateTimeString);
            //// 201903050915 => 20190305091500000
            //zScore = zScore * 100000;
            //return zScore;
        }

        /// <summary>
        /// 2019-03-06 14:13:22 ngocta2
        /// tao score bat dau ngay
        /// ket hop score EOD va score BOD de xoa data trong ngay khi start app, tranh bi dup data
        /// </summary>
        /// <param name="dateTimeInput"> "2019-03-05 09:15:36.123" / "2019-03-05 09:15:36" / "2019-03-05 09:15" / "2019-03-05" / 20190305 </param>
        /// <returns></returns>
        public long CreateZScore4BOD(string dateTimeInput)
        {
            return CreateZScoreByTemplate(dateTimeInput, EGlobalConfig.__DATETIME_REDIS_SCORE_YY_BOD, 1000000000);
        }

        /// <summary>
        /// 2019-03-06 14:13:22 ngocta2
        /// tao score ket thuc ngay
        /// ket hop score EOD va score BOD de xoa data trong ngay khi start app, tranh bi dup data
        /// </summary>
        /// <param name="dateTimeInput"> "2019-03-05 09:15:36.123" / "2019-03-05 09:15:36" / "2019-03-05 09:15" / "2019-03-05" / 20190305 </param>
        /// <returns></returns>
        public long CreateZScore4EOD(string dateTimeInput)
        {
            return CreateZScoreByTemplate(dateTimeInput, EGlobalConfig.__DATETIME_REDIS_SCORE_YY_EOD, 1);
        }

        public long CreateZScoreByTemplate(string dateTimeInput, string dateTimeFormat, long multiply)
        {
            long zScore;
            string dateTimeString = dateTimeInput;
            // xu ly date ko co - . VD: 20190305
            if (dateTimeString.Length == 8)
                dateTimeString = dateTimeString.Insert(4, "-").Insert(7, "-");
            // "2019-03-05 09:15:36" => #2019-03-05 09:15:36#
            DateTime dateTime = DateTime.Parse(dateTimeString);
            //  #2019-03-05 09:15:36# => "20190305091536"
            dateTimeString = dateTime.ToString(dateTimeFormat);
            // "20190305" => 20190305
            zScore = Convert.ToInt64(dateTimeString);
            // 20190305 => 20190305000000000
            zScore *= multiply;
            // return
            return zScore;
        }

        /// <summary>
        /// 2019-03-08 08:53:49 ngocta2
        /// </summary>
        /// <param name="date">20190308</param>
        /// <returns>2019-03-08</returns>
        public string CreateDateString(string date)
        {
            try
            {
                string d = date.Insert(4, "-").Insert(7, "-");
                DateTime dt = DateTime.Parse(d);
                return d;
            }
            catch (Exception ex)
            {
                _errorLogger.LogError(ex);
                return "";
            }
        }

        /// <summary>
        /// 2019-03-08 10:02:45 ngocta2
        /// 2019-02-20 15:26:59 ngocta2
        /// 91932 => "091932"
        /// 101932 => "101932"
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string CreateTimeString(int time)
        {
            string timeString;
            if (time < 100000)
                timeString = $"0{time}";
            else
                timeString = time.ToString();

            string d = timeString.Insert(2, ":").Insert(5, ":");

            return d;
        }

        /// <summary>
        /// 2019-03-08 08:53:49 ngocta2
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string CreateDateTimeString(string date, string time)
        {
            return $"{CreateDateString(date)} {time}";
        }

        /// <summary>
        /// 2019-01-10 13:59:58 ngocta2
        /// check null object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public object CheckNullObject(object data)
        {
            if (data == null)
                return __NULL;
            else
                return data;
        }

        /// <summary>
        /// log sql
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public string SerializeObject(object objectToSerialize)
        {
            try
            {
                return JsonConvert.SerializeObject(objectToSerialize); // SLOW SPEED
            }
            catch
            {
                return __STRING_BLANK_OBJECT; // failed
            }
        }

        /// <summary>
        /// 2019-01-17 15:58:38 ngocta2
        /// chuyen du lieu string thanh int
        /// neu la null string thi thanh 0
        /// </summary>
        /// <param name="number">co the la null string</param>
        /// <returns></returns>
        public int ToInt(object number)
        {
            if (number == null)
                return 0;
            else
            {
                if (int.TryParse(number.ToString(), out _))
                    return Convert.ToInt32(number);
                else
                    return 0;
            }
        }

        /// <summary>
        /// 2019-01-16 13:42:39 ngocta2
        /// chuyen du lieu string thanh long
        /// neu la null string thi thanh 0
        /// </summary>
        /// <param name="number">co the la null string</param>
        /// <returns></returns>
        public long ToLong(object number)
        {
            if (number == null)
                return 0;
            else
            {
                if (long.TryParse(number.ToString(), out _))
                    return Convert.ToInt64(number);
                else
                    return 0;
            }
        }

        /// <summary>
        /// 2019-01-16 13:42:39 ngocta2
        /// chuyen string thanh double
        /// neu null thi thanh 0
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public double ToDouble(object number)
        {
            if (number == null)
                return 0;
            else
            {
                if (double.TryParse(number.ToString(), out _))
                    return Convert.ToDouble(number);
                else
                    return 0;
            }
        }

        /// <summary>
        /// 2019-03-13 15:14:02 ngocta2
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <param name="message"></param>
        /// <param name="append"></param>
        public void WriteFile(string fullFilePath, string message, bool append = true)
        {
            // kiem tra folder trong full path, neu ko co thi tao folder
            FileInfo fileInfo = new FileInfo(fullFilePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();


            using (FileStream stream = new FileStream(fullFilePath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(message);
            }
        }

        /// <summary>
        /// 20190111172524128
        /// </summary>
        /// <returns></returns>
        public string GetTimestamp()
        {
            return DateTime.Now.ToString(EGlobalConfig.__DATETIME_REDIS_SCORE);
        }

        /// <summary>
        /// 2019-03-01 08:49:10 ngocta2
        /// https://stackoverflow.com/questions/4015324/how-to-make-http-post-web-request
        /// https://serverfault.com/questions/800338/why-is-iis-allowing-only-3-connections-at-the-time
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> RequestURLAsync(string url)
        {

            // log buffer begin
            TExecutionContext ec = _debugLogger.WriteBufferBegin($"url={url}");

            try
            {
                string responseString = await _httpClient.GetStringAsync(url);
                return responseString;
            }
            catch (Exception ex)
            {
                // log error
                _errorLogger.LogErrorContext(ex, ec);
                return "";
            }
            finally
            {
                // log debug
                _debugLogger.LogDebugContext(ec, $"end");
            }
        }

        /// <summary>
        /// 2019-11-20 13:38:54 ngocta2
        /// https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        //Guid.NewGuid().ToString()
        //"9f22d404-9e9d-4d18-89d9-ee4b6ccbf8b3"
        //Guid.NewGuid().ToString("n")
        //"16b09d154b284e6194e5083c23a83aa0"
        //Guid.NewGuid().ToString("n").ToUpper()
        //"DF3F91B1A27E486AB544CFA7B312FC80"
        //Guid.NewGuid().ToString("n").ToUpper().Substring(0,4)
        //"A022"
        /// </summary>
        /// <param name="length">max 32</param>
        /// <returns></returns>
        public string GetRandomString(int length)
        {
            return Guid.NewGuid().ToString("n").ToUpper().Substring(0, length);
        }

        /// <summary>
        /// 2019-11-20 14:41:53 ngocta2
        /// https://stackoverflow.com/questions/2706500/how-do-i-generate-a-random-int-number
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int GetRandomNumberInRange(int from, int to)
        {
            Random rnd = new Random();
            return rnd.Next(from, to + 1);  // creates a number between 1 and 12

        }

        /// <summary>
        /// 2019-11-26 10:20:55 ngocta2
        /// lay thong tin ve dataset
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public string GetResultInfo(DataSet dataSet)
        {
            int tableCount = 0;
            string infoString;
            if (dataSet == null)
                infoString = "dataSet=null";
            else
            {
                infoString = $"dataSet.Tables.Count={dataSet.Tables.Count}; ";
                foreach (DataTable dataTable in dataSet.Tables)
                    infoString += $"dataTable[{tableCount++}].Rows.Count={dataTable.Rows.Count}; ";
            }

            return infoString;
        }

        /// <summary>
        /// 2019-11-26 10:20:55 ngocta2
        /// lay thong tin ve cac row da bi anh huong bi sql
        /// </summary>
        /// <param name="affectedRowCount"></param>
        /// <returns></returns>
        public string GetResultInfo(int affectedRowCount)
        {
            return $"affectedRowCount={affectedRowCount}";
        }

        /// <summary>
        /// doi HOSE => HOSTC
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public string GetExchange(string strExchange)
        {
            switch (strExchange)
            {
                case "HOSE": return "HOSTC"; // "HOSTC";
                case "HNX": return "HASTC"; // "HASTC";
                case "UPCOM": return "HASTC"; // "VPOTC"; - UP_ORE
                case "HNX.NY": return "HASTC"; // "HNX.NY";
                case "HNX.UPCOM": return "HASTC"; // "HNX.UPCOM";
            }

            if (strExchange == "HOSTC" || strExchange == "HASTC" || strExchange == "VPOTC")
                return strExchange;

            //return "";            // 3v2 dung nhu nay ok (2017-09-25 10:32:44 ngocta2)
            return strExchange;     // 3v3 thi input = HSX thi return "" la ko dc, phai dung cach nay  (2017-09-25 10:32:44 ngocta2)
        }

        // build js code => hien thi js alert (them cap the script)
        public string JsCode(string strJS)
        {
            return TEMPLATE_JS_CODE.Replace("<!JsCode>", strJS);
        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());
            return string.Join("&", properties.ToArray());
        }

        /*
        Based on Eric J's answer here: https://stackoverflow.com/a/1344255/72350
        - Fixed bias by using 64 characters
        - Removed unneeded 'new char[62]' and 'new byte[1]'
        - Using GetBytes instead of GetNonZeroBytes
        */
        public static string GetUniqueKey(int length)
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
