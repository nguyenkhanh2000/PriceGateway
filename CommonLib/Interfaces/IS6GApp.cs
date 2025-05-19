using Microsoft.Extensions.Configuration;
using MonitorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SystemCore.Interfaces;

namespace CommonLib.Interfaces
{
    /// <summary>
    /// 3 instance logger ghi log Error + SQL + Debug
    /// 1 instance monitorClient send data cho server
    /// tat ca app thuoc Host group deu co 1 instance cua IS6GApp
    /// </summary>
    public interface IS6GApp : IInstance
    {
        IErrorLogger ErrorLogger { get; }
        ISqlLogger SqlLogger { get; }
        IInfoLogger InfoLogger { get; }
        IDebugLogger DebugLogger { get; }
        ICommon Common { get; }
        IConfiguration Configuration { get; }
        IMonitor Monitor { get; }
    }
}
