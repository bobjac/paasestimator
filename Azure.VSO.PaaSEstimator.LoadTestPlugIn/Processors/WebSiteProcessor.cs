﻿using System;
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
        private const string AZURE_REST_AUTHORITY = "https://management.azure.com";

        private IWebSiteGateway webSiteGateway;
        private IServerFarmGateway serverFarmGateway;
        private IRateCardGateway rateCardGateway;
        private IWebSiteInstancesGateway webSiteInstancesGateway;

        public WebSiteProcessor(IWebSiteGateway webSiteGateway, 
            IServerFarmGateway serverFarmGateway, 
            IRateCardGateway rateCardGateway,
            IWebSiteInstancesGateway webSiteInstancesGateway)
        {
            this.webSiteGateway = webSiteGateway;
            this.serverFarmGateway = serverFarmGateway;
            this.rateCardGateway = rateCardGateway;
            this.webSiteInstancesGateway = webSiteInstancesGateway;
        }


        public async Task<WebSiteData> GetWebSiteData(Uri webSiteUri)
        {
            string webSiteData = await this.webSiteGateway.GetWebSiteData(webSiteUri);
            dynamic dynWebSiteData = JsonConvert.DeserializeObject(webSiteData);

           // string id = dynWebSiteData.id;
            string serverFarmId = dynWebSiteData.properties.serverFarmId;
            string siteName = dynWebSiteData.properties.name;

            int capacity = 0;
            int listInstances = 0;

            ServerFarmData sfd = GetServerFarmData(serverFarmId);

            capacity = int.Parse(sfd.Capacity);

            Uri instancesUri = GetWebSiteInstancesUri(webSiteUri);
            string webSiteInstancesData = await this.webSiteInstancesGateway.GetWebSiteInstancesData(instancesUri);
            WebSiteInstancesPayload webSiteInstancePayload = JsonConvert.DeserializeObject<WebSiteInstancesPayload>(webSiteInstancesData);
            List<string> instances = new List<string>();
            webSiteInstancePayload.value.ForEach(x => instances.Add(x.name));

            listInstances = instances.Count;

            bool areSame = false;
            if (capacity == listInstances)
            {
                areSame = true;
            }

            Uri rateCardUri = GetRateCardUri(webSiteUri);
            RateCardPayload rateCardPayload = GetRateCardPayload(rateCardUri);

            string meterName = sfd.GetMeterName();
            var meter = rateCardPayload.Meters.FirstOrDefault(x => x.MeterName.Equals(meterName));

            return new WebSiteData
            {
                WebSiteUri = webSiteUri,
                SiteName = siteName,
                Rate = meter.MeterRates[0],
                RatePeriod = meter.Unit,
                Instances = instances,
                InstanceCount = instances.Count
            };
        }

        private RateCardPayload GetRateCardPayload(Uri rateCardUri)
        {
            var rateCardData = this.rateCardGateway.GetRateCardData(rateCardUri);
            RateCardPayload payload = JsonConvert.DeserializeObject<RateCardPayload>(rateCardData);
            return payload;
        }

        private List<WebSiteInstanceData> GetWebSiteInstances(Uri webSiteInstancesUri)
        {
            List<WebSiteInstanceData> webSiteInstances = new List<WebSiteInstanceData>();



            return webSiteInstances;
        }

        private ServerFarmData GetServerFarmData(string serverFarmId)
        {
            string serverFarmUriString = String.Format("{0}{1}?api-version=2015-08-01", AZURE_REST_AUTHORITY, serverFarmId);
            string serverFarmDataString = serverFarmGateway.GetServerFarmData(new Uri(serverFarmUriString));
            dynamic dynServerFarmData = JsonConvert.DeserializeObject(serverFarmDataString);
            return GetServerFarmData(dynServerFarmData);
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

        private Uri GetWebSiteInstancesUri(Uri webSiteUri)
        {
            string versionString = "?api-version=2015-08-01";
            int index = webSiteUri.AbsoluteUri.IndexOf(versionString);
            string instancesString = string.Format("{0}/instances{1}", webSiteUri.AbsoluteUri.Substring(0, index), versionString);
            return new Uri(instancesString);
        }

        private Uri GetRateCardUri(Uri webSiteUri)
        {
            int index = webSiteUri.AbsoluteUri.IndexOf("/resourceGroups");

            string rateCardUriString = string.Format("{0}/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq 'MS-AZR-0062p' and Currency eq 'USD' and Locale eq 'en-US' and RegionInfo eq 'US'", 
                webSiteUri.AbsoluteUri.Substring(0, index));

            return new Uri(rateCardUriString);
        }
    }
}