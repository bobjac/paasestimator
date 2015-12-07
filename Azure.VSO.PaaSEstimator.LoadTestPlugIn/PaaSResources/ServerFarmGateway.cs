using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.VSO.PaaSEstimator.LoadTestPlugIn.Models;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class ServerFarmGateway : ArmResourceGateway, IServerFarmGateway
    {
        public ServerFarmGateway(IOathGateway oAuthGateway)
            :base(oAuthGateway)
        {

        }

        public string GetServerFarmData(Uri serverFarmUri)
        {
            return GetResourceAsString(serverFarmUri).Result;
        }
    }
}
