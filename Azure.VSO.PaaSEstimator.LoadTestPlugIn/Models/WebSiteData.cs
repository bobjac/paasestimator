using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class WebSiteData
    {
        public Uri WebSiteUri { get; set; }
        public string SiteName { get; set; }
        public string Location { get; set; }
        public List<string> Instances { get; set; }
        public int InstanceCount { get; set; }
        public string ServerFarm { get; set; }
        public double Rate { get; set; }
        public string RatePeriod { get; set; }
    }
}
