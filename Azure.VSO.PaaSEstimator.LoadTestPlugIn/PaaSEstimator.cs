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
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Repositories;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn
{
    public class PaaSEstimator : ILoadTestPlugin
    {
        private string storageAccountConnectionString;

        private LoadTest loadTest;
        private int heartbeatCount;
        private Guid loadTestRun;
        private ILoadTestSnapshotRepository loadTestSnapshotRepository;
        //private WebSiteProcessor webSiteProcessor;

        private ResourceGroupProcessor respurceGroupProcessor;

        private Uri webSiteUri;
        private Uri resourceGroupUri;

        private string clientId;
        private string key;
        private string tenantId;

        private const string CLIENT_ID_PARAM = "ClientId";
        private const string CLIENT_KEY_PARAM = "ClientKey";
        private const string AUTHENTICATION_AUTHORITY = "https://login.windows.net";
        private const string RESOURCE = "https://management.core.windows.net/";
        private const string TENANT_ID_PARAM = "TenantId";
        private const string RESOURCE_GROUP_URI_PARAM = "ResourceGroupUri";

        public void Initialize(LoadTest loadTest)
        {
            this.loadTest = loadTest;
            this.loadTestRun = Guid.NewGuid();
            this.heartbeatCount = 0;

            InitializeAzureADApp();
            InitializeWebSiteUri();
            InitializeLoadestSnapshotRepository();

            InitializeResourceGroupUri();
            InitializeResourceGroupProcessor();

            //InitializeWebSiteProcessor();

            this.loadTest.LoadTestStarting += LoadTest_LoadTestStarting;
            this.loadTest.Heartbeat += LoadTest_Heartbeat;
            this.loadTest.LoadTestFinished += LoadTest_LoadTestFinished;
        }

        private void InitializeResourceGroupUri()
        {
            if (this.loadTest.Context.ContainsKey(RESOURCE_GROUP_URI_PARAM))
            {
                this.resourceGroupUri = new Uri(this.loadTest.Context[RESOURCE_GROUP_URI_PARAM] as string);
            }
        }

        private void InitializeResourceGroupProcessor()
        {
            var resourceGroupGateway = new ResourceGroupGateway(GetAzureADOAuthGateway());
            this.respurceGroupProcessor = new ResourceGroupProcessor(this.loadTestRun, this.loadTest.Name, resourceGroupGateway, this.loadTestSnapshotRepository);
        }

        private void InitializeAzureADApp()
        {
            if (this.loadTest.Context.ContainsKey(CLIENT_ID_PARAM))
            {
                this.clientId = this.loadTest.Context[CLIENT_ID_PARAM] as string;
            }

            if (this.loadTest.Context.ContainsKey(CLIENT_KEY_PARAM))
            {
                this.key = this.loadTest.Context[CLIENT_KEY_PARAM] as string;
            }

            if (this.loadTest.Context.ContainsKey(TENANT_ID_PARAM))
            {
                this.tenantId = this.loadTest.Context[TENANT_ID_PARAM] as string;
            }
        }

        private void InitializeLoadestSnapshotRepository()
        {
            const string STORAGE_ACCOUNT_CONNECTION_STRING = "StorageAccountConnectionString";
            if (this.loadTest.Context.ContainsKey(STORAGE_ACCOUNT_CONNECTION_STRING))
            {
                this.storageAccountConnectionString = this.loadTest.Context[STORAGE_ACCOUNT_CONNECTION_STRING] as string;
            }

            this.loadTestSnapshotRepository = new AzureTableLoadTestSnapshotRepository(this.storageAccountConnectionString);
        }

        private void InitializeWebSiteUri()
        {
            string siteUriString = string.Empty;
            if (this.loadTest.Context.ContainsKey("SiteUri"))
            {
                siteUriString = this.loadTest.Context["SiteUri"] as string;
            }
            else
            {
                // siteUriString = "https://management.azure.com/subscriptions/7840d2da-7eb0-4caa-a8af-e69f387c3557/resourceGroups/p20/providers/Microsoft.Web/sites/bobjacp20-1?api-version=2015-08-01";
            }

            this.webSiteUri = new Uri(siteUriString);
        }

        private void UpdateLoadTestContext(LoadTest loadTest)
        {
            StringBuilder sb = new StringBuilder();
            string sep = "";
            foreach (var currentKey in loadTest.Context.Keys)
            {
                sb.Append(sep);
                sb.Append(currentKey);
                sb.Append("::");
                sb.Append(loadTest.Context[currentKey].ToString());
                sep = ", ";
            }

            var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            loadTestSnapshot.LoadTestName = this.loadTest.Name;
            loadTestSnapshot.EventMessage = "Updating LoadTestContext from constructor";
            loadTestSnapshot.InstanceState = sb.ToString();
            AddLoadTestSnapshot(loadTestSnapshot);
        }

        //private void InitializeWebSiteProcessor()
        //{
        //    IOathGateway oauthGateway = GetAzureADOAuthGateway();
        //    IRateCardGateway rateCardGateway = new RateCardGateway(oauthGateway);
        //    IWebSiteInstancesGateway webSiteInstancesGateway = new WebSiteInstancesGateway(oauthGateway);
        //    IServerFarmGateway serverFarmGateway = new ServerFarmGateway(oauthGateway);
        //    IWebSiteGateway webSiteGateway = new WebSiteGateway(oauthGateway);
        //    ILoadTestSnapshotRepository loadTestSnapshotRepository = new AzureTableLoadTestSnapshotRepository(this.storageAccountConnectionString);

        //    this.webSiteProcessor = new WebSiteProcessor(this.loadTestRun, this.loadTest.Name, webSiteGateway, serverFarmGateway, rateCardGateway, webSiteInstancesGateway, loadTestSnapshotRepository);
        //}
        private IOathGateway GetAzureADOAuthGateway()
        {
            return new AzureADOAuthGateway
            {
                AuthenticationAuthority = AUTHENTICATION_AUTHORITY,
                ClientId = this.clientId,
                Key = this.key,
                Resource = RESOURCE,
                TenantId = this.tenantId
            };
        }

        private void LoadTest_LoadTestFinished(object sender, EventArgs e)
        {
            AddLoadTestFinishedSnapshot();
        }

        private void AddLoadTestFinishedSnapshot()
        {
            //var webSiteData = this.webSiteProcessor.GetPaaSResourceData(this.webSiteUri).Result;
            //string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            //var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            //loadTestSnapshot.LoadTestName = this.loadTest.Name;
            //loadTestSnapshot.EventMessage = "Load Test Complete";
            //loadTestSnapshot.InstanceState = webSiteDataState;
            //AddLoadTestSnapshot(loadTestSnapshot);

            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Load Test Complete");
        }

        private void LoadTest_LoadTestStarting(object sender, EventArgs e)
        {
            // UpdateLoadTestContext(this.loadTest);
            AddLoadTestStartingSnapshot();
        }

        private void AddLoadTestStartingSnapshot()
        {
            //var webSiteData = this.webSiteProcessor.GetPaaSResourceData(this.webSiteUri).Result;
            //string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            //var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            //loadTestSnapshot.LoadTestName = this.loadTest.Name;
            //loadTestSnapshot.EventMessage = "Load Test Starting";
            //loadTestSnapshot.InstanceState = webSiteDataState;

            //AddLoadTestSnapshot(loadTestSnapshot);

            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Load Test Starting");
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
            this.loadTestSnapshotRepository.AddLoadTestSnapshot(loadTestSnapshot);
        }

        private void AddHeartbeatSnapshot()
        {
            //var webSiteData = this.webSiteProcessor.GetPaaSResourceData(this.webSiteUri).Result;
            //string webSiteDataState = JsonConvert.SerializeObject(webSiteData);

            //var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
            //loadTestSnapshot.LoadTestName = this.loadTest.Name;
            //loadTestSnapshot.EventMessage = "Heartbeat";
            //loadTestSnapshot.InstanceState = webSiteDataState;
            //AddLoadTestSnapshot(loadTestSnapshot);

            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Heartbeat");
        }
    }
}
