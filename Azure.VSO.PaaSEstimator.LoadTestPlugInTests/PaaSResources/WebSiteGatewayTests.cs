using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources.Tests
{
    [TestClass()]
    public class WebSiteGatewayTests
    {
        [TestMethod()]
        public void GetWebSiteDataTest()
        {
            string resource = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites/bobjacp20-1/instances?api-version=2015-08-01";

            var azureADOAuthGateway = new AzureADOAuthGateway
            {
                AuthenticationAuthority = "https://login.windows.net",
                ClientId = "78316e80-f898-43f3-9823-f652d979dc0f",
                Key = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=",
                Resource = "https://management.core.windows.net/",
                TenantId = "4ac2c945-49d5-4b59-8a70-a08dffe43dba"
            };

            var webSiteGateway = new WebSiteGateway(azureADOAuthGateway);
            string webSiteData = webSiteGateway.GetResourceAsString(resource).Result;

            Assert.IsNotNull(webSiteData);
        }
    }
}