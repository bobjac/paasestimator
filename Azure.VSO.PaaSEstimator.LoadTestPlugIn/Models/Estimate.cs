using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models
{
    public class Estimate
    {
        public int Id { get; set; }
        public string LoadTestRun { get; set; }
        public double MonthlyEstimate { get; set; }

        //public virtual ICollection
    }
}
