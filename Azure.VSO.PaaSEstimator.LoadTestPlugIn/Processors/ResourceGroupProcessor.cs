using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors
{
    public class ResourceGroupProcessor : IPaasResourceProcessor
    {
        Guid loadTestRun;
        private string loadTestName;
        private IResourceGroupGateway resourceGroupGateway;
        private ILoadTestSnapshotRepository loadTestSnapshotRepository;

        public ResourceGroupProcessor(Guid loadTestRun, 
            string loadTestName,
            IResourceGroupGateway resourceGroupGateway, 
            ILoadTestSnapshotRepository loadTestSnapshotRepository)
        {
            this.loadTestRun = loadTestRun;
            this.loadTestName = loadTestName;
            this.resourceGroupGateway = resourceGroupGateway;
            this.loadTestSnapshotRepository = loadTestSnapshotRepository;
        }

        public async Task<string> GetPaaSResourceData(Uri webSiteUri)
        {
            return await Task.FromResult<string>("TEST");
        }

        public List<IPaasResourceProcessor> LoadProcessors(Uri resourceGroupUri)
        {
            List<IPaasResourceProcessor> processors = new List<IPaasResourceProcessor>();

            //string resourceGroupAbsoluteUri = resourceGroupUri.AbsoluteUri;
            //int index = resourceGroupAbsoluteUri.IndexOf("?");
            //if (index > -1)
            //{
            //    resourceGroupAbsoluteUri = resourceGroupUri.AbsoluteUri.Substring(0, index);
            //}

            //string webSitesAbsoluteUri = string.Format("{0}/providers/Microsoft.Web/sites?api-version=2015-08-01", resourceGroupAbsoluteUri);
            //string webSitesData = this.resourceGroupGateway.GetWebSitesData(new Uri(webSitesAbsoluteUri)).Result;

            //var siteNames = GetWebSiteNames(webSitesData);
            //if (siteNames.Count() > 0)
            //{
            //    IOathGateway oAuthGateway = (this.resourceGroupGateway as ResourceGroupGateway).OAuthGateway;

            //    WebSiteProcessor webSiteProcessor = new WebSiteProcessor(
            //        new WebSiteGateway(oAuthGateway),
            //        new ServerFarmGateway(oAuthGateway),
            //        new RateCardGateway(oAuthGateway),
            //        new WebSiteInstancesGateway(oAuthGateway),
            //        this.loadTestSnapshotRepository);


            //    foreach (var currentSiteName in siteNames)
            //    {
            //        string currentSiteUri = string.Format("{0}/providers/Microsoft.Web/sites/{1}?api-version=2015-08-01", resourceGroupAbsoluteUri, currentSiteName);
            //        Uri uri = new Uri(currentSiteUri);
            //        webSiteProcessor.CaptureSnapshots(uri);
            //    }

            //   processors.Add(webSiteProcessor);
            //}

            return processors;
        }

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

        public Task<string> GetPaaSResourceJson(Uri resourceUri)
        {
            throw new NotImplementedException();
        }

        public void CaptureSnapshots(Uri resourceUri, string captureEvent)
        {
            CaptureWebSiteSnapshots(resourceUri, captureEvent);
        }

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
    }
}
