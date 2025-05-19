using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemCore.Entities
{
    static public class EGlobalConfig
    {
        private const string __DATETIME_FORMAT_1 = "yyyy-MM-dd HH:mm:ss.fff"; // chi tiet den ca ms
        private const string __DATETIME_FORMAT_2 = "yyyyMMddHHmmssfff";       // 5G
        private const string __DATETIME_FORMAT_3 = "yyyy-MM-dd";              // chi ngay thang nam
        private const string __DATETIME_FORMAT_4 = "yyyyMMddHHmmss";          // bo qua millisecond
        private const string __DATETIME_FORMAT_5 = "yyyyMMddHHmm";            // bo qua second
        private const string __DATETIME_FORMAT_6 = "yyyyMMdd";                // bo qua hour : tao score bat dau ngay
        private const string __DATETIME_FORMAT_7 = "yyyyMMdd235959999";       // hour fixed : tao score ket thuc ngay
        private const string __DATETIME_FORMAT_14 = "yyMMddHHmmss";            // bo qua millisecond
        private const string __DATETIME_FORMAT_15 = "yyMMddHHmm";              // bo qua second
        private const string __DATETIME_FORMAT_16 = "yyMMdd";                  // bo qua hour : tao score bat dau ngay
        private const string __DATETIME_FORMAT_17 = "yyMMdd235959999";         // hour fixed : tao score ket thuc ngay
        private const string __DATETIME_FORMAT_21 = "HHmmss";                  // chi lay gio, phut, giay
        private const string __DATETIME_FORMAT_22 = "HHmm";                    // chi lay gio, phut
        private const string __DATETIME_FORMAT_23 = "HH";                      // chi lay gio


        public const string __LEADING_ZERO_THREAD_ID = "0000";
        public const string __LEADING_ZERO_TASK_ID = "000000";
        public const string __DATETIME_REDIS_SCORE = __DATETIME_FORMAT_2;
        public const string __DATETIME_REDIS_VALUE = __DATETIME_FORMAT_1;
        public const string __DATETIME_MONITOR = __DATETIME_FORMAT_1;
        public const string __DATETIME_LOG = __DATETIME_FORMAT_1;
        //public const string __DATETIME_LOG_FILENAME_S     = DATETIME_FORMAT_3;
        //public const string __DATETIME_REDIS_SCORE_US     = DATETIME_FORMAT_4;
        //public const string __DATETIME_REDIS_SCORE_UM     = DATETIME_FORMAT_5;
        public const string __DATETIME_YYYYMMDDHHMMSS = __DATETIME_FORMAT_4;
        public const string __DATETIME_YYYYMMDD = __DATETIME_FORMAT_6;
        //public const string __DATETIME_REDIS_SCORE_EOD    = DATETIME_FORMAT_7;
        public const string __DATETIME_REDIS_SCORE_YY_US = __DATETIME_FORMAT_14;
        public const string __DATETIME_REDIS_SCORE_YY_UM = __DATETIME_FORMAT_15;
        public const string __DATETIME_REDIS_SCORE_YY_BOD = __DATETIME_FORMAT_16;
        public const string __DATETIME_REDIS_SCORE_YY_EOD = __DATETIME_FORMAT_17;
        //public const string __DATETIME_HHMMSS				= DATETIME_FORMAT_21;
        //public const string __DATETIME_HHMM				= DATETIME_FORMAT_22;
        public const string __DATETIME_HH = __DATETIME_FORMAT_23;


        // neu them 1 dai ip moi thi 
        // 1. them vao predefine_ip_1
        // 2. them vao predefine_ip_2
        // 3. build lai SystemCore.dll
        // 4. update lai tat ca cho co lien quan SystemCore.dll
        // predefine_ip_1		
        private const string PREFIX_IP_LAN_FOX = "172.16.0.";
        private const string PREFIX_IP_LAN_HSX = "10.26.248.";
        private const string PREFIX_IP_LAN_HNX = "10.26.100.";
        private const string PREFIX_IP_LAN_FPTS = "10.26.2.";
        private const string PREFIX_IP_LAN_FPTS_4 = "10.26.4."; // 2018-08-13 16:49:57 Hungpv
        private const string PREFIX_IP_LAN_FPTS_BLAZE = "10.26.5."; // 2018-07-06 08:07:57 ngocta2
        // predefine_ip_2
        public static string[] __PREFIX_IP_LIST = new string[]{
            PREFIX_IP_LAN_FOX,
            PREFIX_IP_LAN_HSX,
            PREFIX_IP_LAN_HNX,
            PREFIX_IP_LAN_FPTS,
            PREFIX_IP_LAN_FPTS_4,
            PREFIX_IP_LAN_FPTS_BLAZE
        };

        static public string DateTimeNow => DateTime.Now.ToString(__DATETIME_MONITOR);

        public const string __DATA_NULL = null;

        public const string __STRING_UNKNOWN_IP = "UnknownIp";
        public const string __STRING_BEFORE = "BEFORE: ";
        public const string __STRING_AFTER = "AFTER: ";
        public const string __STRING_BLANK = "";
        public const string __STRING_RETURN_NEW_LINE = "\r\n";
        public const string __STRING_UNKNOWN = "unknown";
        public const string __STRING_NULL = "null";
        public const string __STRING_0 = "0";
        public const string __STRING_1 = "1";
        public const string __STRING_APPLICATION_JSON = "application/json";
        public const string __STRING_ACCESS_DENIED = "ACCESS_DENIED";
        public const string __STRING_SERVER_VARS_HTTP_USER_AGENT = "HTTP_USER_AGENT";
        public const string __STRING_SERVER_VARS_REMOTE_ADDR = "REMOTE_ADDR";
        public const string __STRING_SERVER_VARS_LOCAL_ADDR = "LOCAL_ADDR";
        public const string __STRING_HEADER_USER_AGENT = "User-Agent";
        public const string __STRING_SUCCESS = "SUCCESS";
        public const string __STRING_FAILED = "FAILED";
        public const string __STRING_ERROR_ACCOUNT = "058C123456xxx";
        public const string __STRING_ERROR_PASSWORD = "fpts12345xxx";
        public const string __STRING_RANDOM_SESSIONNO = "058C20029020200220155602093211xxx";
        public const string __SQL_BUYSELLINDICATOR_BUY = "BUY";
        public const string __SQL_BUYSELLINDICATOR_SELL = "SELL";
        public const string __SQL_BUYSELLINDICATOR_B = "B";
        public const string __SQL_BUYSELLINDICATOR_S = "S";

        /// <summary>
        /// 5/18/2020 3:22:13 PM trungnt4
        /// </summary>
        public const string VD_SERVER_DATA = "ServerData";
        public const string VD_CURRENT_VIEW = "CurrentView";
        public const string RP_PENDINGSETTLEMENT_BUY = "BUY";
        public const string RP_PENDINGSETTLEMENT_SELL = "SELL";
        public const string RP_PENDINGSETTLEMENT_B = "B";
        public const string RP_PENDINGSETTLEMENT_S = "S";


        /// <summary>
        /// 2020-03-04 15:56:03 ngocta2
        /// [Collection("Sequential")]
        //public class UnitTest_CV1AccountHanlderA
        // method A1()
        // method A2()
        //[Collection("Sequential")]
        //public class UnitTest_CV1AccountHanlderB
        // method B1()
        // method B2()
        // ... cac A1,A2,B1,B2 se run theo thu tu lan luot, 
        // vi run tat ca 1 luc se gay error (async)
        /// </summary>
        public const string __UNIT_TEST_RUN_SEQUENTIAL = "Sequential";

        public const long __CODE_SUCCESS = 0;
        public const string __RESPONSE_MSG_SUCCESS = "SUCCESS";
        public const long __CODE_FAIL = -1;
        public const long __CODE_CHECK_SUCCESS = 1;
        public const long __CODE_CHECK_FAIL = 0;
        public const long __CODE_ERROR_IN_LAYER_DAL = -9998;
        public const long __CODE_ERROR_IN_LAYER_BLL = -9997;
        public const long __CODE_ERROR_IN_LAYER_GUI = -9996;        // controller : tai day thuong return HttpStatusCode.InternalServerError (500)
        public const long __CODE_ERROR_IN_LAYER_UNKOWN = -9995;     // unknown (co the ko bao gio vao duoc day)
        public const long __CODE_ACCESS_DENIED = -123456;       // controller		
        public const long __CODE_ERROR_ODDLOT_ADDORDER = -11044;        // Ban chua dang ky su dung dich vu nay! (You have not registered this service!)	
        public const long __CODE_ERROR_ODDLOT_CANCELORDER = -35002;
        public const long __CODE_ERROR_STOCK_GATEWAY = -9994;

        // db
        public const string DB_CODE_CHECK_SESSION_FAILED = "-1";
        public const int CODE_CHECK_DB_PASSWORD_SUCCESS = 1;
        public const int CODE_CHECK_TRADING_PASSWORD_SUCCESS = 1;
        public const int CODE_CHECK_TRADING_PASSWORD_FAILED = 0;
        public const long DB_ERROR_CODE_SUCCESS = 0;

        // Error OTP
        public const int ERROR_OTP_SESSION_NOT_FOUND = -123456;
        public const int ERROR_OTP_OVER_RETRYTIMES_MAX = 181104;
        public const int ERROR_OTP_ERROR_OR_EXPIRE_OTP = 181105;
        public const int ERROR_OTP_CREATE_NULL = 181109;
        public const int OTP_VERYFIRE_SUCESS = 0;

        public const int ERROR_OTP_TIME_EXPIRED = 181107;
        public const int ERROR_OTP_OVER_RESENDTIMES_MAX = 181106;
        public const int ERROR_OTP_SEND_FAILED = 181111;
        public const int OTP_SEND_SUCESS = 0;

        public enum CODE_CHECK_SESSION
        {
            FAILED = 0,
            SUCCESS = 1
        }
        public enum CODE_LOGIN_STATUS
        {
            OK = 0,             // login thanh cong
            FIRST_LOGIN = 1     // login loi, lan dau login, bi ep doi mat khau
        }
        public enum CODE_TRADING_PASSWORD_TYPE
        {
            USE_DB = 0,         // password tinh
            USE_TOKEN = 1       // password pass code (RSA)
        }

        public enum CODE_RSA_RESULT
        {
            CODE_SUCCESS = 0,
            CODE_FAILED_ACCESS_DENIED = 1,
            CODE_FAILED_NEXT_PASSCODE_REQUIRED = 2,
            CODE_FAILED_NEW_PIN_REQUIRED = 5,
            CODE_FAILED_PIN_ACCEPTED = 6,
            CODE_FAILED_PIN_REJECTED = 7
        }

        public enum CODE_ACCOUNT_LOGIN
        {
            FAILED = 1010101, // Invalid loginName..login fail try again
            SUCCESS = 0
        }

        public enum CODE_CHECK_TRD_PWD
        {
            FAILED = 0,
            SUCCESS = 1
        }

        public enum CODE_AUTH_CHANGE_CHECKPASS2
        {
            FAILED = 1030303, // Session Expired ..login again and try	
            SUCCESS = 0
        }

        public enum CODE_AUTH_SAVE_MAC
        {
            FAILED = -1,
            SUCCESS = 1
        }

        public enum CODE_CHANGE_PASSWORD
        {
            FAILED = 1030303, // Incorect Accounts number
            SUCCESS = 1030320 // Password has been changed successul.
        }

        public enum CODE_CHANGE_PASSWORD1ST
        {
            FAILED = 1030302, // Please enter the correct Old Password
            SUCCESS = 1030319 // Login pass change successfull.Welcome to FPTS Accounts!
        }
        public enum CODE_CHECKCAPTCHA
        {
            FAILED = -123456,
            SUCCESS = 1
        }


        /// <summary>
        /// /// VB function : Right
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        /// <summary>
        /// 5/18/2020 3:22:13 PM trungnt4
        /// Global config de su dung chinh xac js cua tung view
        /// </summary>
        public enum ALL_VIEW
        {
            REPORT_PENDING_SETTLEMENT = 1001,
            REPORT_PENDING_SETTLEMENT_DETAIL = 1002,
            REPORT_CLIENT_ACTIVITY_RANGE = 1003,
            REPORT_TRADE_LOG = 1004,
            REPORT_CLIENT_ADV_LIST = 1005,
            REPORT_STOCK_SETTLEMENT = 1006,
            REPORT_CASH_SETTLEMENT = 1007,
            REPORT_STOCK_DETAILS = 1008,
            REPORT_CURR_MARGIN = 1009,
            REPORT_DETAIL_CURR_MARGIN = 1010,
            REPORT_ASSET_2 = 1011,
            REPORT_CW = 1012,
            REPORT_LIST_FEE = 1013,
            ODDLOT_ORDER_FORM = 1014,
            ODDLOT_HISTORY = 1015,
            REPORT_CLIENT_ADV_HIS = 1016,
            STOPLOSS_ORDERFORM = 1017,
            STOPLOSS_HISTORY = 1018,
            REPORT_TRANS_BALANCE = 1019,
            REPORT_TRANS_SUMMARY = 1020,
            REPORT_PROFIT_LOSS = 1021,
            REPORT_NAV = 1022,
            REPORT_ASSET = 1023

        }

        // ---------------- EXPORT EXCEL/PDF ---------------- 
        public const string PATH_FONT_ARIAL = "c:/windows/fonts/arial.ttf";
        public const string CONTENTTYPE_EXCEL = "application/ms-excel";
        public const string CONTENTTYPE_PDF = "application/pdf";
        public const string CHARSET_UTF8 = "UTF-8";
        public const string EXPORT_HEADER_DISPOSITION = "content-disposition";
        public const string TEMPLATE_HEADER_EXPORT_EXCEL = "attachment; filename=(LoginName)_(NameTab).xls";
        public const string TEMPLATE_FILENAME_PDF = "(LoginName)_(NameTab).pdf";
        // ---------------- /EXPORT EXCEL TRADELOG/PDF --------------------------------------------------------------------------------------------------------
        public const string VIEW_EXPORT_EXCEL_REPORT_TRADELOG = "Export_Excel_Report_Tradelog";
        public const string TEMPLATE_HEADER_EXPORT_EXCEL_REPORT_TRADELOG = "attachment; filename=(LoginName)_ReportTradelog.xls";
        public const string TEMPLATE_FILENAME_PDF_REPORT_TRADELOG = "(LoginName)_LichSuKhopLenh.pdf";


        // HienDV log
        public const string SESSION_TOKEN = "aspfpt_sessiontoken";
        public const string SESSION_CLIENT_CODE = "client_code";
    }
}
