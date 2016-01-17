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
    /// <summary>
    /// Visual Studio Load Test Plugin to capture state information of PaaS (elastic scale) resources
    /// during the course of a load test
    /// </summary>
    public class PaaSEstimator : ILoadTestPlugin
    {
        /// <summary>
        /// The context parameter name representing the client id of the registered application for OAuth Authentication
        /// </summary>
        private const string CLIENT_ID_PARAM = "ClientId";

        /// <summary>
        /// The context parameter name representing the client key of the registered application for OAuth Authentication
        /// </summary>
        private const string CLIENT_KEY_PARAM = "ClientKey";

        /// <summary>
        /// The Authentication authority for OAuth Authentication
        /// </summary>
        private const string AUTHENTICATION_AUTHORITY = "https://login.windows.net";

        /// <summary>
        /// The base url to be used with Azure Resource Manager web api calls
        /// </summary>
        private const string RESOURCE = "https://management.core.windows.net/";

        /// <summary>
        /// The context parameter name representing the tenant id of the registered application for OAuth Authentication
        /// </summary>
        private const string TENANT_ID_PARAM = "TenantId";

        /// <summary>
        /// The context parameter representing the uri of the resource group to be monitored during the load test
        /// </summary>
        private const string RESOURCE_GROUP_URI_PARAM = "ResourceGroupUri";

        /// <summary>
        /// The context parameter representing connection string of the ResourceGroupCostEstimateRepository
        /// </summary>
        private const string RESOURCE_GROUP_COST_ESTIMATE_CONNECTIONSTRING = "ResourceGroupCostEstimateConnectionString";

        /// <summary>
        /// The connection string of the storage account used to store the snapshots 
        /// </summary>
        private string storageAccountConnectionString;

        /// <summary>
        /// The connection string of the long term storage of Resource Group Cost Estimates
        /// </summary>
        private string resourceGroupCostEstimateConnectionString;

        /// <summary>
        /// The load test object passed in from VSO
        /// </summary>
        private LoadTest loadTest;

        /// <summary>
        /// How many heartbeats to count before writing snapshots to storage
        /// </summary>
        private int heartbeatCount;

        /// <summary>
        /// Unique identifier of the load test run
        /// </summary>
        private Guid loadTestRun;

        /// <summary>
        /// Repository used to store load test snapshots
        /// </summary>
        private ILoadTestSnapshotRepository loadTestSnapshotRepository;

        /// <summary>
        /// Domain service used to capture state of all dynamically scaled resources in a resource group
        /// </summary>
        private ResourceGroupProcessor respurceGroupProcessor;

        /// <summary>
        /// The uri of the web site being processed
        /// </summary>
        private Uri webSiteUri;

        /// <summary>
        /// The uri for the resource group being processed
        /// </summary>
        private Uri resourceGroupUri;

        /// <summary>
        /// The actual client id used for OAuth Authentication
        /// </summary>
        private string clientId;

        /// <summary>
        /// The actual key used for OAuth authentication
        /// </summary>
        private string key;

        /// <summary>
        /// The actual tenant id used for OAuth authentication
        /// </summary>
        private string tenantId;

        /// <summary>
        /// Public method used for initialization of the load test
        /// </summary>
        /// <param name="loadTest"></param>
        public void Initialize(LoadTest loadTest)
        {
            this.loadTest = loadTest;
            this.loadTestRun = Guid.NewGuid();
            this.heartbeatCount = 0;

            InitializeAzureADApp();
            InitializeWebSiteUri();
            InitializeLoadestSnapshotRepository();
            InitializeResourceGroupCostEstimateRepository();

            InitializeResourceGroupUri();
            InitializeResourceGroupProcessor();

            this.loadTest.LoadTestStarting += LoadTest_LoadTestStarting;
            this.loadTest.Heartbeat += LoadTest_Heartbeat;
            this.loadTest.LoadTestFinished += LoadTest_LoadTestFinished;
        }

        private void InitializeResourceGroupCostEstimateRepository()
        {
            if (this.loadTest.Context.ContainsKey(RESOURCE_GROUP_COST_ESTIMATE_CONNECTIONSTRING))
            {
                this.resourceGroupCostEstimateConnectionString = this.loadTest.Context[RESOURCE_GROUP_COST_ESTIMATE_CONNECTIONSTRING] as string;
            }
        }

        /// <summary>
        /// Initialized the resource group uri
        /// </summary>
        private void InitializeResourceGroupUri()
        {
            if (this.loadTest.Context.ContainsKey(RESOURCE_GROUP_URI_PARAM))
            {
                this.resourceGroupUri = new Uri(this.loadTest.Context[RESOURCE_GROUP_URI_PARAM] as string);
            }
        }

        /// <summary>
        /// Initializes the ResourceGroupProcessor
        /// </summary>
        private void InitializeResourceGroupProcessor()
        {
            var resourceGroupGateway = new ResourceGroupGateway(GetAzureADOAuthGateway());
            this.respurceGroupProcessor = new ResourceGroupProcessor(this.loadTestRun, this.loadTest.Name, resourceGroupGateway, this.loadTestSnapshotRepository, this.resourceGroupCostEstimateConnectionString);
        }

        /// <summary>
        /// Initialized the Azure AD app used for OAuth authentication
        /// </summary>
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

        /// <summary>
        /// Initialized the LoadTestSnapshotRepository
        /// </summary>
        private void InitializeLoadestSnapshotRepository()
        {
            const string STORAGE_ACCOUNT_CONNECTION_STRING = "StorageAccountConnectionString";
            if (this.loadTest.Context.ContainsKey(STORAGE_ACCOUNT_CONNECTION_STRING))
            {
                this.storageAccountConnectionString = this.loadTest.Context[STORAGE_ACCOUNT_CONNECTION_STRING] as string;
            }

            this.loadTestSnapshotRepository = new AzureTableLoadTestSnapshotRepository(this.storageAccountConnectionString);
        }

        /// <summary>
        /// Initialized the WebSite URI
        /// </summary>
        private void InitializeWebSiteUri()
        {
            string siteUriString = string.Empty;
            if (this.loadTest.Context.ContainsKey("SiteUri"))
            {
                siteUriString = this.loadTest.Context["SiteUri"] as string;
            }

            this.webSiteUri = new Uri(siteUriString);
        }

        ///// <summary>
        ///// Initialized the 
        ///// </summary>
        ///// <param name="loadTest"></param>
        //private void UpdateLoadTestContext(LoadTest loadTest)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    string sep = "";
        //    foreach (var currentKey in loadTest.Context.Keys)
        //    {
        //        sb.Append(sep);
        //        sb.Append(currentKey);
        //        sb.Append("::");
        //        sb.Append(loadTest.Context[currentKey].ToString());
        //        sep = ", ";
        //    }

        //    var loadTestSnapshot = new LoadTestSnapShot(this.loadTestRun);
        //    loadTestSnapshot.LoadTestName = this.loadTest.Name;
        //    loadTestSnapshot.EventMessage = "Updating LoadTestContext from constructor";
        //    loadTestSnapshot.InstanceState = sb.ToString();
        //    AddLoadTestSnapshot(loadTestSnapshot);
        //}

        /// <summary>
        /// Gets the IOauthGateway to be injected into the other resource gateways
        /// </summary>
        /// <returns>Implementation of the IOathGateway</returns>
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

        /// <summary>
        /// Delegate called when the load test is finished
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">Additional event args</param>
        private void LoadTest_LoadTestFinished(object sender, EventArgs e)
        {
            AddLoadTestFinishedSnapshot();
        }

        /// <summary>
        /// Method that calls the resource group processor when the load test is finished
        /// </summary>
        private void AddLoadTestFinishedSnapshot()
        {
            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Load Test Complete");

            try
            { 
                var resourceGroupCostEstimate = this.respurceGroupProcessor.CalculateCostEstimate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Delegate called whth the load test is starting
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Additional event args</param>
        private void LoadTest_LoadTestStarting(object sender, EventArgs e)
        {
            AddLoadTestStartingSnapshot();
        }

        /// <summary>
        /// Method that calls the resource group processor when the load test is starting
        /// </summary>
        private void AddLoadTestStartingSnapshot()
        {
            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Load Test Starting");
        }

        /// <summary>
        /// Delegate that is called during a load test hearbeat.
        /// Ensure that you are only capturing a shapshot every 60 seconds
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Any additioanl event args being passed</param>
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

        /// <summary>
        /// Helper method to write a LoadTestSnapshot to the repository
        /// </summary>
        /// <param name="loadTestSnapshot"></param>
        private void AddLoadTestSnapshot(LoadTestSnapShot loadTestSnapshot)
        {
            this.loadTestSnapshotRepository.AddLoadTestSnapshot(loadTestSnapshot);
        }

        /// <summary>
        /// Method to use the resource group processor to capture state when a heartbeat is hit
        /// </summary>
        private void AddHeartbeatSnapshot()
        {
            this.respurceGroupProcessor.CaptureSnapshots(this.resourceGroupUri, "Heartbeat");
        }
    }
}
