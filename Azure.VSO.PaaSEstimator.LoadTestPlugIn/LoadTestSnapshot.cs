using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn
{
    public class LoadTestSnapShot : TableEntity
    {
        public LoadTestSnapShot()
            :base()
        {
            this.Timestamp = DateTime.Now.ToLocalTime();
        }

        public LoadTestSnapShot(Guid loadTestRun)
            : base(partitionKey: loadTestRun.ToString(), rowKey: Guid.NewGuid().ToString())
        {
            this.Timestamp = DateTime.Now.ToLocalTime();
        }

        public string LoadTestName { get; set; }
        public string ResourceId { get; set; }
        public string ResourceType { get; set; }
        public string EventMessage { get; set; }
        public string InstanceState { get; set; }
    }
}
