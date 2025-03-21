// <copyright file="FnD365AlertInfoToAsb.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Requests;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Utilities;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;

    /// <summary>
    /// FnD365AlertInfoToAsb class.
    /// </summary>
    public class FnD365AlertInfoToAsb
    {
        private readonly ILogger<FnD365AlertInfoToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IAlertProvider _alertProvider;

        /// <summary>
        /// FnD365AlertInfoToAsb constuctor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        /// <param name="alertProvider">alertProvider</param>
        public FnD365AlertInfoToAsb(ILogger<FnD365AlertInfoToAsb> logger, IOrganizationServiceAsync2 crmServiceClient, IAlertProvider alertProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._alertProvider = alertProvider;
        }

        /// <summary>
        /// FnD365AlertInfoToAsb Run Function.
        /// </summary>
        /// <param name="req">http request</param>
        /// <returns>IActionResult result</returns>
        [Function(nameof(FnD365AlertInfoToAsb))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            Protect.ForNull(req.Body, nameof(req));
            _logger.LogInformation(nameof(FnD365AlertInfoToAsb) +" trigger started");
            string errMsg = string.Empty;
            bool isSuccess = false;
            AlertInfoRequest alertRequest = new AlertInfoRequest();

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                Guid? accountId = Validators.GetAccountIdFromContextAndCheckEntity(contextEntity, AlertConstants.Mcorp_alert, _crmServiceClient);

                if (accountId.HasValue)
                {
                    _logger.LogInformation(LogConstants.reqPrepStart);
                    alertRequest = _alertProvider.GetAlertRequest(accountId.Value, _crmServiceClient, _logger, out errMsg);
                    _logger.LogInformation(LogConstants.reqPrepEnd);

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        _logger.LogInformation(LogConstants.serStart);
                        var reqData = JsonConvert.SerializeObject(alertRequest);
                        _logger.LogInformation(reqData);
                        _logger.LogInformation(LogConstants.serEnd);

                        _logger.LogInformation(LogConstants.asbMsgStart);
                        DynTaskResponse asbTask = FnCommon.SendMessageASB(reqData, nameof(AlertInfoRequest));
                        _logger.LogInformation(LogConstants.asbMsgEnd);

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
                            _logger.LogInformation(nameof(FnD365AlertInfoToAsb) + " trigger complete");
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
                    _logger.LogError("Context Entity Does not match or required attribute is missing");
                }
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365AlertInfoToAsb) + "Error:" + ex.Message;
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