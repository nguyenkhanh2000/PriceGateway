using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Temporaries;

namespace CommonLib.Interfaces
{
    /// <summary>
    /// + Log SQL   => folder SQL
    ///     log tat ca sql update vao SQL Server, Oracle, Redis
    /// </summary>
    public interface ILogSql : ILog
    {

        /// <summary>
        /// ghi chi tiet log SQL(SQL+NoSQL) vao file (async)
        /// ghi log sql TRUOC + SAU khi exec sp/script vao db
        /// </summary>
        /// <param name="data"></param>
        void LogSql(string data);

        /// <summary>
        /// ghi chi tiet log SQL(SQL+NoSQL) vao file (async)
        /// ghi log sql TRUOC + SAU khi exec sp/script vao db
        /// ghi log theo số tài khoản khách hàng
        /// </summary>
        /// <param name="data"></param>
        void LogSql(string Stk, string data);


        /// <summary>
        /// log sql co eid de chinh xac hon voi code multi-thread
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="data"></param>
        void LogSqlContext(TExecutionContext executionContext, string data);

        void LogSqlContext2(string Stk, TExecutionContext executionContext, string data);

        void LogSqlIPServer(string Stk, string data);


        /// <summary>
        /// 2019-09-26 09:21:35 ngocta2
        /// log data chia nho
        /// D:\WebLog\S6G\TAChartSaverApp\SQL\20190926\REDIS-ABT.js
        /// D:\WebLog\S6G\TAChartSaverApp\SQL\20190926\REDIS-ACB.js
        /// D:\WebLog\S6G\TAChartSaverApp\SQL\20190926\REDIS-ABI.js
        /// D:\WebLog\S6G\TAChartSaverApp\SQL\20190926\REDIS-VN30F1M.js
        /// </summary>
        /// <param name="executionContext">co the truyen null thi log ra file ko can thong tin ec</param>
        /// <param name="fileName">REDIS-ABT.js</param>
        /// <param name="data">2019-09-20 08:45:09.792 +07:00 [INF] =================...</param>
        void LogSqlSub(TExecutionContext ec, string fileName, string data);

        /// <summary>
        /// hiendv new log
        /// </summary>
        /// <param name="ec"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
		void LogToFile(TExecutionContext ec, string fileName, string data);
    }
}
