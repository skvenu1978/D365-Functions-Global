// <copyright file="FnD365DeleteCreditTierInOmniInfoToAsb.cs" company="moneycorp">
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
    /// FnD365DeleteCreditTierInOmniInfoToAsb class.
    /// </summary>
    public class FnD365DeleteCreditTierInOmniInfoToAsb
    {
        private readonly ILogger<FnD365DeleteCreditTierInOmniInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        /// <summary>
        /// FnD365DeleteCreditTierInOmniInfoToAsb Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        public FnD365DeleteCreditTierInOmniInfoToAsb(ILogger<FnD365DeleteCreditTierInOmniInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
        }

        /// <summary>
        /// FnD365DeleteCreditTierInOmniInfoToAsb Run.
        /// </summary>
        /// <param name="req">http req</param>
        /// <returns>IActionResult</returns>
        [Function(nameof(FnD365DeleteCreditTierInOmniInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            this._logger.LogInformation("FnD365DeleteCreditTierInOmniInfoToAsb trigger started");
            DeleteCreditTiersRequest deleteCreditTiersRequest = new DeleteCreditTiersRequest();
            bool isSuccess = false;
            string errMsg = string.Empty;

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    EntityReference contextEntity = FnCommon.GetEntityReferenceFromRequest(requestBody);
                    bool isProd = FnCommon.GetIsProd();

                    this._logger.LogInformation("Preparing Request Data Start");
                    deleteCreditTiersRequest = DeleteCreditTiersConversion.GetDeleteCreditTiersRequest(contextEntity, _logger, out errMsg);
                    this._logger.LogInformation("Preparing Request Data End");

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var _reqData = JsonConvert.SerializeObject(deleteCreditTiersRequest);
                        this._logger.LogInformation(_reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(_reqData, nameof(DeleteCreditTiersRequest));

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            this._logger.LogInformation("DeleteCreditTiersRequest trigger complete");
                        }
                        else
                        {
                            this._logger.LogError(asbTask.Exception, "Error in sending to ASB: " + asbTask.ErrorMessage);
                        }
                    }
                    else
                    {
                        this._logger.LogError("Request is not valid to be sent to OMNI or error preparing request: " + errMsg);
                    }
                }

                this._logger.LogInformation("FnD365DeleteCreditTierInOmniInfoToAsb trigger complete");
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365DeleteCreditTierInOmniInfoToAsb) + "Error:" + ex.Message;
                this._logger.LogError(ex, errMsg);
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