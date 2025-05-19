using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Temporaries;

namespace CommonLib.Interfaces
{
	public interface ILogInfo
    {
        /// <summary>
        /// khi log info de biet app dang chay den dau
        /// </summary>
        /// <param name="data"></param>
        void LogInfo(string data);


        /// <summary>
        /// log info, log ngay ma ko can quan tam debug flag
        /// </summary>
        /// <param name="executionContext"></param>
        /// <param name="data"></param>
        void LogInfoContext(TExecutionContext executionContext, string data);
    }
}
