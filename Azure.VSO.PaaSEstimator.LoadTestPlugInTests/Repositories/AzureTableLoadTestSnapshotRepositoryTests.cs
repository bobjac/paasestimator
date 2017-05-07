using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories.Tests
{
    [TestClass()]
    public class AzureTableLoadTestSnapshotRepositoryTests
    {
        [TestMethod()]
        public void GetLoadTestSnapshotsTest()
        {
            //string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=ydBjzHK3apIFfIXeIY7pau/gwX9zVJyMZrBvdsAkYjJF/2qgCLEMzLb3B9wLB0luBVSlW2KWBh+RaqR42jz/bQ==;BlobEndpoint=https://bobjacp20.blob.core.windows.net/;TableEndpoint=https://bobjacp20.table.core.windows.net/;QueueEndpoint=https://bobjacp20.queue.core.windows.net/;FileEndpoint=https://bobjacp20.file.core.windows.net/";
            string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=FS0wgyZGzPCHOKJNoJMwoix+8QEpEtjG/hY6mkEki19rD97ToDZIC22hEf/rZswqdRYSX56qmb/XG5wY5S8piQ==;EndpointSuffix=core.windows.net";
            string partitionKey = "e2c6f6c0-e9da-4792-9cdd-b76b94d703f7";

            var repository = new AzureTableLoadTestSnapshotRepository(storageAccountConnectionString);
            var snapshots = repository.GetLoadTestSnapshots(partitionKey).OrderByDescending(x => x.Timestamp);

            Assert.IsNotNull(snapshots);
        }
    }
}