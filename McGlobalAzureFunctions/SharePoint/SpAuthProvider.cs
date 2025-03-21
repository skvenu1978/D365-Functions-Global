// <copyright file="SpAuthProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.SharePoint
{
    using Azure.Identity;
    using Microsoft.Graph;

    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Models.Common;

    public class SpAuthProvider
    {
        public static GraphServiceClient GetGraphService(SpCredentials creds)
        {
            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var options = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };

            var clientSecretCredential = new ClientSecretCredential(SharePointConstants.SpTenantId, creds.ClientId, creds.ClientSecret, options);
            return new GraphServiceClient(clientSecretCredential, scopes);
        }
    }
}