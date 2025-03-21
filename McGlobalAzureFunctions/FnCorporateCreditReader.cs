// <copyright file="FnCorporateCreditReader.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Azure.Messaging.ServiceBus;
    using McGlobalAzureFunctions.Abstractions.Requests;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.OmniResponse;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// FnCorporateCreditReader
    /// </summary>
    public class FnCorporateCreditReader
    {
        private readonly ILogger<FnCorporateCreditReader> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly ICreditTierProvider _creditTierProvider;

        /// <summary>
        /// FnCorporateCreditReader constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        /// <param name="creditTierProvider">creditTierProvider</param>
        public FnCorporateCreditReader(ILogger<FnCorporateCreditReader> logger, IOrganizationServiceAsync2 crmServiceClient, ICreditTierProvider creditTierProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._creditTierProvider = creditTierProvider;
        }

        /// <summary>
        /// FnCorporateCreditReader Run function.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageActions"></param>
        /// <returns></returns>
        [Function(nameof(FnCorporateCreditReader))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporatecredit", Connection = "AsbQueueEndPoint")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            AsbMessageInfo msgProps = FnCommon.GetMessageProperties(message);

            if (msgProps.CanProcess)
            {
                try
                {
                    string errMsg = string.Empty;
                    this._logger.LogInformation($"Message Processing Start");
                    DynTaskResponse taskResp = new DynTaskResponse();
                    taskResp.IsSuccess = false;
                    bool canProcess = false;

                    switch (msgProps.MessageType)
                    {
                        case nameof(UpdateCreditTierRequest):
                            UpdateCreditTierRequest ctierResp = FnCommon.DeSerializeServiceBusMessage<UpdateCreditTierRequest>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _creditTierProvider.UpdateCreditTier(ctierResp, msgProps.RequestId, _crmServiceClient, this._logger);
                            break;
                    }

                    this._logger.LogInformation($"Message Processing end");

                    if (taskResp.IsSuccess)
                    {
                        await messageActions.CompleteMessageAsync(message);
                    }
                    else
                    {
                        this._logger.LogError("Failed to handle asp response:" + taskResp.ErrorMessage);
                        await messageActions.DeadLetterMessageAsync(message);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error Processing Response");
                }
            }
        }
    }
}