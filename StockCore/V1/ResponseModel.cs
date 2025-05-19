using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.V1
{
    public class ResponseModel
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

        public long Code { get; set; }    // error code tra ra tu sp
        public string Message { get; set; }    // error message tra ra tu sp => khong phai dung msg nay de show ra cho end-user xem, msg nay chi giup cho debug
        public object Data { get; set; }    // data object co the la DataSet hoac DataTable hoac NULL .... tuy vao context

        // 2018-05-21 10:44:22 ngocta2
        // khi tao instance, default gan cho cac gia tri sau
        public ResponseModel()
        {
            Code = RESPONSE_CODE_INIT;
            Message = RESPONSE_MSG_INIT;
            Data = RESPONSE_DATA_NULL;
        }

        // DB Response
        public const long DB_ERROR_CODE_SUCCESS = 0;
    }
}
