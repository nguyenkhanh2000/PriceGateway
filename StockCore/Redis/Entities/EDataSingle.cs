using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis.Entities
{
    public class EDataSingle : EBaseValue
    {
        /// <summary>
        /// noi dung data chinh can luu redis
        /// </summary>
        [JsonProperty(Order = 2)]
        public object Data { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public EDataSingle(object data) : base()
        {
            Data = data;
        }
    }
}
