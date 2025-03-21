// <copyright file="FnD365ClientContactInfoToAsb.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Const;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;

    /// <summary>
    /// FnD365ClientContactInfoToAsb class.
    /// </summary>
    public class FnD365ClientContactInfoToAsb
    {
        private readonly ILogger<FnD365ClientContactInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;

        /// <summary>
        /// FnD365ClientContactInfoToAsb constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        public FnD365ClientContactInfoToAsb(ILogger<FnD365ClientContactInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
        }

        /// <summary>
        /// FnD365ClientContactInfoToAsb Run function.
        /// </summary>
        /// <param name="req">http req</param>
        /// <returns>IActionResult</returns>
        [Function(nameof(FnD365ClientContactInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            this._logger.LogInformation(nameof(FnD365ClientContactInfoToAsb) +" trigger started");
            bool isSuccess = false;
            string errMsg = string.Empty;

            ClientContactInfoRequest clientContactRequest = new ClientContactInfoRequest();

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                    RemoteExecutionContext remoteContext = FnCommon.GetRemoteExecContext(requestBody);
                    bool isContactBeingInactivated = false;
                    bool isContactTrigger = false;

                    if (remoteContext.PrimaryEntityName == "contact")
                    {
                        isContactTrigger = true;
                        int contactStateCode = AttributeHelper.GetStateStatusOptionSetValue(contextEntity, "statecode");

                        if (contactStateCode == 1)
                        {
                            isContactBeingInactivated = true;
                        }
                    }
                    
                    bool isProd = FnCommon.GetIsProd();
                    this._logger.LogInformation("Context Entity name:" + contextEntity.LogicalName);

                    this._logger.LogInformation("Preparing Request Data Start");
                    clientContactRequest = ObjConversion.GetClientInfoRequest(contextEntity, _crmServiceClient, this._logger, isContactBeingInactivated, isContactTrigger, out errMsg);
                    this._logger.LogInformation("Preparing Request Data End");

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var reqData = JsonConvert.SerializeObject(clientContactRequest);
                        this._logger.LogInformation(reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(reqData, nameof(ClientContactInfoRequest));

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            this._logger.LogInformation("ClientContactInfoRequest trigger complete");
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

                this._logger.LogInformation("FnD365ClientContactInfoToAsb trigger complete");
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365ClientContactInfoToAsb) + "Error:" + ex.Message;
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