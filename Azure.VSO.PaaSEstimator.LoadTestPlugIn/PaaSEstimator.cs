using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.LoadTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn
{
    public class PaaSEstimator : ILoadTestPlugin
    {
        private LoadTest loadTest;
        private string connectionString;
        private int heartbeatCount;
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private Guid loadTestRun;

        private const string clientId = "78316e80-f898-43f3-9823-f652d979dc0f";
        private const string key = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=";

        public void Initialize(LoadTest loadTest)
        {
            this.loadTest = loadTest;
            this.loadTestRun = Guid.NewGuid();
            this.heartbeatCount = 0;
            this.connectionString = "DefaultEndpointsProtocol=https;AccountName=bobjacp20;AccountKey=ydBjzHK3apIFfIXeIY7pau/gwX9zVJyMZrBvdsAkYjJF/2qgCLEMzLb3B9wLB0luBVSlW2KWBh+RaqR42jz/bQ==;BlobEndpoint=https://bobjacp20.blob.core.windows.net/;TableEndpoint=https://bobjacp20.table.core.windows.net/;QueueEndpoint=https://bobjacp20.queue.core.windows.net/;FileEndpoint=https://bobjacp20.file.core.windows.net/";
            this.storageAccount = CloudStorageAccount.Parse(this.connectionString);
            this.tableClient = this.storageAccount.CreateCloudTableClient();
            this.loadTest.LoadTestStarting += LoadTest_LoadTestStarting;
            this.loadTest.Heartbeat += LoadTest_Heartbeat;
            this.loadTest.LoadTestFinished += LoadTest_LoadTestFinished;

        }

        private void LoadTest_LoadTestFinished(object sender, EventArgs e)
        {
            AddLoadTestFinishedSnapshot();
        }

        private void AddLoadTestFinishedSnapshot()
        {
            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Load Test Complete";
            loadTestSnapshot.InstanceState = "{ json: goeshere}";
            AddLoadTestSnapshot(loadTestSnapshot);
        }

        private void LoadTest_LoadTestStarting(object sender, EventArgs e)
        {
            AddLoadTestStartingSnapshot();
        }

        private void AddLoadTestStartingSnapshot()
        {
            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Load Test Starting";
            loadTestSnapshot.InstanceState = "{ json: goeshere}";
            AddLoadTestSnapshot(loadTestSnapshot);
        }

        private string GetJson()
        {
            return string.Empty;
        }

        private void LoadTest_Heartbeat(object sender, HeartbeatEventArgs e)
        {
            if (this.heartbeatCount >= 60)
            {
                AddHeartbeatSnapshot();
                this.heartbeatCount = 0;
            }
            else
            {
                this.heartbeatCount++;
            }
        }

        private void AddLoadTestSnapshot(LoadTestSnapShot loadTestSnapshot)
        {
            var table = this.tableClient.GetTableReference("loadTestSnapshot");
            table.CreateIfNotExists();

            var insertOperation = TableOperation.Insert(loadTestSnapshot);
            TableResult result = table.Execute(insertOperation);
        }

        private void AddHeartbeatSnapshot()
        {
            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Heartbeat";
            loadTestSnapshot.InstanceState = "{ json: goeshere}";
            AddLoadTestSnapshot(loadTestSnapshot);
        }
    }
}
