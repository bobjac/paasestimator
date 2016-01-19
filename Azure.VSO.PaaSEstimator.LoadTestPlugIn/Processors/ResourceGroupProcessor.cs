using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;
using Newtonsoft.Json;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors
{
    /// <summary>
    /// An IPaasResourceProcessor that captures snapshots of all paas resources in a resource group
    /// </summary>
    public class ResourceGroupProcessor : IPaasResourceProcessor
    {
        /// <summary>
        /// The unique identifier of the load test
        /// </summary>
        Guid loadTestRun;

        /// <summary>
        /// The name of the load test
        /// </summary>
        private string loadTestName;

        /// <summary>
        /// Gateway class used to make REST call to ARM
        /// </summary>
        private IResourceGroupGateway resourceGroupGateway;

        /// <summary>
        /// Repository class used for storing load test snapshots
        /// </summary>
        private ILoadTestSnapshotRepository loadTestSnapshotRepository;

        private ResourceGroupEstimateRepository resourceGroupCostEstimateRepository;
        private string resourceGroupCostEstimateConnectionString;
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="loadTestRun">Unique identifier of the load test</param>
        /// <param name="loadTestName">The name of the load test</param>
        /// <param name="resourceGroupGateway">The gateway class used to make the ARM REST calls</param>
        /// <param name="loadTestSnapshotRepository">The repository class used to storage load test snapshots</param>
        public ResourceGroupProcessor(Guid loadTestRun, 
            string loadTestName,
            IResourceGroupGateway resourceGroupGateway, 
            ILoadTestSnapshotRepository loadTestSnapshotRepository,
            string resourceGroupCostEstimateRepositoryConnectionString)
        {
            this.loadTestRun = loadTestRun;
            this.loadTestName = loadTestName;
            this.resourceGroupGateway = resourceGroupGateway;
            this.loadTestSnapshotRepository = loadTestSnapshotRepository;
            this.resourceGroupCostEstimateConnectionString = resourceGroupCostEstimateRepositoryConnectionString;
        }

        /// <summary>
        /// Gets a list of web site names from json data returned from the ARM rest call
        /// </summary>
        /// <param name="webSitesJsonData">Json data returned from the ARM rest call asking for the web sites in a resource group</param>
        /// <returns>IEnumerable<string> containing the names of the web sites</string></returns>
        private IEnumerable<string> GetWebSiteNames(string webSitesJsonData)
        {
            List<string> webSiteNames = new List<string>();

            dynamic dynWebSitesData = JsonConvert.DeserializeObject(webSitesJsonData);
            foreach (var currentWebSite in dynWebSitesData.value)
            {
                string siteName = currentWebSite.properties.name;
                webSiteNames.Add(siteName);
            }

            return webSiteNames;
        }

        /// <summary>
        /// Returns the Json string representing the resource group
        /// </summary>
        /// <param name="resourceUri">The uri of the resource group</param>
        /// <returns>The json string</returns>
        public Task<string> GetPaaSResourceJson(Uri resourceUri)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Captures the snapshots of the resources in the resource group
        /// </summary>
        /// <param name="resourceUri">The resource group uri</param>
        /// <param name="captureEvent">An event associated with the capture</param>
        public void CaptureSnapshots(Uri resourceUri, string captureEvent)
        {
            CaptureWebSiteSnapshots(resourceUri, captureEvent);
        }

        /// <summary>
        /// Captures snapshots of the websites that exist in the resource group
        /// </summary>
        /// <param name="resourceGroupUri">The resource group uri</param>
        /// <param name="captureEvent">An event associated with the capture</param>
        private void CaptureWebSiteSnapshots(Uri resourceGroupUri, string captureEvent)
        {
            string resourceGroupAbsoluteUri = resourceGroupUri.AbsoluteUri;
            int index = resourceGroupAbsoluteUri.IndexOf("?");
            if (index > -1)
            {
                resourceGroupAbsoluteUri = resourceGroupUri.AbsoluteUri.Substring(0, index);
            }

            string webSitesAbsoluteUri = string.Format("{0}/providers/Microsoft.Web/sites?api-version=2015-08-01", resourceGroupAbsoluteUri);
            string webSitesData = this.resourceGroupGateway.GetWebSitesData(new Uri(webSitesAbsoluteUri)).Result;

            var siteNames = GetWebSiteNames(webSitesData);
            if (siteNames.Count() > 0)
            {
                IOathGateway oAuthGateway = (this.resourceGroupGateway as ResourceGroupGateway).OAuthGateway;

                WebSiteProcessor webSiteProcessor = new WebSiteProcessor(
                    this.loadTestRun,
                    this.loadTestName,
                    new WebSiteGateway(oAuthGateway),
                    new ServerFarmGateway(oAuthGateway),
                    new RateCardGateway(oAuthGateway),
                    new WebSiteInstancesGateway(oAuthGateway),
                    this.loadTestSnapshotRepository);


                foreach (var currentSiteName in siteNames)
                {
                    string currentSiteUri = string.Format("{0}/providers/Microsoft.Web/sites/{1}?api-version=2015-08-01", resourceGroupAbsoluteUri, currentSiteName);
                    Uri uri = new Uri(currentSiteUri);
                    webSiteProcessor.CaptureSnapshots(uri, captureEvent);
                }
            }
        }

        public ResourceGroupCostEstimate CalculateCostEstimate()
        {
            var snapshots = this.loadTestSnapshotRepository.GetLoadTestSnapshots(this.loadTestRun.ToString());
            var orderedResources = (from y in snapshots
                           group y by y.ResourceId into g
                           select g.OrderByDescending(o => o.Timestamp));

            var resourceGroupCostEstimate = GetResourceGroupCostEstimate(orderedResources);

            using (var repository = new ResourceGroupEstimateRepository(this.resourceGroupCostEstimateConnectionString))
            {
                repository.ResourceGroupCostEstimates.Add(resourceGroupCostEstimate);
                repository.SaveChanges();
            }
            
            return resourceGroupCostEstimate;
        }

        private ResourceGroupCostEstimate GetResourceGroupCostEstimate(IEnumerable<IOrderedEnumerable<LoadTestSnapShot>> groupedSnapshots)
        {
            var resourceGroupCostEstimate = new ResourceGroupCostEstimate();
            var webSiteEstimates = GetWebsiteEstimates(groupedSnapshots);

            resourceGroupCostEstimate.TotalMonthlyEstimate = webSiteEstimates.Sum(x => x.CostEstimate);
            resourceGroupCostEstimate.LoadTestId = this.loadTestRun;

            webSiteEstimates.ForEach(x => resourceGroupCostEstimate.ResourceEstimates.Add(x));

            double totalHourlyEstimate = resourceGroupCostEstimate.ResourceEstimates.Sum(x => x.CostEstimate);
            double totalMonthlyEstimate = totalHourlyEstimate * 24 * 30;

            resourceGroupCostEstimate.TotalMonthlyEstimate = totalMonthlyEstimate;

            return resourceGroupCostEstimate;
        }

        private List<ResourceCostEstimate> GetWebsiteEstimates(IEnumerable<IOrderedEnumerable<LoadTestSnapShot>> groupedSnapshots)
        {
            List<ResourceCostEstimate> webSiteCostEstimates = new List<ResourceCostEstimate>();

            foreach (var current in groupedSnapshots.GetOrderedWebsites())
            {
                ResourceCostEstimate webSiteCostEstimate = new ResourceCostEstimate();

                double maxCost = 0.0;

                foreach (var currentSnapshot in current)
                {
                    var instanceState = JsonConvert.DeserializeObject<WebSiteData>(currentSnapshot.InstanceState);

                    double currentCost = instanceState.Rate * instanceState.InstanceCount;

                    if (currentCost > maxCost)
                    {
                        maxCost = currentCost;

                        webSiteCostEstimate.Rate = instanceState.Rate;
                        webSiteCostEstimate.Instances = instanceState.InstanceCount;
                        webSiteCostEstimate.ResourceId = instanceState.WebSiteUri.ToString();
                        webSiteCostEstimate.CostEstimate = currentCost;
                        webSiteCostEstimate.RatePeriod = instanceState.RatePeriod;
                    }

                }

                webSiteCostEstimates.Add(webSiteCostEstimate);
            }

            return webSiteCostEstimates;
        }
    }
}
