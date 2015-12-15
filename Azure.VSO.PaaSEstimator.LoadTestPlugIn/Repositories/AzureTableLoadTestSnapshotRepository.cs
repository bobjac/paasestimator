using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories
{
    public class AzureTableLoadTestSnapshotRepository : ILoadTestSnapshotRepository
    {

        private CloudTableClient tableClient;

        public AzureTableLoadTestSnapshotRepository(string storageAccountConnectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            this.tableClient = storageAccount.CreateCloudTableClient();
        }

        public void AddLoadTestSnapshot(LoadTestSnapShot loadTestSnapshot)
        {
            var table = this.tableClient.GetTableReference("loadTestSnapshot");
            table.CreateIfNotExists();

            var insertOperation = TableOperation.Insert(loadTestSnapshot);
            TableResult result = table.Execute(insertOperation);
        }
    }
}
