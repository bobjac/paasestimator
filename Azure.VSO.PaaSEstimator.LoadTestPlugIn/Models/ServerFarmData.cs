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

        public string GetMeterName()
        {
            string meterName = string.Empty;

            StringBuilder sb = new StringBuilder();

            if (string.Compare(Family, "S") == 0)
            {
                sb.Append("Standard ");
            }

            if (string.Compare(Family, "P") == 0)
            {
                sb.Append("Premium ");
            }

            int capacity = int.Parse(Capacity);
            if (capacity == 1)
            {
                sb.Append("Small ");
            }
            else if (capacity == 2)
            {
                sb.Append("Medium ");
            }
            else
            {
                sb.Append("Large ");
            }

            sb.Append("App Service Hours");

            return sb.ToString();
        }
    }
}
