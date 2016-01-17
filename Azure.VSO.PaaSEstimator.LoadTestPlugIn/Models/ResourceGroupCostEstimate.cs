using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class ResourceGroupCostEstimate
    {
        public int ID { get; set; }
        public Guid LoadTestId { get; set; }
        public double TotalMonthlyEstimate { get; set; }
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public ResourceGroupCostEstimate()
        {
            this.ResourceEstimates = new List<ResourceCostEstimate>();
        }

        //public List<ResourceCostEstimate> ResourceEstimates = new List<ResourceCostEstimate>(); 
        public ICollection<ResourceCostEstimate> ResourceEstimates { get; set; }
    }
}
