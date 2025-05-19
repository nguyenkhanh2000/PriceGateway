using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Interfaces;

namespace CommonLib.Interfaces
{
    public interface IDebugLogger : ISerilogProvider, ILogDebug, IInstance
    {
    }
}
