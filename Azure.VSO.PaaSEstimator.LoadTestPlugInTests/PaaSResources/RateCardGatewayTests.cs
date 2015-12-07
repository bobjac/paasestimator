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
    public class RateCardGatewayTests
    {
        private const string rateCardUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq 'MS-AZR-0003p' and Currency eq 'USD' and Locale eq 'en-US' and RegionInfo eq 'US'";
        //private const string rateCardUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId%20eq%20'MS-AZR-0003p'%20and%20Currency%20eq%20'USD'%20and%20Locale%20eq%20'en-US'%20and%20RegionInfo%20eq%20'US'";
        [TestMethod()]
        public void GetRateCardDataTest()
        {
            var rateCardGateway = new RateCardGateway(GetOAuthGateway());
            var rateCardData = rateCardGateway.GetRateCardData(new Uri(rateCardUri));

            Assert.IsNotNull(rateCardData);

        }

        private IOathGateway GetOAuthGateway()
        {
            return new AzureADOAuthGateway
            {
                AuthenticationAuthority = "https://login.windows.net",
                ClientId = "78316e80-f898-43f3-9823-f652d979dc0f",
                Key = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=",
                Resource = "https://management.core.windows.net/",
                TenantId = "4ac2c945-49d5-4b59-8a70-a08dffe43dba"
            }; ;
        }
    }
}