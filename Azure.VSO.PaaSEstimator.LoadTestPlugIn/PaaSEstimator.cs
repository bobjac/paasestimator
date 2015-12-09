using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.LoadTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Processors;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources;
using Newtonsoft.Json;

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

        private WebSiteProcessor webSiteProcessor;

        private Uri webSiteUri = new Uri("https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites/bobjacp20-1?api-version=2015-08-01");

        private const string CLIENT_ID = "78316e80-f898-43f3-9823-f652d979dc0f";
        private const string KEY = "+9IaNb9TTwmAkAjayZheZY1ZQJYohWtPCpMrFRmoRcg=";
        private const string AUTHENTICATION_AUTHORITY = "https://login.windows.net";
        private const string RESOURCE = "https://management.core.windows.net/";
        private const string TENANT_ID = "4ac2c945-49d5-4b59-8a70-a08dffe43dba";

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

            InitializeWebSiteProcessor();
        }

        private void InitializeWebSiteProcessor()
        {
            IOathGateway oauthGateway = GetAzureADOAuthGateway();
            IRateCardGateway rateCardGateway = new RateCardGateway(oauthGateway);
            IWebSiteInstancesGateway webSiteInstancesGateway = new WebSiteInstancesGateway(oauthGateway);
            IServerFarmGateway serverFarmGateway = new ServerFarmGateway(oauthGateway);
            IWebSiteGateway webSiteGateway = new WebSiteGateway(oauthGateway);

            this.webSiteProcessor = new WebSiteProcessor(webSiteGateway, serverFarmGateway, rateCardGateway, webSiteInstancesGateway);
        }
        private IOathGateway GetAzureADOAuthGateway()
        {
            return new AzureADOAuthGateway
            {
                AuthenticationAuthority = AUTHENTICATION_AUTHORITY,
                ClientId = CLIENT_ID,
                Key = KEY,
                Resource = RESOURCE,
                TenantId = TENANT_ID
            };
        }

        private void LoadTest_LoadTestFinished(object sender, EventArgs e)
        {
            AddLoadTestFinishedSnapshot();
        }

        private void AddLoadTestFinishedSnapshot()
        {
            var webSiteData = this.webSiteProcessor.GetWebSiteData(this.webSiteUri).Result;
            string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Load Test Complete";
            loadTestSnapshot.InstanceState = webSiteDataState;
            AddLoadTestSnapshot(loadTestSnapshot);
        }

        private void LoadTest_LoadTestStarting(object sender, EventArgs e)
        {
            AddLoadTestStartingSnapshot();
        }

        private void AddLoadTestStartingSnapshot()
        {
            var webSiteData = this.webSiteProcessor.GetWebSiteData(this.webSiteUri).Result;
            string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Load Test Starting";
            loadTestSnapshot.InstanceState = webSiteDataState;
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
            var webSiteData = this.webSiteProcessor.GetWebSiteData(this.webSiteUri).Result;
            string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Heartbeat";
            loadTestSnapshot.InstanceState = webSiteDataState;
            AddLoadTestSnapshot(loadTestSnapshot);
        }
    }
}
