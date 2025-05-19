using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Temporaries;

namespace CommonLib.Interfaces
{
    /// <summary>
    /// + Log Error => folder ERROR
    ///     log tat ca error details
    /// </summary>
    public interface ILogError : ILog
    {
        /// <summary>
        /// ghi chi tiet log error vao file (async)
        /// </summary>
        /// <param name="ex"></param>
        void LogError(Exception ex);

        void LogErrorLogReader(Exception ex);

        /// <summary>
        /// co du data de debug
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="executionContext"></param>
        void LogErrorContext(Exception ex, TExecutionContext executionContext);

        /// <summary>
        /// 2022-05-16 LinhNH
        /// Lưu log error theo tên file là số tài khoản khách hàng
        /// </summary>
        /// <param name="Stk">Số tài khoản KH</param>
        /// <param name="ex"></param>
        /// <param name="executionContext"></param>
        void LogErrorContext(string Stk, Exception ex, TExecutionContext executionContext);

        /// <summary>
        /// 2022-11-24 NgocNT3
        /// Lưu log error theo tên file là số tài khoản khách hàng + IPServer webapp
        /// </summary>
        /// <param name="Stk">Số tài khoản KH</param>
        /// <param name="ex"></param>
        /// <param name="executionContext"></param>
        void LogErrorContextIP(string Stk, Exception ex, TExecutionContext executionContext);
    }
}
