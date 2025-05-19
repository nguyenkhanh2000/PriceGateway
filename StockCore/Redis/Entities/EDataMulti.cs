using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCore.Redis.Entities
{
    public class EDataMulti : EBaseValue
    {
        /// <summary>
        /// noi dung data chinh can luu redis
        /// </summary>
        [JsonProperty(Order = 2)]
        public IList<object> Data { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public EDataMulti(IList<object> data) : base()
        {
            Data = data;
        }
    }
}
