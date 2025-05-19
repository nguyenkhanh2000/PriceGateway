using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Entities;

namespace StockCore.Redis.Entities
{
    public abstract class EBaseValue
    {
        /// <summary>
        /// thoi gian update data vao redis (auto)
        /// </summary>
        [JsonProperty(Order = 1)]
        public string Time { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public EBaseValue()
        {
            this.Time = DateTime.Now.ToString(EGlobalConfig.__DATETIME_REDIS_VALUE);
        }
    }
}
