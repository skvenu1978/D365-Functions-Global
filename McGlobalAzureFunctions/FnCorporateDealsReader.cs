// <copyright file="FnCorporateDealsReader.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Azure.Messaging.ServiceBus;
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.OmniResponse;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// FnCorporateDealsReader class.
    /// </summary>
    public class FnCorporateDealsReader
    {
        private readonly ILogger<FnCorporateDealsReader> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IOptyRdrProvider _optyReaderProvider;
        private readonly IRateProvider _rateProvider;

        /// <summary>
        /// FnCorporateDealsReader Constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">svc client</param>
        /// <param name="optyReaderProvider">optyReaderProvider</param>
        /// <param name="rateProvider">rateProvider</param>
        public FnCorporateDealsReader(ILogger<FnCorporateDealsReader> logger, IOrganizationServiceAsync2 crmServiceClient, IOptyRdrProvider optyReaderProvider, IRateProvider rateProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._optyReaderProvider = optyReaderProvider;
            this._rateProvider = rateProvider;
        }

        /// <summary>
        /// Function Run Method.
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="messageActions">messageActions</param>
        /// <returns>Task result</returns>
        [Function(nameof(FnCorporateDealsReader))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporatedeals", Connection = "AsbQueueEndPoint")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            AsbMessageInfo msgProps = FnCommon.GetMessageProperties(message);

            if (msgProps.CanProcess)
            {
                try
                {
                    string errMsg = string.Empty;
                    _logger.LogInformation($"Message Processing Start");
                    DynTaskResponse taskResp = new DynTaskResponse();
                    taskResp.IsSuccess = false;
                    bool canProcess = false;
                    bool isProd = FnCommon.GetIsProd();

                    switch (msgProps.MessageType)
                    {
                        case nameof(DealSummary):
                            DealSummary dealSumResp = FnCommon.DeSerializeServiceBusMessage<DealSummary>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                var reqData = JsonConvert.SerializeObject(dealSumResp);
                                _logger.LogInformation(reqData);
                                taskResp = _optyReaderProvider.ProcessDealSummary(dealSumResp, msgProps.RequestId, _crmServiceClient, _logger);
                            }
                            break;
                        case nameof(PlanSummary):
                            PlanSummary planSumResp = FnCommon.DeSerializeServiceBusMessage<PlanSummary>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                var reqData = JsonConvert.SerializeObject(planSumResp);
                                _logger.LogInformation(reqData);
                                taskResp = _optyReaderProvider.ProcessPlanSummary(planSumResp, msgProps.RequestId, _crmServiceClient, _logger);
                            }
                            break;
                    }

                    _logger.LogInformation($"Message Processing end");

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
        }
    }
}
