using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public interface IRateCardGateway
    {
        string GetRateCardData(Uri rateCardUri);
    }
}
