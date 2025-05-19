using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Interfaces;

namespace CommonLib.Interfaces
{
    public interface ICommon : IInstance
    {
        // string
        string Left(string input, int leftCount);
        string Right(string input, int rightCount);
        string Mid(string input, int begin, int count);

        // redis
        long CreateZScore();
        string CreateDateString(string date);
        string CreateTimeString(int time);
        string CreateDateTimeString(string date, string time);
        long CreateZScore4Insert(string symbol, string dateTimeInput);
        long CreateZScore4Update(string dateTimeInput);
        long CreateZScore4BOD(string dateTimeInput);
        long CreateZScore4EOD(string dateTimeInput);

        // monitor
        string GetLocalDateTime();
        string GetLocalIp();

        // utils
        string ReadFileNoLock(string fullPath);
        string SerializeObject(object objectToSerialize);
        string CheckNullString(string data);
        object CheckNullObject(object data);
        int ToInt(object number);
        long ToLong(object number);
        double ToDouble(object number);
        void WriteFile(string fullFilePath, string message, bool append = true);
        string GetTimestamp();
        Task<string> RequestURLAsync(string url);
        string GetRandomString(int length);
        int GetRandomNumberInRange(int from, int to);

        string GetResultInfo(DataSet dataSet);
        string GetResultInfo(int affectedRowCount);
        string GetExchange(string strExchange);
        string JsCode(string strJS);

        /// <summary>
        /// Serialize object to query string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string GetQueryString(object obj);
    }
}
