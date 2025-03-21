// <copyright file="FnAttachmentActivityReader.cs" company="moneycorp">
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
    using McGlobalAzureFunctions.OmniResponse;
    using McGlobalAzureFunctions.Const;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// FnAttachmentActivityReader Function
    /// </summary>
    public class FnAttachmentActivityReader
    {
        private readonly ILogger<FnAttachmentActivityReader> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IAttachmentProvider _attProvider;
        private readonly IActivityProvider _actProvider;

        /// <summary>
        /// FnAttachmentActivityReader constructor
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="crmServiceClient">crmServiceClient</param>
        /// <param name="attProvider">attProvider</param>
        /// <param name="actProvider">actProvider</param>
        public FnAttachmentActivityReader(ILogger<FnAttachmentActivityReader> logger, IOrganizationServiceAsync2 crmServiceClient, IAttachmentProvider attProvider, IActivityProvider actProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
            _attProvider = attProvider;
            _actProvider = actProvider; 
        }

        /// <summary>
        /// FnAttachmentActivityReader Function RUN. 
        /// </summary>
        /// <param name="message">Azure Message</param>
        /// <param name="messageActions">Messge actions</param>
        /// <returns>Task response</returns>
        [Function(nameof(FnAttachmentActivityReader))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporateattachments", Connection = "AsbQueueEndPoint")]
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

                    switch (msgProps.MessageType)
                    {
                        case nameof(CrmActivity):
                            CrmActivity activityResp = FnCommon.DeSerializeServiceBusMessage<CrmActivity>(message, messageActions, out canProcess);
                            if (canProcess)
                                taskResp = _actProvider.ProcessCrmActivity(activityResp, msgProps.RequestId, _crmServiceClient, _logger);
                            break;
                        case nameof(AttachmentRequest):
                            AttachmentRequest attachReq = FnCommon.DeSerializeServiceBusMessage<AttachmentRequest>(message, messageActions, out canProcess);
                            if (canProcess)
                            {
                                string? clientId = Environment.GetEnvironmentVariable(SharePointConstants.SharePointAppClientId, EnvironmentVariableTarget.Process);
                                string? clientSecret = Environment.GetEnvironmentVariable(SharePointConstants.SharePointAppSecret, EnvironmentVariableTarget.Process);
                                string? spSiteName = Environment.GetEnvironmentVariable(SharePointConstants.SharePointSiteName, EnvironmentVariableTarget.Process);

                                SpCredentials creds = new SpCredentials();
                                creds.TenantId = SharePointConstants.SpTenantId;

                                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(spSiteName))
                                {
                                    creds.ClientId = clientId;
                                    creds.ClientSecret = clientSecret;
                                    taskResp = _attProvider.ProcessAttachmentRequest(attachReq, creds, spSiteName, msgProps.RequestId, _crmServiceClient);
                                }
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