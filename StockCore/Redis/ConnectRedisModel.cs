using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis
{
    public class ConnectRedisModel
    {
        public string ClientCode { get; set; }          //  
        public string key { get; set; }          //  
        public string connectString { get; set; }          //  
        public int DB { get; set; }          // 
        public string fromScore { get; set; }          // 
        public string toScore { get; set; }          // 
        public string reportType { get; set; }          // 
    }
}
