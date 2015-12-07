using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class ArmResourceGateway : IResourceGateway
    {
        public IOathGateway OAuthGateway { get; set; }

        public ArmResourceGateway(IOathGateway oAuthGateway)
        {
            this.OAuthGateway = oAuthGateway;
        }

        //public async Task<string> GetResourceAsString(string resource)
        //{
        //    var httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.OAuthGateway.GetOAuthToken());
        //    HttpResponseMessage responseMessage = await httpClient.GetAsync(resource);

        //    string s = await responseMessage.Content.ReadAsStringAsync();
        //    return s;
        //}

        public async Task<string> GetResourceAsString(Uri resource)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.OAuthGateway.GetOAuthToken());

            HttpResponseMessage responseMessage = await httpClient.GetAsync(resource);

            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}
