﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class WebSiteGateway : ArmResourceGateway, IWebSiteGateway
    {
        public WebSiteGateway(IOathGateway oAuthGateway)
            :base(oAuthGateway)
        {

        }

        public Task<string> GetWebSiteData(Uri websiteUri)
        {
            return GetResourceAsString(websiteUri);
        }
    }
}
