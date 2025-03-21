namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.OmniResponse;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;

    /// <summary>
    /// FnD365RateTrackerOpportunityRequestToAsb Class.
    /// </summary>
    public class FnD365RateTrackerOpportunityRequestToAsb
    {
        private readonly ILogger<FnD365RateTrackerOpportunityRequestToAsb> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IRateProvider _rateProvider;

        /// <summary>
        /// FnD365RateTrackerOpportunityRequestToAsb Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        /// <param name="rateProvider">rateProvider</param>
        public FnD365RateTrackerOpportunityRequestToAsb(ILogger<FnD365RateTrackerOpportunityRequestToAsb> logger, IOrganizationServiceAsync2 crmServiceClient, IRateProvider rateProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._rateProvider = rateProvider;
        }

        /// <summary>
        /// FnD365RateTrackerOpportunityRequestToAsb Run.
        /// </summary>
        /// <param name="req">http req</param>
        /// <returns>IActionResult</returns>
        [Function("FnD365RateTrackerOpportunityRequestToAsb")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            this._logger.LogInformation("FnD365RateTrackerOpportunityRequestToAsb Function started");

            D365RateTrackerOpportunityRequest rateTrackerOpportunityRequest = new D365RateTrackerOpportunityRequest();
            bool isSuccess = false;
            string errMsg = string.Empty;

            try
            {
                if (req.Body != null)
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    RemoteExecutionContext remoteExecContext = FnCommon.GetRemoteExecContext(requestBody);
                    bool isCreate = false;

                    if (remoteExecContext != null)
                    {
                        string messageName = remoteExecContext.MessageName;

                        if (messageName == "Create")
                        {
                            isCreate = true;
                        }
                    }

                    var remoteExecContextString = JsonConvert.SerializeObject(remoteExecContext);
                    this._logger.LogInformation("RemoteExecContext JSON:" + remoteExecContextString);
                    Entity contextEntity = FnCommon.GetEntityFromRequest(requestBody);
                    this._logger.LogInformation("Preparing Request Data Start");
                    rateTrackerOpportunityRequest = _rateProvider.GetRateTrackerOpportunityRequest(contextEntity, isCreate, _crmServiceClient, _logger, out errMsg);
                    this._logger.LogInformation("Preparing Request Data End");

                    if (string.IsNullOrEmpty(errMsg))
                    {
                        var _reqData = JsonConvert.SerializeObject(rateTrackerOpportunityRequest);
                        this._logger.LogInformation(_reqData);

                        DynTaskResponse asbTask = FnCommon.SendMessageASB(_reqData, nameof(D365RateTrackerOpportunityRequest));

                        if (asbTask.IsSuccess)
                        {
                            isSuccess = true;
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
            }
            catch (Exception ex)
            {
                errMsg = "Error in function: " + nameof(FnD365RateTrackerOpportunityRequestToAsb) + "Error:" + ex.Message;
                this._logger.LogError(ex, errMsg);
            }

            this._logger.LogInformation("FnD365RateTrackerOpportunityRequestToAsb Function started");

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
