# PaaS Estimator
The PaaS Estimator is a Visual Studio Team Services Load Test plugin that estimates the cost of running PaaS workloads.  Estimating PaaS workloads can be difficult given the elastic scale of them.  Instances are added and removed based off metrics used in elastic scale rules and the cost can fluctuate depending on user load.  The PaaS estimator takes advantage of the real-world load used in performance tests to see how the elastic configuration will respond.

The VSTS load test plug in polls the reseources in resource group at regular by using the Azure Resource Manager APIs, and calculates the cost via the Azure Billing API.  At the conclusion of the load test, the estimated cost is stored in a data store to be visualized by an external production

This sample creates proper interfaces to support multiple elastic resources, storage repositories, and extensions.  The default instances of the interfaces are scoped to Azure Web Apps, Azure Storage Tables (temporary storage during load test), and Azure SQL Database (final estimates).


