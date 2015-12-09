using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class WebSiteInstancesPayload
    {
        public List<WebSiteInstance> value { get; set; }
        public string nextLink { get; set; }
    }
}
