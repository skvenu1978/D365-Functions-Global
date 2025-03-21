// <copyright file="FnCorporateEmailsReader.cs" company="moneycorp">
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
    using McGlobalAzureFunctions.Const;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// Function to Read Corporate
    /// Emails from ASB Queue
    /// </summary>
    public class FnCorporateEmailsReader
    {
        private readonly ILogger<FnCorporateEmailsReader> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IEmailProvider _provider;

        /// <summary>
        /// FnCorporateEmailsReader Constructor.
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="crmSvcClient">Svc Client</param>
        /// <param name="provider">Interface provider</param>
        public FnCorporateEmailsReader(ILogger<FnCorporateEmailsReader> logger, IOrganizationServiceAsync2 crmSvcClient, IEmailProvider provider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmSvcClient, nameof(crmSvcClient));
            _logger = logger;
            _crmServiceClient = crmSvcClient;
            _provider = provider;
        }

        /// <summary>
        /// Function Run Method.
        /// </summary>
        /// <param name="message">Asb Message</param>
        /// <param name="messageActions">ASb Message Actions</param>
        /// <returns></returns>
        [Function(nameof(FnCorporateEmailsReader))]
        public async Task Run(
            [ServiceBusTrigger("mextod365corporateemails", Connection = "AsbQueueEndPoint")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation(nameof(FnCorporateEmailsReader) + $" Function Processing Started");

            AsbMessageInfo msgProps = FnCommon.GetMessageProperties(message);

            if (msgProps.CanProcess)
            {
                try
                {
                    bool isProd = FnCommon.GetIsProd();
                    string errMsg = string.Empty;
                    DynTaskResponse taskResp = new DynTaskResponse();
                    taskResp.IsSuccess = false;
                    
                    EmailActivity? activityResp = JsonConvert.DeserializeObject<EmailActivity>(message.Body.ToString());
                    _logger.LogInformation(LogConstants.serStart);
                    var reqData = JsonConvert.SerializeObject(activityResp);
                    _logger.LogInformation(reqData);
                    _logger.LogInformation(LogConstants.serEnd);

                    if (activityResp != null)
                    {
                        if (activityResp.RequestName == "SendEmailMessageRequest" || activityResp.RequestName == "RecordEmailMessageRequest")
                        {
                            taskResp = _provider.ProcessEmailActivityRequest(activityResp, msgProps.RequestId, _crmServiceClient, isProd, _logger);
                        }

                        if (activityResp.RequestName == "RecordDocumentRequest")
                        {
                            taskResp = _provider.ProcessLetterActivityRequest(activityResp, msgProps.RequestId, _crmServiceClient, _logger);
                        }
                    }

                    if (taskResp.IsSuccess)
                    {
                        _logger.LogInformation($"Message Processing Successful");
                        await messageActions.CompleteMessageAsync(message);
                    }
                    else
                    {
                        _logger.LogInformation($"Message Processing Failed:" + taskResp.ErrorMessage, taskResp.Exception);

                        _logger.LogError("Failed to handle asp response:" + taskResp.ErrorMessage, new Exception(taskResp.ErrorMessage));
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
