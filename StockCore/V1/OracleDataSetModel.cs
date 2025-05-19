using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.V1
{
    // FutureV1Models.ResponseModel.Data la 1 dataset, can tach nho hon
    // moi table la 1 list cac obj generic type
    public class OracleDataSetModel<T>
    {
        public List<T> Table { get; set; }   // table 0
        public List<T> Table1 { get; set; }  // table 1
        public List<T> Table2 { get; set; }  // table 2
        public List<T> Table3 { get; set; }  // table 3
        public List<T> Table4 { get; set; }  // table 4 
    }
}
