using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class ResourceGroupGateway : ArmResourceGateway, IResourceGroupGateway
    {
        public ResourceGroupGateway(IOathGateway oAuthGateway)
            :base(oAuthGateway)
        {

        }

        public Task<string> GetResourceGroupData(Uri websiteUri)
        {
            return this.GetResourceAsString(websiteUri);
        }

        //public IOathGateway OAuthGateway
        //{
        //    get
        //    {
        //        return this.OAuthGateway;
        //    }
        //    set
        //    {
        //        this.OAuthGateway = value;
        //    }
        //}

        public Task<string> GetWebSitesData(Uri webSitesUri)
        {
            return this.GetResourceAsString(webSitesUri);
        }
    }
}
