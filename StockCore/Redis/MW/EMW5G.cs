using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis.MW
{
    public class EMW5G
    {
        public const string __TABLE_WL = "WLTable";
        public const string __FIELD_ID = "ID";
        public const string __FIELD_TEMPLATE_NAME = "template_name";
        public const string __FIELD_TICKERS_LIST = "tickers_list";
        public const string __FIELD_DEFAULT_MARKETWATCH = "default_MarketWatch";

        public string Username { get; set; }          // so tk (Clientcode)
        public string TemplateName { get; set; }          // tên danh mục 
        public string CodeList { get; set; }          // danh sách mã
        public string TemplateID { get; set; }          // Id mã
        public string MobileUserAgent { get; set; }          //  User Agent client
        public string Row { get; set; }          //  số lượng mã ưu tiên
        public string action { get; set; }
        public string name { get; set; }

        public int ErrorCode { get; set; }          // Code loi  
        public string ErrorMess { get; set; }          // Thong bao loi
    }
    public class MWModel_1
    {
        public string Score { get; set; }

        public string Name { get; set; }

        public string List { get; set; }

        public string Row { get; set; }

        public string Default_MarketWatch { get; set; }

        public string UserAgent { get; set; }

    }

    public class DataClientMW
    {
        public long Score { get; set; }
        public string ClientCode { get; set; }
        public string Symbols { get; set; }
        public int Position { get; set; }
    }

    public class DataMW
    {
        public DataMW()
        {
            Rows = new List<MWModel_1>();
        }
        public List<MWModel_1> Rows { get; set; }      // [] tương tự Data 

        public MWModel Data { get; set; }
        public string ClientCode { get; set; }
        public long Score { get; set; }
    }

    public class MWModel
    {
        public string Name { get; set; }

        public string List { get; set; }

        public string Row { get; set; }

    }
    public class ZSET_VALUE // {"Name":"Ngân hàng","List":"ACB,STB"}
    {
        public string Name { get; set; }    // Ngân hàng
        public string List { get; set; }    // ACB,STB
        public string Row { get; set; }    // ACB,STB
    }

    public class ConnectRedisMWS5GModel
    {
        public string Exchange { get; set; }          //  
        public string key { get; set; }          //  
        public string connectString { get; set; }          //  
        public int DB { get; set; }          // 
                                             // 
    }

    public class ListS5GModel
    {
        public string Time { get; set; }
        public List<Object> Data { get; set; }
    }

    public class HNXDS
    {
        public string Time { get; set; }
        public List<Object> Data { get; set; }
    }
}
