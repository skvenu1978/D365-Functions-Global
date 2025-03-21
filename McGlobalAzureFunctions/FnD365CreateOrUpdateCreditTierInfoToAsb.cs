// <copyright file="FnD365CreateOrUpdateCreditTierInfoToAsb.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Requests;
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
    /// FnD365CreateOrUpdateCreditTierInfoToAsb Class.
    /// </summary>
    public class FnD365CreateOrUpdateCreditTierInfoToAsb
    {
        private readonly ILogger<FnD365CreateOrUpdateCreditTierInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly ICreditTierProvider _creditTierProvider;

        /// <summary>
        /// FnD365CreateOrUpdateCreditTierInfoToAsb Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        /// <param name="creditTierProvider">creditTierProvider</param>
        public FnD365CreateOrUpdateCreditTierInfoToAsb(ILogger<FnD365CreateOrUpdateCreditTierInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient, ICreditTierProvider creditTierProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._creditTierProvider = creditTierProvider;
        }

        /// <summary>
        /// FnD365CreateOrUpdateCreditTierInfoToAsb Run.
        /// </summary>
        /// <param name="req">http req</param>
        /// <returns>IActionResult</returns>
        [Function(nameof(FnD365CreateOrUpdateCreditTierInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            this._logger.LogInformation("FnD365CreateOrUpdateCreditTierInfoToAsb trigger started");
            bool isSuccess = false;
            string errMsg = string.Empty;
            D365CreateOrUpdateCreditTierRequest createOrUpdateCreditTierRequest = new D365CreateOrUpdateCreditTierRequest();

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                    bool isProd = FnCommon.GetIsProd();
                    bool isUpdateFromOMNI = false;

                    var remJson = FnCommon.GetRemoteExecContext(requestBody);

                    if (contextEntity.Contains("mcorp_isupdatefromomni"))
                    {
                        isUpdateFromOMNI = AttributeHelper.GetBooleanValueDefaultFalse(contextEntity, "mcorp_isupdatefromomni");
                    }

                    var remExecData = JsonConvert.SerializeObject(remJson);
                    this._logger.LogInformation("Remote Context Data:" + remExecData);

                    this._logger.LogInformation("Preparing Request Data Start:"+ contextEntity.Id.ToString());
                    createOrUpdateCreditTierRequest = _creditTierProvider.GetCreateOrUpdateCreditTierRequest(contextEntity, _crmServiceClient, isUpdateFromOMNI, isProd, out errMsg);
                    this._logger.LogInformation("Preparing Request Data End:" + contextEntity.Id.ToString());

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var _reqData = JsonConvert.SerializeObject(createOrUpdateCreditTierRequest);
                        this._logger.LogInformation(_reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(_reqData, nameof(D365CreateOrUpdateCreditTierRequest));

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            this._logger.LogInformation("D365CreateOrUpdateCreditTierRequest trigger complete");
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

                this._logger.LogInformation("FnD365CreateOrUpdateCreditTierInfoToAsb trigger complete");
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365CreateOrUpdateCreditTierInfoToAsb) + "Error:" + ex.Message;
                _logger.LogError(ex, errMsg);
            }

            if (isSuccess)
                return new OkObjectResult("Completed successfully");
            else
                return new BadRequestObjectResult(errMsg);
        }
    }
}