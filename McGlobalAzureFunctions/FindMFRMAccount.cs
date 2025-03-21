// <copyright file="FindMFRMAccount.cs" company="moneycorp">
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
    /// FindMFRMAccount Class.
    /// </summary>
    public class FindMFRMAccount
    {
        private readonly ILogger _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IFoxProvider _foxReader;

        /// <summary>
        /// FindMFRMAccount Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">svc client</param>
        /// <param name="foxReader">fox interface</param>
        public FindMFRMAccount(ILogger<FindMFRMAccount> logger, IOrganizationServiceAsync2 crmServiceClient, IFoxProvider foxReader)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(foxReader, nameof(foxReader));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._foxReader = foxReader;
        }

        /// <summary>
        /// FindMFRMAccount Run Method.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="nameCriteria"></param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        [Function(nameof(FindMFRMAccount))]
        public ContentResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mfrmaccount")] HttpRequestData req, [FromQuery] string nameCriteria, FunctionContext executionContext)
        {
            Protect.ForNullOrWhiteSpace(nameCriteria, "nameCriteria");

            this._logger.LogTrace("FindMFRMAccount function triggered");
            this._logger.LogTrace($"Search name:{0}", nameCriteria);

            string errMsg = string.Empty;
            this._logger.LogTrace("Searching in D365 start");
            List<AccountSearchResultMc> resultMcs = _foxReader.SearchMFRMAccount(nameCriteria, null, _crmServiceClient, out errMsg);
            this._logger.LogTrace("Searching in D365 end");

            if (!string.IsNullOrEmpty(errMsg))
            {
                this._logger.LogError(errMsg);
                return new ContentResult()
                {
                    Content = "Unable to search for account:" + errMsg,
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }
            else
            {
                this._logger.LogTrace($"FindMFRMAccount function exited with {0} rows", resultMcs.Count);
                this._logger.LogTrace(JsonConvert.SerializeObject(resultMcs));

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