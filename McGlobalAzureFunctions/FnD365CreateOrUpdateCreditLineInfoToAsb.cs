// <copyright file="FnD365CreateOrUpdateCreditLineInfoToAsb.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure Function to
    /// Send Credit Line to OMNI.
    /// </summary>
    public class FnD365CreateOrUpdateCreditLineInfoToAsb
    {
        private readonly ILogger<FnD365CreateOrUpdateCreditLineInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        /// <summary>
        /// Constructor for 
        /// FnD365CreateOrUpdateCreditLineInfoToAsb.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="crmServiceClient">Svc Client</param>
        public FnD365CreateOrUpdateCreditLineInfoToAsb(ILogger<FnD365CreateOrUpdateCreditLineInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
        }

        /// <summary>
        /// Main function that execute on the trigger
        /// webhook from D365.
        /// </summary>
        /// <param name="req">Http req</param>
        /// <returns>action result</returns>
        [Function("FnD365CreateOrUpdateCreditLineInfoToAsb")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("FnD365CreateOrUpdateCreditLineInfoToAsb trigger started");
            bool isSuccess = false;
            string errMsg = string.Empty;

            D365CreateOrUpdateCreditLineRequest createOrUpdateCreditLineRequest = new D365CreateOrUpdateCreditLineRequest();

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    string pluginMessageName = string.Empty;

                    Entity contextEntity = FnCommon.GetEntityFromRequestWithMessageType(requestBody, out pluginMessageName);
                    bool isProd = FnCommon.GetIsProd();

                    bool isMessageValid = false;

                    _logger.LogInformation("Preparing Request Data Start");
                    (createOrUpdateCreditLineRequest,isMessageValid) = CreateOrUpdateCreditLineConversion.GetCreateOrUpdateCreditLineRequest(contextEntity, _crmServiceClient, out errMsg);

                    if (isMessageValid)
                    {
                        _logger.LogInformation("Preparing Request Data End");

                        if (string.IsNullOrEmpty(errMsg))
                        {
                            var reqData = JsonConvert.SerializeObject(createOrUpdateCreditLineRequest);
                            _logger.LogInformation(reqData);

                            DynTaskResponse asbTask = FnCommon.SendMessageASB(reqData, nameof(D365CreateOrUpdateCreditLineRequest));

                            if (asbTask.IsSuccess)
                            {
                                isSuccess = true;
                                _logger.LogInformation("D365CreateOrUpdateCreditLineRequest trigger complete");
                            }
                            else
                            {
                                _logger.LogError(asbTask.Exception, "Error in sending to ASB: " + asbTask.ErrorMessage);
                            }
                        }
                        else
                        {
                            _logger.LogError("Request is not valid to be sent to OMNI or error preparing request: " + errMsg);
                        }
                    }
                    else
                    {
                        _logger.LogInformation(errMsg);
                    }
                }
                
                _logger.LogInformation("FnD365CreateOrUpdateCreditLineInfoToAsb trigger complete");
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365CreateOrUpdateCreditLineInfoToAsb) + "Error:" + ex.Message;
                _logger.LogError(ex, errMsg);
            }

            if (isSuccess)
            {
                return new OkObjectResult("Completed successfully");
            }
            else
            {
                return new BadRequestObjectResult(errMsg);
            }
        }
    }
}