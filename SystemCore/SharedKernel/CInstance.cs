using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemCore.Entities;
using SystemCore.Interfaces;

namespace SystemCore.SharedKernel
{
    public abstract class CInstance : IInstance
    {
        // private vars 
        private string _instanceId;
        private string _createdDateTime;

        // public props 
        public string InstanceId { get { return this._instanceId; } }

        public string CreatedDateTime { get { return this._createdDateTime; } }

        /// <summary>
        /// constructor
        /// </summary>
        public CInstance()
        {
            this._instanceId = Guid.NewGuid().ToString();
            this._createdDateTime = DateTime.Now.ToString(EGlobalConfig.__DATETIME_LOG);
        }
    }
}
