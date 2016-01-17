using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class ResourceCostEstimate
    {
        public int ID { get; set; }
        public string ResourceId { get; set; }
        public double Rate { get; set; }
        public int Instances { get; set; }
        public double CostEstimate { get; set; }
        public string RatePeriod { get; set; }
        [Timestamp]
        public byte[] TimeStamp { get; set; }
    }
}
