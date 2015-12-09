using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class WebSiteInstancesGateway : ArmResourceGateway, IWebSiteInstancesGateway
    {
        public WebSiteInstancesGateway(IOathGateway oAuthGateway)
            :base(oAuthGateway)
        {

        }

        public Task<string> GetWebSiteInstancesData(Uri webSiteInstancesUri)
        {
            return GetResourceAsString(webSiteInstancesUri);
        }
    }
}
