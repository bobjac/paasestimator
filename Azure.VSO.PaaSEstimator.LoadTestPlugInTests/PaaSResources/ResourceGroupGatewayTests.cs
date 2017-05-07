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
    public class ResourceGroupGatewayTests
    {
        [TestMethod()]
        public void GetResourceGroupDataTest()
        {
            string resourceGroupUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20?api-version=2014-04-01";

            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            string resourceGroupData = resourceGroupGateway.GetResourceGroupData(new Uri(resourceGroupUri)).Result;

            Assert.IsNotNull(resourceGroupData);
        }

        private IOathGateway GetOAuthGateway()
        {
            return new AzureADOAuthGateway
            {
                AuthenticationAuthority = "https://login.windows.net",
                ClientId = "78316e80-f898-43f3-9823-f652d979dc0f",
                Key = "2pTqoxSBLRUjubXFTgkFazOwlmSN0e9XPSLe1RJHCEo=",
                Resource = "https://management.core.windows.net/",
                TenantId = "4ac2c945-49d5-4b59-8a70-a08dffe43dba"
            };
        }

        [TestMethod()]
        public void GetWebSitesDataTest()
        {
            string resourceGroupUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites?api-version=2015-08-01";

            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            string webSitesData = resourceGroupGateway.GetWebSitesData(new Uri(resourceGroupUri)).Result;

            Assert.IsNotNull(webSitesData);
        }
    }
}