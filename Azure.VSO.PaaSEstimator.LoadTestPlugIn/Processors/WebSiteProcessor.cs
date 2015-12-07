using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors
{
    public class WebSiteProcessor
    {
        private IWebSiteGateway webSiteGateway;
        private IServerFarmGateway serverFarmGateway;
        private IRateCardGateway rateCardGateway;

        public WebSiteProcessor(IWebSiteGateway webSiteGateway, IServerFarmGateway serverFarmGateway, IRateCardGateway rateCardGateway)
        {
            this.webSiteGateway = webSiteGateway;
            this.serverFarmGateway = serverFarmGateway;
            this.rateCardGateway = rateCardGateway;
        }

        //public WebSiteProcessor(WebSiteGateway webSiteGateway, ServerFarmGateway serverFarmGateway)
        //{
        //    this.webSiteGateway = webSiteGateway;
        //    this.serverFarmGateway = serverFarmGateway;
        //}

        public async Task<WebSiteData> GetWebSiteData(Uri webSiteUri)
        {
            string webSiteData = await this.webSiteGateway.GetWebSiteData(webSiteUri);
            
            dynamic dynWebSiteData = JsonConvert.DeserializeObject(webSiteData);
     
            string id = dynWebSiteData.id;
            string serverFarmId = dynWebSiteData.properties.serverFarmId;

            string azureRestAuthority = "https://management.azure.com";

            string serverFarmUriString = String.Format("{0}{1}?api-version=2015-08-01", azureRestAuthority, serverFarmId);
            string serverFarmDataString = serverFarmGateway.GetServerFarmData(new Uri(serverFarmUriString));

            Uri rateCardUri = GetRateCardUri(webSiteUri);
            var rateCardData = this.rateCardGateway.GetRateCardData(rateCardUri);

            dynamic dynServerFarmData = JsonConvert.DeserializeObject(serverFarmDataString);
            ServerFarmData serverFarmData = GetServerFarmData(dynServerFarmData);

            return new WebSiteData
            {
                SiteName = dynWebSiteData.properties.name
            };
        }

        private ServerFarmData GetServerFarmData(dynamic serverFarmData)
        {
            return new ServerFarmData
            {
                ServerFarmName = serverFarmData.name,
                Location = serverFarmData.location,
                Capacity = serverFarmData.sku.capacity,
                Family = serverFarmData.sku.family,
                Size = serverFarmData.sku.size,
                Tier = serverFarmData.sku.tier
            };
        }

        private Uri GetServerFarmUri(Uri webSiteUri, string serverFarmName)
        {
            string serverFarmUri = webSiteUri.AbsoluteUri;
            int sitesIndex = serverFarmUri.IndexOf("/sites");
            string substring = serverFarmUri.Substring(0, sitesIndex);

            return new Uri(string.Format("{0}/serverfarms/{1}/?api-version=2015-08-01"));
        }

        private Uri GetRateCardUri(Uri webSiteUri)
        {
            int index = webSiteUri.AbsoluteUri.IndexOf("/resourceGroups");
            string rateCardUriString = string.Format("{0}/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq 'MS-AZR-0003p' and Currency eq 'USD' and Locale eq 'en-US' and RegionInfo eq 'US'", 
                webSiteUri.AbsoluteUri.Substring(0, index));
            return new Uri(rateCardUriString);
        }
    }
}
