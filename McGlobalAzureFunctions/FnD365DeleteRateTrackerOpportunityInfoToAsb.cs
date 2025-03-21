// <copyright file="FnD365DeleteRateTrackerOpportunityInfoToAsb.cs" company="moneycorp">
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

    public class FnD365DeleteRateTrackerOpportunityInfoToAsb
    {
        private readonly ILogger<FnD365DeleteRateTrackerOpportunityInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        public FnD365DeleteRateTrackerOpportunityInfoToAsb(ILogger<FnD365DeleteRateTrackerOpportunityInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
        }

        [Function(nameof(FnD365DeleteRateTrackerOpportunityInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            this._logger.LogInformation("FnD365DeleteRateTrackerOpportunityInfoToAsb trigger started");
            bool isSuccess = false;
            string errMsg = string.Empty;
            DeleteRateTrackerOpportunityRequest deleteRateTrackerOpportunityRequest = new DeleteRateTrackerOpportunityRequest();

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                    bool isProd = FnCommon.GetIsProd();

                    this._logger.LogInformation("Preparing Request Data Start");
                    deleteRateTrackerOpportunityRequest = DeleteRateTrackerOpportunityConversion.GetDeleteRateTrackerOpportunityRequest(contextEntity, _crmServiceClient, _logger, out errMsg);
                    this._logger.LogInformation("Preparing Request Data End");

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var reqData = JsonConvert.SerializeObject(deleteRateTrackerOpportunityRequest);
                        this._logger.LogInformation(reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(reqData, nameof(DeleteRateTrackerOpportunityRequest));

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            this._logger.LogInformation("DeleteRateTrackerOpportunityRequest trigger complete");
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

                this._logger.LogInformation("FnD365DeleteRateTrackerOpportunityInfoToAsb trigger complete");
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