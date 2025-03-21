// <copyright file="FnCorporateClientsReader.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Azure.Messaging.ServiceBus;
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Gate;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// FnCorporateClientsReader class.
    /// </summary>
    public class FnCorporateClientsReader
    {
        private readonly ILogger<FnCorporateClientsReader> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IClientResponseProvider _clientProvider;
        private readonly IContactProvider _contactProvider;
        private readonly IFraudProvider _fraudProvider;

        /// <summary>
        /// FnCorporateClientsReader constructor.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">svc client</param>
        /// <param name="clientProvider">client provider interface</param>
        /// <param name="contactProvider">contact provider interface</param>
        /// <param name="fraudProvider">fraud Provider interface</param>
        public FnCorporateClientsReader(ILogger<FnCorporateClientsReader> logger, IOrganizationServiceAsync2 crmServiceClient, IClientResponseProvider clientProvider,
            IContactProvider contactProvider, IFraudProvider fraudProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            this._logger = logger;
            this._crmServiceClient = crmServiceClient;
            this._clientProvider = clientProvider;
            this._contactProvider = contactProvider;
            this._fraudProvider = fraudProvider;
        }

        /// <summary>
        /// FnCorporateClientsReader RUN method.
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="messageActions">messageActions</param>
        /// <returns>Task result</returns>
        [Function(nameof(FnCorporateClientsReader))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporateclients", Connection = "AsbQueueEndPoint")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            AsbMessageInfo msgProps = FnCommon.GetMessageProperties(message);

            if (msgProps.CanProcess)
            {
                try
                {
                    string errMsg = string.Empty;
                    _logger.LogTrace($"Message Processing Start");
                    DynTaskResponse taskResp = new DynTaskResponse();
                    taskResp.IsSuccess = false;
                    bool canProcess = false;

                    switch (msgProps.MessageType)
                    {
                        case nameof(ClientContactInfoResponse):
                            ClientContactInfoResponse resp = FnCommon.DeSerializeServiceBusMessage<ClientContactInfoResponse>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _clientProvider.ProcessClientContactInfoResponse(resp, msgProps.RequestId, _crmServiceClient);
                            break;
                        case nameof(ClientInfoResponse):
                            ClientInfoResponse cInfoResp = FnCommon.DeSerializeServiceBusMessage<ClientInfoResponse>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _clientProvider.ProcessClientInfoResponse(cInfoResp, msgProps.RequestId, _crmServiceClient);
                            break;
                        case nameof(ReadMarketingPreferenceRequest):
                            ReadMarketingPreferenceRequest markPrefsResp = FnCommon.DeSerializeServiceBusMessage<ReadMarketingPreferenceRequest>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _clientProvider.ProcessMarketingPrefsUpdate(markPrefsResp, _crmServiceClient);
                            break;
                        case nameof(ContactInfoResponse):
                            ContactInfoResponse ctInfoResp = FnCommon.DeSerializeServiceBusMessage<ContactInfoResponse>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _contactProvider.ProcessContactInfoResponse(ctInfoResp, msgProps.RequestId, _crmServiceClient);
                            break;
                        case nameof(UserPrivilegesInfo):
                            UserPrivilegesInfo userPrivResp = FnCommon.DeSerializeServiceBusMessage<UserPrivilegesInfo>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _contactProvider.UpdateUserPrivileges(userPrivResp, msgProps.RequestId, _crmServiceClient);
                            break;
                        case nameof(ContactActivatingWorkflowInfo):
                            ContactActivatingWorkflowInfo cactivatingResp = FnCommon.DeSerializeServiceBusMessage<ContactActivatingWorkflowInfo>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _contactProvider.UpdateContactActivation(cactivatingResp, msgProps.RequestId, _crmServiceClient, _logger);
                            break;
                        case nameof(ContactWebActivationInfoResponse):
                            ContactWebActivationInfoResponse cactivationResp = FnCommon.DeSerializeServiceBusMessage<ContactWebActivationInfoResponse>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _contactProvider.UpdateOmniRefAndCheckForErrors(cactivationResp, msgProps.RequestId, _crmServiceClient, _logger);
                            break;
                        case nameof(FraudClientAuthorisationStatusChangeRequest):
                            FraudClientAuthorisationStatusChangeRequest fraudReq = FnCommon.DeSerializeServiceBusMessage<FraudClientAuthorisationStatusChangeRequest>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _fraudProvider.ProcessFraudClientAuthorisationStatusChangeRequest(fraudReq, msgProps.RequestId, _crmServiceClient, _logger);
                            break;
                    }

                    this._logger.LogTrace($"Message Processing end");

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