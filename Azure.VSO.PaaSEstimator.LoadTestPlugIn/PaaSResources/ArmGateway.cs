using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class ArmGateway
    {
        public IOathGateway OAuthGateway { get; set; }

        public ArmGateway(IOathGateway oAuthGateway)
        {
            this.OAuthGateway = oAuthGateway;
        }

        public async Task<string> GetResourceAsString(string resource)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.OAuthGateway.GetOAuthToken());
            HttpResponseMessage responseMessage = await httpClient.GetAsync(resource);

            string s = await responseMessage.Content.ReadAsStringAsync();
            return s;
        }
    }
}
