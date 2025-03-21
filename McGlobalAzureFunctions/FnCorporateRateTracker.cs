// <copyright file="FnCorporateRateTracker.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Azure.Messaging.ServiceBus;
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// FnCorporateRateTracker class.
    /// </summary>
    public class FnCorporateRateTracker
    {
        private readonly ILogger<FnCorporateRateTracker> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IRateProvider _rateProvider;

        /// <summary>
        /// FnCorporateRateTracker Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">svc client</param>
        /// <param name="rateProvider">rateProvider</param>
        public FnCorporateRateTracker(ILogger<FnCorporateRateTracker> logger, IOrganizationServiceAsync2 crmServiceClient, IRateProvider rateProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._rateProvider = rateProvider;
        }

        /// <summary>
        /// Function Run Method.
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="messageActions">messageActions</param>
        /// <returns>Task result</returns>
        [Function(nameof(FnCorporateRateTracker))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporateratetracker", Connection = "AsbQueueEndPoint")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation($"FnCorporateRateTracker Message Processing Start");

            AsbMessageInfo msgProps = FnCommon.GetMessageProperties(message);

            if (msgProps.CanProcess)
            {
                try
                {
                    string errMsg = string.Empty;
                    DynTaskResponse taskResp = new DynTaskResponse();
                    taskResp.IsSuccess = false;
                    bool canProcess = false;
                    bool isProd = FnCommon.GetIsProd();

                    switch (msgProps.MessageType)
                    {
                        case nameof(D365RateTrackerOpportunityInfo):
                            D365RateTrackerOpportunityInfo rateTrackResp = FnCommon.DeSerializeServiceBusMessage<D365RateTrackerOpportunityInfo>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                var reqData = JsonConvert.SerializeObject(rateTrackResp);
                                _logger.LogInformation(reqData);
                                taskResp = _rateProvider.ProcessRateTrackerOptyInfo(msgProps.RecordId, rateTrackResp, msgProps.RequestId, _crmServiceClient, isProd,_logger);
                            }
                            break;
                        case nameof(RateTrackerNotification):
                            RateTrackerNotification rateNotifyResp = FnCommon.DeSerializeServiceBusMessage<RateTrackerNotification>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                var reqData = JsonConvert.SerializeObject(rateNotifyResp);
                                _logger.LogInformation(reqData);
                                taskResp = _rateProvider.ProcessRateTrackerNotification(rateNotifyResp, msgProps.RequestId, _crmServiceClient, isProd, _logger);
                            }
                            break;
                        case nameof(DeleteRateTrackerOpportunityInfo):
                            DeleteRateTrackerOpportunityInfo delOptyResp = FnCommon.DeSerializeServiceBusMessage<DeleteRateTrackerOpportunityInfo>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                var reqData = JsonConvert.SerializeObject(delOptyResp);
                                _logger.LogInformation(reqData);
                                taskResp = _rateProvider.ProcessRateTrackerDeletion(delOptyResp, msgProps.RequestId, _crmServiceClient, _logger);
                            }
                            break;
                    }

                    _logger.LogInformation($"FnCorporateRateTracker Message Processing end: "+ msgProps.MessageType);

                    if (taskResp.IsSuccess)
                    {
                        await messageActions.CompleteMessageAsync(message);
                    }
                    else
                    {
                        _logger.LogError("Failed to handle asp response:" + taskResp.ErrorMessage);
                        await messageActions.DeadLetterMessageAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Processing Response");
                }
            }

            _logger.LogInformation($"FnCorporateRateTracker Message Processing end");
        }
    }
}
