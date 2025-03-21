// <copyright file="FindMFRMContactDetails.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Fox;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Fox;
    using McGlobalAzureFunctions.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// FindMFRMContactDetails class
    /// </summary>
    public class FindMFRMContactDetails
    {
        private readonly ILogger<FindMFRMContactDetails> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IFoxProvider _foxReader;

        /// <summary>
        /// FindMFRMContactDetails Constructor
        /// </summary>
        /// <param name="logger">ilogger object</param>
        /// <param name="crmServiceClient">D365 Service Client</param>
        /// <param name="foxReader">Fox Interface</param>
        public FindMFRMContactDetails(ILogger<FindMFRMContactDetails> logger, IOrganizationServiceAsync2 crmServiceClient, IFoxProvider foxReader)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(foxReader, nameof(foxReader));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._foxReader = foxReader;
        }

        /// <summary>
        /// FindMFRMContactDetails Run Method
        /// </summary>
        /// <param name="req">Http req</param>
        /// <param name="accountId">D365 Account id</param>
        /// <param name="contactIds">D365 Contact Ids List</param>
        /// <param name="executionContext">fn execution context</param>
        /// <returns></returns>
        [Function(nameof(FindMFRMContactDetails))]
        public ContentResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mfrmcontactfulldetails")] HttpRequestData req, [FromQuery] string accountId, [FromQuery] string contactIds, FunctionContext executionContext)
        {
            _logger.LogDebug("FindMFRMContactDetails function triggered");
            string errMsg = string.Empty;
            List<string> cIds = ConvertCsvToList(contactIds);
            _logger.LogDebug("Searching in D365 start");
            AccountContactDetailedSearchResultMc resultMc = _foxReader.GetAccountRelatedContacts(accountId, cIds, _crmServiceClient, out errMsg);
            _logger.LogDebug("Searching in D365 end");

            if (!string.IsNullOrEmpty(errMsg))
            {
                _logger.LogError(errMsg);

                return new ContentResult()
                {
                    Content = "Error in processing the function:" + errMsg,
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }
            else
            {
                _logger.LogDebug($"FindMFRMContactDetails function exited successfully");
               
                return new ContentResult()
                {
                    Content = JsonConvert.SerializeObject(resultMc),
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 200
                };
            }
        }

        /// <summary>
        /// ConvertCsvToList.
        /// </summary>
        /// <param name="contactIds">contactIds</param>
        /// <returns>string list</returns>
        private static List<string> ConvertCsvToList(string contactIds)
        {
            List<string> cIds = new List<string>();

            if (contactIds.Contains(","))
            {
                string[] ids = contactIds.Split(',');

                foreach (string id in ids)
                {
                    bool isGuid = Validators.IsValidGuid(id);
                    cIds.Add(id);
                }
            }
            else
            {
                cIds.Add(contactIds);
            }

            return cIds;
        }
    }
}
