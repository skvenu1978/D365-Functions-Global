// <copyright file="FindMFRMContacts.cs" company="moneycorp">
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
    /// FindMFRMContacts class
    /// </summary>
    public class FindMFRMContacts
    {
        private readonly ILogger<FindMFRMContacts> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IFoxProvider _foxReader;

        /// <summary>
        /// FindMFRMContacts Constructor
        /// </summary>
        /// <param name="logger">ilogger object</param>
        /// <param name="crmServiceClient">D365 Service Client</param>
        /// <param name="foxReader">Fox Interface</param>
        public FindMFRMContacts(ILogger<FindMFRMContacts> logger, IOrganizationServiceAsync2 crmServiceClient, IFoxProvider foxReader)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(foxReader, nameof(foxReader));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._foxReader = foxReader;
        }

        /// <summary>
        /// FindMFRMContacts Run Method
        /// </summary>
        /// <param name="req">http req</param>
        /// <param name="accountid">D365 account Id</param>
        /// <param name="executionContext">Fn Exec Context</param>
        /// <returns></returns>
        [Function(nameof(FindMFRMContacts))]
        public ContentResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mfrmcontact/{accountid}")] HttpRequestData req, string accountid, FunctionContext executionContext)
        {
            Protect.ForNullOrWhiteSpace(accountid, nameof(accountid));
            var logger = executionContext.GetLogger("FindMFRMContacts");
            _logger.LogDebug("FindMFRMContacts function triggered");
            string errMsg = string.Empty;
            _logger.LogDebug("Searching in D365 start");
            List<ContactSearchResultMc> resultMcs = _foxReader.GetLinkedAuthorisedContacts(accountid, _crmServiceClient, out errMsg);
            _logger.LogDebug("Searching in D365 end");

            if (!string.IsNullOrEmpty(errMsg))
            {
                _logger.LogError("Unable to connect to D365:" + errMsg);

                return new ContentResult()
                {
                    Content = "Unable to connect to D365:" + errMsg,
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }
            else
            {
                _logger.LogDebug($"FindMFRMContacts function exited with {0} rows", resultMcs.Count);
                _logger.LogDebug(JsonConvert.SerializeObject(resultMcs));

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