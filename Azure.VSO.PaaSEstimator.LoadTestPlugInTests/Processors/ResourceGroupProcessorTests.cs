using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors.Tests
{
    [TestClass()]
    public class ResourceGroupProcessorTests
    {
        [TestMethod()]
        public void LoadProcessorsTest()
        {
            ILoadTestSnapshotRepository loadTestSnapshotRepository = GetLoadTestSnapshotRepository();
            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            var resourceGroupProcessor = new ResourceGroupProcessor(Guid.NewGuid(), "LoadProcessorsTest()", resourceGroupGateway, loadTestSnapshotRepository);

            string resourceGroupUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20?api-version=2014-04-01";
            var processors = resourceGroupProcessor.LoadProcessors(new Uri(resourceGroupUri));
            Assert.IsNotNull(processors);

            var webSiteProcessors = processors.Where(x => x.GetType() == typeof(WebSiteProcessor));
            Assert.IsTrue(webSiteProcessors.Count() == 1);
        }

        private ILoadTestSnapshotRepository GetLoadTestSnapshotRepository()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=ydBjzHK3apIFfIXeIY7pau/gwX9zVJyMZrBvdsAkYjJF/2qgCLEMzLb3B9wLB0luBVSlW2KWBh+RaqR42jz/bQ==;BlobEndpoint=https://bobjacp20.blob.core.windows.net/;TableEndpoint=https://bobjacp20.table.core.windows.net/;QueueEndpoint=https://bobjacp20.queue.core.windows.net/;FileEndpoint=https://bobjacp20.file.core.windows.net/";
            return new AzureTableLoadTestSnapshotRepository(storageConnectionString);
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
            };
        }

        [TestMethod()]
        public void CaptureSnapshotsTest()
        {
            ILoadTestSnapshotRepository loadTestSnapshotRepository = GetLoadTestSnapshotRepository();
            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            var resourceGroupProcessor = new ResourceGroupProcessor(Guid.NewGuid(), "CaptureSnapshotsTest()", resourceGroupGateway, loadTestSnapshotRepository);

            string resourceGroupUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20?api-version=2014-04-01";
            
            resourceGroupProcessor.CaptureSnapshots(new Uri(resourceGroupUri), "CaptureSnapshotsTest()");
        }
    }
}