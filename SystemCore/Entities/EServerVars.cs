using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCore.Entities
{
    /// <summary>
	/// luu cac info thuoc ServerVariables
	/// HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString()
	/// </summary>
    public class EServerVars
    {
        public string RemoteAddr { get; set; } // ClientIp 
        public string LocalAddr { get; set; } // ServerIp 
        public string HttpUserAgent { get; set; } // HTTP_USER_AGENT
        public string RedisAddr { get; set; }  // Redis Ip 
        public string RedisDB { get; set; }  // Redis DB
        public int MaxSymbol { get; set; }  // Redis DB
    }
}
