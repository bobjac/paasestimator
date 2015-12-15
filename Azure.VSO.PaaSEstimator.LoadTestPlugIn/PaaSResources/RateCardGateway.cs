using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class RateCardGateway : ArmResourceGateway, IRateCardGateway
    {
        public RateCardGateway(IOathGateway oAuthGateway)
            :base(oAuthGateway)
        {

        }

        public string GetRateCardData(Uri rateCardUri)
        {
            return GetResourceAsString(rateCardUri).Result;
        }
    }
}
