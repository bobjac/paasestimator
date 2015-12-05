using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Azure.VSO.PaaSEstimator.LoadTestPlugIn.PaaSResources
{
    public class AzureADOAuthGateway : IOathGateway
    {
        public string AuthenticationAuthority { get; set; }
        public string ClientId { get; set; }
        public string Key { get; set; }
        public string Resource { get; set; }
        public string TenantId { get; set; }
        
        public AzureADOAuthGateway(
            string authenticationAuthority, 
            string clientId, 
            string key, 
            string resource,
            string tenantId)
        {
            this.AuthenticationAuthority = authenticationAuthority;
            this.ClientId = clientId;
            this.Key = key;
            this.Resource = resource;
            this.TenantId = tenantId;
        }

        public AzureADOAuthGateway()
        {
            this.AuthenticationAuthority = "https://login.windows.net";
            this.Resource = "https://management.core.windows.net/";
        }

        public string GetOAuthToken()
        {
            var authenticationContext = new AuthenticationContext(
                string.Format("{0}/{1}", this.AuthenticationAuthority, 
                this.TenantId));

            var clientCredential = new ClientCredential(this.ClientId, this.Key);

            var result = authenticationContext.AcquireToken(this.Resource, clientCredential);

            return result.AccessToken;
        }
    }
}
