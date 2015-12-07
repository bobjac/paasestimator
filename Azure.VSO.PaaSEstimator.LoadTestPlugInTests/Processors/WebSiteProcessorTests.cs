using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors.Tests
{
    [TestClass()]
    public class WebSiteProcessorTests
    {
        private IOathGateway oauthGateway;

        [TestMethod()]
        public void GetWebSiteDataTest()
        {
            string jsonString = @"{""id"":""/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites/bobjacp20-1"",""name"":""bobjacp20-1"",""type"":""Microsoft.Web/sites"",""location"":""West US"",""tags"":{""hidden-related:/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourcegroups/p20/providers/Microsoft.Web/serverfarms/p20"":""empty""},""properties"":{""name"":""bobjacp20-1"",""state"":""Stopped"",""hostNames"":[""bobjacp20-1.azurewebsites.net""],""webSpace"":""p20-WestUSwebspace"",""selfLink"":""https://waws-prod-bay-039.api.azurewebsites.windows.net:454/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/webspaces/p20-WestUSwebspace/sites/bobjacp20-1"",""repositorySiteName"":""bobjacp20-1"",""owner"":null,""usageState"":0,""enabled"":true,""adminEnabled"":true,""enabledHostNames"":[""bobjacp20-1.azurewebsites.net"",""bobjacp20-1.scm.azurewebsites.net""],""siteProperties"":{""metadata"":null,""properties"":[],""appSettings"":null},""availabilityState"":0,""sslCertificates"":null,""csrs"":[],""cers"":null,""siteMode"":null,""hostNameSslStates"":[{""name"":""bobjacp20-1.azurewebsites.net"",""sslState"":0,""ipBasedSslResult"":null,""virtualIP"":null,""thumbprint"":null,""toUpdate"":null,""toUpdateIpBasedSsl"":null,""ipBasedSslState"":0,""hostType"":0},{""name"":""bobjacp20-1.scm.azurewebsites.net"",""sslState"":0,""ipBasedSslResult"":null,""virtualIP"":null,""thumbprint"":null,""toUpdate"":null,""toUpdateIpBasedSsl"":null,""ipBasedSslState"":0,""hostType"":1}],""computeMode"":null,""serverFarm"":null,""serverFarmId"":""/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/serverfarms/p20"",""lastModifiedTimeUtc"":""2015-12-04T02:16:51.76"",""storageRecoveryDefaultState"":""Running"",""contentAvailabilityState"":0,""runtimeAvailabilityState"":0,""siteConfig"":null,""deploymentId"":""bobjacp20-1"",""trafficManagerHostNames"":null,""sku"":""Standard"",""premiumAppDeployed"":null,""scmSiteAlsoStopped"":false,""targetSwapSlot"":null,""hostingEnvironment"":null,""hostingEnvironmentProfile"":null,""microService"":""WebSites"",""gatewaySiteName"":null,""clientAffinityEnabled"":true,""clientCertEnabled"":false,""domainVerificationIdentifiers"":null,""kind"":null,""outboundIpAddresses"":""104.42.225.41,104.42.224.45,104.42.225.216,104.42.230.71"",""cloningInfo"":null,""hostingEnvironmentId"":null,""tags"":{""hidden-related:/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourcegroups/p20/providers/Microsoft.Web/serverfarms/p20"":""empty""}}}";

            var webSiteGateway = GetWebSiteGateway();
            var serverFarmGateway = GetServerFarmGateway();
            var rateCardGateway = GetRateCardGateway();

            var webSiteProcessor = new WebSiteProcessor(webSiteGateway, serverFarmGateway, rateCardGateway);

            Uri webSiteUri = new Uri("https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites/bobjacp20-1?api-version=2015-08-01");
            var webSiteData = webSiteProcessor.GetWebSiteData(webSiteUri).Result;

            Assert.IsNotNull(webSiteData);
            Assert.AreEqual<string>("bobjacp20-1", webSiteData.SiteName);
        }

        private RateCardGateway GetRateCardGateway()
        {
            return new RateCardGateway(GetAzureADOAuthGateway());
        }

        private WebSiteGateway GetWebSiteGateway()
        {
            return new WebSiteGateway(GetAzureADOAuthGateway());
        }

        private ServerFarmGateway GetServerFarmGateway()
        {
            return new ServerFarmGateway(GetAzureADOAuthGateway());
        }

        private IOathGateway GetAzureADOAuthGateway()
        {
            if (this.oauthGateway == null)
            {
                this.oauthGateway = new AzureADOAuthGateway
                {
                    AuthenticationAuthority = "https://login.windows.net",
                    ClientId = "78316e80-f898-43f3-9823-f652d979dc0f",
                    Key = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=",
                    Resource = "https://management.core.windows.net/",
                    TenantId = "4ac2c945-49d5-4b59-8a70-a08dffe43dba"
                };
            }

            return this.oauthGateway;
        }
    }
}