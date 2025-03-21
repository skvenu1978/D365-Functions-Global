// <copyright file="FindMFRMSample.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Fox;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Fox;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// FindMFRMSample Sample
    /// Class
    /// </summary>
    public class FindMFRMSample
    {
        private readonly ILogger<FindMFRMSample> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IFoxProvider _foxReader;

        /// <summary>
        /// FindMFRMSample Constructor
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crm service client</param>
        /// <param name="foxReader">fox interface</param>
        public FindMFRMSample(ILogger<FindMFRMSample> logger, IOrganizationServiceAsync2 crmServiceClient, IFoxProvider foxReader)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._foxReader = foxReader;
        }

        /// <summary>
        /// FindMFRMSample Run Function
        /// </summary>
        /// <param name="req">http request</param>
        /// <param name="nameCriteria">search name</param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        [Function(nameof(FindMFRMSample))]
        public ContentResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sample/{nameCriteria}")] HttpRequestData req, string nameCriteria, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("FindMFRMSample");
            logger.LogInformation("FindMFRMSample triggered");
            logger.LogInformation($"FindMFRMSample search name:{0}",nameCriteria);
            AccountSearchResultMc data = _foxReader.GetSampleMFRMAccountData(nameCriteria);
            AccountSearchResultMc data2 = _foxReader.GetSampleMFRMAccountData(nameCriteria);
            List<AccountSearchResultMc> resultMcs = new List<AccountSearchResultMc>();
            resultMcs.Add(data);
            resultMcs.Add(data2);
            logger.LogInformation("FindMFRMSample complete");
            return new ContentResult() { Content = JsonConvert.SerializeObject(resultMcs),ContentType = "application/json; charset=UTF-8", StatusCode = 200 };
        }
    }
}