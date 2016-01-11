using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors
{
    /// <summary>
    /// IPaasResourceProcessor that captures snapshots of web sites 
    /// </summary>
    public class WebSiteProcessor : IPaasResourceProcessor
    {
        /// <summary>
        /// OAuth authority for REST calls
        /// </summary>
        private const string AZURE_REST_AUTHORITY = "https://management.azure.com";

        /// <summary>
        /// Unique identifier of the load test run
        /// </summary>
        private Guid loadTestRun;

        /// <summary>
        /// The name of the load test
        /// </summary>
        private string loadTestName;

        /// <summary>
        /// The gateway class used for making ARM REST calls for web site data
        /// </summary>
        private IWebSiteGateway webSiteGateway;

        /// <summary>
        /// The gateway class used for making ARM REST calls for server farm data
        /// </summary>
        private IServerFarmGateway serverFarmGateway;

        /// <summary>
        /// The gateway class used for making ARM REST calls for rate card data
        /// </summary>
        private IRateCardGateway rateCardGateway;

        /// <summary>
        /// The gateway class used for making ARM REST calls for web site instances data
        /// </summary>
        private IWebSiteInstancesGateway webSiteInstancesGateway;

        /// <summary>
        /// Repository class used for storing snapshots of web site data
        /// </summary>
        private ILoadTestSnapshotRepository loadTestSnapshotRepository;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="loadTestRun">The unique identifier of the load test</param>
        /// <param name="loadTestName">The name of the load test</param>
        /// <param name="webSiteGateway">Gateway class used to make ARM REST calls for web site data</param>
        /// <param name="serverFarmGateway">Gateway class used to make ARM REST calls for server farm data<</param>
        /// <param name="rateCardGateway">Gateway class used to make ARM REST calls for rate card data<</param>
        /// <param name="webSiteInstancesGateway">Gateway class used to make ARM REST calls for web site instance data<</param>
        /// <param name="loadTestSnapshotRepository">Repository class used for storing snapshots of web site data</param>
        public WebSiteProcessor(Guid loadTestRun, 
            string loadTestName,
            IWebSiteGateway webSiteGateway, 
            IServerFarmGateway serverFarmGateway, 
            IRateCardGateway rateCardGateway,
            IWebSiteInstancesGateway webSiteInstancesGateway,
            ILoadTestSnapshotRepository loadTestSnapshotRepository)
        {
            this.loadTestRun = loadTestRun;
            this.loadTestName = loadTestName;
            this.webSiteGateway = webSiteGateway;
            this.serverFarmGateway = serverFarmGateway;
            this.rateCardGateway = rateCardGateway;
            this.webSiteInstancesGateway = webSiteInstancesGateway;
            this.loadTestSnapshotRepository = loadTestSnapshotRepository;
        }

        /// <summary>
        /// Returns web site data given an ARM URI for the web site
        /// </summary>
        /// <param name="webSiteUri">ARM URI of the web site</param>
        /// <returns>Web site data</returns>
        public async Task<WebSiteData> GetPaaSResourceData(Uri webSiteUri)
        {
            string webSiteData = await this.webSiteGateway.GetWebSiteData(webSiteUri);
            dynamic dynWebSiteData = JsonConvert.DeserializeObject(webSiteData);

           // string id = dynWebSiteData.id;
            string serverFarmId = dynWebSiteData.properties.serverFarmId;
            string siteName = dynWebSiteData.properties.name;

            ServerFarmData sfd = GetServerFarmData(serverFarmId);

            Uri instancesUri = GetWebSiteInstancesUri(webSiteUri);
            string webSiteInstancesData = await this.webSiteInstancesGateway.GetWebSiteInstancesData(instancesUri);
            WebSiteInstancesPayload webSiteInstancePayload = JsonConvert.DeserializeObject<WebSiteInstancesPayload>(webSiteInstancesData);

            List<string> instances = new List<string>();
            webSiteInstancePayload.value.ForEach(x => instances.Add(x.name));

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

        /// <summary>
        /// Returns payload of rate card data
        /// </summary>
        /// <param name="rateCardUri">ARM URI for REST calls for Rate Card information</param>
        /// <returns>Payload of Rate Card Information</returns>
        private RateCardPayload GetRateCardPayload(Uri rateCardUri)
        {
            var rateCardData = this.rateCardGateway.GetRateCardData(rateCardUri);
            RateCardPayload payload = JsonConvert.DeserializeObject<RateCardPayload>(rateCardData);
            return payload;
        }

        /// <summary>
        /// Returns server farm data given a server farm id
        /// </summary>
        /// <param name="serverFarmId">The id of the server farm</param>
        /// <returns>Server farm data</returns>
        private ServerFarmData GetServerFarmData(string serverFarmId)
        {
            string serverFarmUriString = String.Format("{0}{1}?api-version=2015-08-01", AZURE_REST_AUTHORITY, serverFarmId);
            string serverFarmDataString = serverFarmGateway.GetServerFarmData(new Uri(serverFarmUriString));
            dynamic dynServerFarmData = JsonConvert.DeserializeObject(serverFarmDataString);
            return GetServerFarmData(dynServerFarmData);
        }

        /// <summary>
        /// Constructs a ServerFarmData object from a dynamic object that 
        /// contains JSON returned by ARM REST call
        /// </summary>
        /// <param name="serverFarmData">Dynamic object containing the JSCON from
        /// The ARM REST call for server farm data</param>
        /// <returns>Populated ServerFarmData object</returns>
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

        /// <summary>
        /// Gets are Server Farm ARM URI given the web site URI and the server farm name
        /// </summary>
        /// <param name="webSiteUri">The uri of the web site that is stored in the server farm</param>
        /// <param name="serverFarmName">The name of the server farm</param>
        /// <returns></returns>
        private Uri GetServerFarmUri(Uri webSiteUri, string serverFarmName)
        {
            string serverFarmUri = webSiteUri.AbsoluteUri;
            int sitesIndex = serverFarmUri.IndexOf("/sites");
            string substring = serverFarmUri.Substring(0, sitesIndex);

            return new Uri(string.Format("{0}/serverfarms/{1}/?api-version=2015-08-01"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="webSiteUri"></param>
        /// <returns></returns>
        private Uri GetWebSiteInstancesUri(Uri webSiteUri)
        {
            string versionString = "?api-version=2015-08-01";
            int index = webSiteUri.AbsoluteUri.IndexOf(versionString);
            string instancesString = string.Format("{0}/instances{1}", webSiteUri.AbsoluteUri.Substring(0, index), versionString);
            return new Uri(instancesString);
        }

        /// <summary>
        /// Gets the URI required to get rate card information given a web site uri
        /// </summary>
        /// <param name="webSiteUri">The uri of the web site</param>
        /// <returns>The URI of the ARM REST call for RateCard information</returns>
        private Uri GetRateCardUri(Uri webSiteUri)
        {
            int index = webSiteUri.AbsoluteUri.IndexOf("/resourceGroups");

            string rateCardUriString = string.Format("{0}/providers/Microsoft.Commerce/RateCard?api-version=2015-06-01-preview&$filter=OfferDurableId eq 'MS-AZR-0062p' and Currency eq 'USD' and Locale eq 'en-US' and RegionInfo eq 'US'", 
                webSiteUri.AbsoluteUri.Substring(0, index));

            return new Uri(rateCardUriString);
        }

        /// <summary>
        /// Returns the json data from an ARM REST call to a given resource
        /// </summary>
        /// <param name="resourceUri">The resource you want data from</param>
        /// <returns>Task<string> containing the json returned from the ARM REST call</string></returns>
        public Task<string> GetPaaSResourceJson(Uri resourceUri)
        {
            return Task.FromResult(JsonConvert.SerializeObject(GetPaaSResourceData(resourceUri)));
        }

        /// <summary>
        /// Capture the snapshot of a web site along with an associated event
        /// </summary>
        /// <param name="resourceUri">The uri of the web site</param>
        /// <param name="captureEvent">The event to be associated with the capture</param>
        public void CaptureSnapshots(Uri resourceUri, string captureEvent)
        {
            var webSiteData = GetPaaSResourceData(resourceUri).Result;
            string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTestName;
            loadTestSnapshot.EventMessage = captureEvent;
            loadTestSnapshot.ResourceType = "WebSite";
            loadTestSnapshot.InstanceState = webSiteDataState;
            loadTestSnapshot.ResourceId = webSiteData.WebSiteUri.ToString();

            this.loadTestSnapshotRepository.AddLoadTestSnapshot(loadTestSnapshot);
        }

        
    }
}
