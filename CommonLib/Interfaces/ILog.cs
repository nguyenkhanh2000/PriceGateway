using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Interfaces
{
    /// <summary>
    /// created class
    /// them thread, task
    /// </summary>
    public interface ILog
    {
        /// <summary>
		/// prop: full path cua file log
		/// D:\WebLog\S6G\CommonLib.Tests\SQL\20190926.js
		/// </summary>
		string FullLogPath { get; }

        /// <summary>
        /// D:\WebLog\S6G\CommonLib.Tests
        /// </summary>
        string LogRootPath { get; }

        /// <summary>
        /// D:\WebLog\S6G\CommonLib.Tests\DEBUG
        /// path cua log folder (4 loai log)
        /// </summary>
        string LogDirPath { get; }


        /// <summary>
        /// hiendv
        /// new log
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        void LogToFile(string fileName, string data);
    }
}
