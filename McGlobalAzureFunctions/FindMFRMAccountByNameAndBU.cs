// <copyright file="FindMFRMAccountByNameAndBU.cs" company="moneycorp">
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
    /// FindMFRMAccountByNameAndBU class.
    /// </summary>
    public class FindMFRMAccountByNameAndBU
    {
        private readonly ILogger<FindMFRMAccountByNameAndBU> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IFoxProvider _foxReader;

        /// <summary>
        /// FindMFRMAccountByNameAndBU constructor.
        /// </summary>
        /// <param name="logr">logger object.</param>
        /// <param name="crmSvcClient">svc client.</param>
        /// <param name="foxRdr">fox interface.</param>
        public FindMFRMAccountByNameAndBU(ILogger<FindMFRMAccountByNameAndBU> logr, IOrganizationServiceAsync2 crmSvcClient, IFoxProvider foxRdr)
        {
            Protect.ForNull(logr, nameof(logr));
            Protect.ForNull(crmSvcClient, nameof(crmSvcClient));
            this._logger = logr;
            this._crmServiceClient = crmSvcClient;
            this._foxReader = foxRdr;
        }

        /// <summary>
        /// FindMFRMAccountByNameAndBU Run method.
        /// </summary>
        /// <param name="req">req</param>
        /// <param name="nameCriteria">nameCriteria</param>
        /// <param name="owningBusiness">owningBusiness</param>
        /// <param name="executionContext">executionContext</param>
        /// <returns>ContentResult</returns>
        [Function(nameof(FindMFRMAccountByNameAndBU))]
        public ContentResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mfrmaccountbynameandowningbu")] HttpRequestData req, [FromQuery] string nameCriteria, [FromQuery] int owningBusiness, FunctionContext executionContext)
        {
            Protect.ForNullOrWhiteSpace(nameCriteria, "nameCriteria");
            Protect.ForNegative(owningBusiness, "owningBusiness");

            var logger = executionContext.GetLogger("FindMFRMAccountByNameAndBU");
            logger.LogDebug("FindMFRMAccountByNameAndBU function triggered");
            string errMsg = string.Empty;
            logger.LogDebug("Searching in D365 start");
            List<AccountSearchResultMc> resultMcs = _foxReader.SearchMFRMAccount(nameCriteria, owningBusiness, _crmServiceClient, out errMsg);
            logger.LogDebug("Searching in D365 end");

            if (!string.IsNullOrEmpty(errMsg))
            {
                logger.LogError(errMsg);

                return new ContentResult()
                {
                    Content = "Unable to connect to D365:" + errMsg,
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }
            else
            {
                logger.LogDebug($"FindMFRMAccountByNameAndBU function exited with {0} rows", resultMcs.Count);
                logger.LogDebug(JsonConvert.SerializeObject(resultMcs));

                return new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(resultMcs),
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 200
                };
            }
        }
    }
}