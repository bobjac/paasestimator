using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;
using System.Data.Entity;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors.Tests
{
    [TestClass()]
    public class ResourceGroupProcessorTests
    {
        private ILoadTestSnapshotRepository GetLoadTestSnapshotRepository()
        {
            //string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=ydBjzHK3apIFfIXeIY7pau/gwX9zVJyMZrBvdsAkYjJF/2qgCLEMzLb3B9wLB0luBVSlW2KWBh+RaqR42jz/bQ==;BlobEndpoint=https://bobjacp20.blob.core.windows.net/;TableEndpoint=https://bobjacp20.table.core.windows.net/;QueueEndpoint=https://bobjacp20.queue.core.windows.net/;FileEndpoint=https://bobjacp20.file.core.windows.net/";
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=FS0wgyZGzPCHOKJNoJMwoix+8QEpEtjG/hY6mkEki19rD97ToDZIC22hEf/rZswqdRYSX56qmb/XG5wY5S8piQ==;EndpointSuffix=core.windows.net";
            return new AzureTableLoadTestSnapshotRepository(storageConnectionString);
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
        public void CaptureSnapshotsTest()
        {
            string connectionString = "Server=tcp:bobjacp20.database.windows.net,1433;Initial Catalog=bobjacp20;Persist Security Info=False;User ID=bobjac;Password=P@ssw0rd1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            ILoadTestSnapshotRepository loadTestSnapshotRepository = GetLoadTestSnapshotRepository();
            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            var resourceGroupProcessor = new ResourceGroupProcessor(Guid.NewGuid(), "CaptureSnapshotsTest()", resourceGroupGateway, loadTestSnapshotRepository, connectionString);

            string resourceGroupUri = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20?api-version=2014-04-01";

            resourceGroupProcessor.CaptureSnapshots(new Uri(resourceGroupUri), "CaptureSnapshotsTest()");
        }

        [TestMethod()]
        public void CalculateCostEstimateTest()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<ResourceGroupEstimateRepository>());
            string connectionString = "Server=tcp:bobjacp20.database.windows.net,1433;Initial Catalog=bobjacp20;Persist Security Info=False;User ID=bobjac;Password=P@ssw0rd1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            Guid loadTestRun = Guid.Parse("dee52837-f7ac-4a6e-a034-514bda8e3c6e");
            string loadTestName = "paasestimatorloadtest";

            double expectedHourly = 0.2920000000072 * 5;
            double expected = expectedHourly * 24 * 30;

            var resourceGroupGateway = new ResourceGroupGateway(GetOAuthGateway());
            ILoadTestSnapshotRepository loadTestSnapshotRepository = GetLoadTestSnapshotRepository();
            var resourceGroupProcessor = new ResourceGroupProcessor(loadTestRun, loadTestName, resourceGroupGateway, loadTestSnapshotRepository, connectionString);
            var resourceGroupCostEstimate = resourceGroupProcessor.CalculateCostEstimate();

            Assert.AreEqual(expected, resourceGroupCostEstimate.TotalMonthlyEstimate);
        }
    }
}