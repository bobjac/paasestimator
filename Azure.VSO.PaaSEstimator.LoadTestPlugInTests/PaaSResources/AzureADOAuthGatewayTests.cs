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
    public class AzureADOAuthGatewayTests
    {
        private AzureADOAuthGateway azureADOAuthGateway = new AzureADOAuthGateway
        {
            AuthenticationAuthority = "https://login.windows.net",
            ClientId = "78316e80-f898-43f3-9823-f652d979dc0f",
            //Key = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=",
            Key = "2pTqoxSBLRUjubXFTgkFazOwlmSN0e9XPSLe1RJHCEo=",
            Resource = "https://management.core.windows.net/",
            TenantId = "4ac2c945-49d5-4b59-8a70-a08dffe43dba"
        };

        [TestMethod()]
        public void GetOAuthTokenTest()
        {
            string bearerToken = azureADOAuthGateway.GetOAuthToken();
            Assert.IsNotNull(bearerToken);
        }
    }
}