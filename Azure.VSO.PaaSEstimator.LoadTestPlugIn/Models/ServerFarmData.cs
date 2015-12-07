using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class ServerFarmData
    {
        public string ServerFarmName { get; set; }
        public string Location { get; set; }
        public string Capacity { get; set; }
        public string Family { get; set; }
        public string Size { get; set; }
        public string Tier { get; set; }
    }
}
