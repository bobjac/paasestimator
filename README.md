# PaaS Estimator
The PaaS Estimator is a Visual Studio Team Services Load Test plugin that estimates the cost of running PaaS workloads.  Estimating PaaS workloads can be difficult given the elastic scale of them.  Instances are added and removed based off metrics used in elastic scale rules and the cost can fluctuate depending on user load.  The PaaS estimator takes advantage of the real-world load used in performance tests to see how the elastic configuration will respond.

The VSTS load test plug in polls the reseources in resource group at regular by using the Azure Resource Manager APIs, and calculates the cost via the Azure Billing API.  At the conclusion of the load test, the estimated cost is stored in a data store to be visualized by an external production

This sample creates proper interfaces to support multiple elastic resources, storage repositories, and extensions.  The default instances of the interfaces are scoped to Azure Web Apps, Azure Storage Tables (temporary storage during load test), and Azure SQL Database (final estimates).

## Project Setup
The solution is broken into 3 projects.  
    - Azure.VSO.PaaSEstimator.LoadTestPlugin 
    - Azure.VSO.PaaSEstimator.LoadTestPluginTests
    - Azure.VSO.PaaSEstimator.LoadTest

The core project is found in Azure.VSO.PaaSEstimator.LoadTestPlugin and contains all plugin code.  Processors, found in the corresponing folder are used to coordinate the estimation of the corresponding Azure resource.  An Azure Webapp is the only resource available at the time, but it serves as a sample of all other elastic scale resources in Azure.  The PaaSResources folder is used to abstract the REST API calls to Azure Resource Manager.  The Repositories folder is used to store estimate data and temporary data during the load test run.

The Azure.VSO.PaaSEstimator.LoadTestPluginTests contains unit tests for the Azure.VSO.PaaSEstimator.LoadTestPlugin code.

The Azure.VSO.PaaSEstimator.LoadTest project contains configuration settings and parameters for the load test itself.

## Execution
The project is preconfigured with parameters and data stores set up.  Simply run the load test with the existing settings.  For your own purposes, replace the Azure resources with your own.


