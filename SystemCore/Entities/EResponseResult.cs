using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCore.Entities
{
    /// <summary>
	/// DTO return tu WebApi (GUI)
	/// </summary>
    public class EResponseResult
    {
        public const string RESPONSE_CONTENT_TYPE_JSON = "application/json";
        public const long RESPONSE_CODE_SUCCESS = 0;
        public const string RESPONSE_MSG_SUCCESS = "SUCCESS";
        public const long RESPONSE_CODE_INIT = -1;
        public const string RESPONSE_MSG_INIT = "INIT";
        public const long RESPONSE_CODE_ACCESS_DENIED = -123456;
        public const string RESPONSE_MSG_ACCESS_DENIED = "ACCESS_DENIED";
        public const string RESPONSE_DATA_NULL = null;
        public const long RESPONSE_CODE_INVALID_TRADING_PASSWORD = -10;
        public const long RESPONSE_CODE_INVALID_DATA = -20;

        /// <summary>
        /// code return tu sp hoac system neu co exception
        /// </summary>
        public long Code { get; set; }

        /// <summary>
        /// message return tu sp hoac error msg neu co exception
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// data khong xac dinh type
        /// </summary>
        public object Data { get; set; }
    }
}
