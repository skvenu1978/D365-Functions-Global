// <copyright file="HealthCheck.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Gate;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// HealthCheck class
    /// </summary>
    public class HealthCheck
    {
        private readonly ILogger<HealthCheck> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        /// <summary>
        /// HealthCheck constructor
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        public HealthCheck(ILogger<HealthCheck> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
        }

        /// <summary>
        /// HealthCheck Run function
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("HealthCheck")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            Guid? userId = null;

            try
            {
                WhoAmIRequest request = new WhoAmIRequest();
                WhoAmIResponse response = (WhoAmIResponse)_crmServiceClient.Execute(request);

                userId = response.UserId;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, "IOrg Service Client failed" + ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError(ex, "IOrg Service Client failed" + ex.Message);
                }
            }

            if (userId.HasValue)
            {
                _logger.LogInformation($"D365 Global Integration Function healthy!. Service authentication successful");
            }
            else
            {
                _logger.LogError($"D365 Global Integration Function not healthy!. Issue in connection string");
            }

            return new OkObjectResult("HealthCheck successful!");
        }
    }
}
