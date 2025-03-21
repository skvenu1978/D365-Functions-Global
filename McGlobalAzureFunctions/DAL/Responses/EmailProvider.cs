// <copyright file="EmailProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.Utilities;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Function to process Emails
    /// </summary>
    public class EmailProvider : IEmailProvider
    {
        /// <summary>
        /// Process Email Activity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="isProd"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse ProcessEmailActivityRequest(EmailActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateEmailsReader> logger)
        {
            logger.LogInformation("ProcessEmailActivityRequest started for requestId:" + requestId.ToString());

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;

            if (request != null)
            {
                logger.LogInformation("Request is not null");

                Guid? createdEmailId = null;

                if (!string.IsNullOrEmpty(request.RegardingEntityName) &&
                    request.RegardingEntityId.HasValue)
                {
                    bool checkRegardingRecordExists = false;
                    logger.LogInformation("Checking regarding record exists...start");
                    Entity regEntity = CrudCommon.CheckEntityExistsById(svcClient, request.RegardingEntityName.ToLower(), request.RegardingEntityId.Value, out checkRegardingRecordExists);
                    logger.LogInformation("Checking regarding record exists...start");

                    if (checkRegardingRecordExists)
                    {
                        logger.LogInformation("Creating email start");

                        try
                        {
                            // create Email
                            Entity email = new Entity(EmailConstants.Email);
                            email[EmailConstants.Regardingobjectid] = new EntityReference(request.RegardingEntityName, request.RegardingEntityId.Value);

                            if (request.Recipients != null)
                            {
                                email["to"] = GetParties(request.Recipients, svcClient);
                            }

                            bool userOrQueueExists = false;
                            EntityCollection senderParty = GetSenderPartyInfo(request, svcClient, out userOrQueueExists);

                            if (userOrQueueExists)
                            {
                                email["from"] = senderParty;
                            }
                            else
                            {
                                // search for the sender using sender email address
                                response.ErrorMessage = "Sender/Sender Email cannot be found";
                                return response;
                            }

                            email["actualend"] = DateTime.Now.ToUniversalTime();
                            email["subject"] = request.Subject;
                            email["description"] = request.Body;
                            email["directioncode"] = false;
                            email["trackingtoken"] = string.Empty;
                            email["ownerid"] = regEntity["ownerid"];
                            email["mcorp_mmmemailid"] = request.DocumentId;
                            createdEmailId = svcClient.Create(email);
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            response.ErrorMessage = "ProcessEmailActivityRequest failed to create email:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }

                        logger.LogInformation("Creating email complete");
                        logger.LogInformation("Creating email attachments start");

                        try
                        {
                            if (createdEmailId.HasValue)
                            {
                                // upload attachments to D365
                                bool attachmentTaskComplete = CreateAttachments(request.Attachments, createdEmailId.Value, "email", svcClient);

                                if (attachmentTaskComplete)
                                {
                                    if (request.SendEmail == true)
                                    {
                                        if (isProd)
                                        {
                                            // if its prod, send email
                                            SendEmailRequest sendReq = new SendEmailRequest();
                                            sendReq.EmailId = createdEmailId.Value;
                                            sendReq.TrackingToken = string.Empty;
                                            sendReq.IssueSend = true;
                                            svcClient.Execute(sendReq);
                                        }
                                        else
                                        {
                                            // if its non prod, set status to COMPLETED
                                            CrudCommon.SetState(createdEmailId.Value, "email", 1, 2, svcClient);
                                        }
                                    }
                                    else
                                    {
                                        CrudCommon.SetState(createdEmailId.Value, "email", 1, 2, svcClient);
                                    }

                                    response.IsSuccess = true;
                                }
                            }
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            response.ErrorMessage = "ProcessEmailActivityRequest failed to create attachments:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }
                        catch (Exception ex)
                        {
                            response.ErrorMessage = "ProcessEmailActivityRequest failed to create attachments:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }

                        logger.LogInformation("Creating email attachments complete");
                    }
                    else
                    {
                        response.ErrorMessage = "Regarding record does not exist in D365";
                        response.Exception = new Exception(response.ErrorMessage);
                        logger.LogInformation(response.ErrorMessage);
                    }
                }
                else
                {
                    logger.LogInformation("Request Regarding is null");
                }
            }

            logger.LogInformation("ProcessEmailActivityRequest Completed for requestId:" + requestId.ToString());

            return response;
        }

        /// <summary>
        /// Process Letter Activity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse ProcessLetterActivityRequest(EmailActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateEmailsReader> logger)
        {
            logger.LogInformation("ProcessLetterActivityRequest started for requestId:" + requestId.ToString());

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;

            if (request != null)
            {
                logger.LogInformation("Request is not null");

                Guid? createdLetterId = null;

                if (!string.IsNullOrEmpty(request.RegardingEntityName) &&
                    request.RegardingEntityId.HasValue)
                {
                    bool checkRegardingRecordExists = false;
                    logger.LogInformation("Checking regarding record exists...start");
                    Entity regEntity = CrudCommon.CheckEntityExistsById(svcClient, request.RegardingEntityName.ToLower(), request.RegardingEntityId.Value, out checkRegardingRecordExists);
                    logger.LogInformation("Checking regarding record exists...start");

                    if (checkRegardingRecordExists)
                    {
                        logger.LogInformation("Creating letter start");

                        try
                        {
                            // create Email
                            Entity letter = new Entity("letter");
                            letter[EmailConstants.Regardingobjectid] = new EntityReference(request.RegardingEntityName, request.RegardingEntityId.Value);
                            letter["to"] = GetLetterPartyInfo(request);

                            bool userOrQueueExists = false;
                            EntityCollection senderParty = GetSenderPartyInfo(request, svcClient, out userOrQueueExists);

                            if (userOrQueueExists)
                            {
                                letter["from"] = senderParty;
                            }
                            else
                            {
                                // search for the sender using sender email address
                                response.ErrorMessage = "Sender/Sender Party cannot be found";
                                return response;
                            }

                            letter["actualend"] = DateTime.Now.ToUniversalTime();
                            letter["subject"] = request.Subject;
                            letter["description"] = request.Body;
                            letter["directioncode"] = true;
                            letter["ownerid"] = regEntity["ownerid"];
                            createdLetterId = svcClient.Create(letter);
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            response.ErrorMessage = "ProcessLetterActivityRequest failed to create email:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }

                        logger.LogInformation("Creating Letter complete");
                        logger.LogInformation("Creating Letter attachments start");

                        try
                        {
                            if (createdLetterId.HasValue)
                            {
                                // upload attachments to D365
                                bool attachmentTaskComplete = CreateAttachments(request.Attachments, createdLetterId.Value,"letter",svcClient);

                                if (attachmentTaskComplete)
                                {
                                    CrudCommon.SetState(createdLetterId.Value, "letter", 1, 4, svcClient);
                                    response.IsSuccess = true;
                                }
                            }
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            response.ErrorMessage = "ProcessLetterActivityRequest failed to create attachments:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }
                        catch (Exception ex)
                        {
                            response.ErrorMessage = "ProcessLetterActivityRequest failed to create attachments:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }

                        logger.LogInformation("Creating Letter attachments complete");
                    }
                    else
                    {
                        response.ErrorMessage = "Regarding record does not exist in D365";
                        response.Exception = new Exception(response.ErrorMessage);
                        logger.LogInformation(response.ErrorMessage);
                    }
                }
                else
                {
                    logger.LogInformation("Request Regarding is null");
                }
            }

            logger.LogInformation("ProcessLetterActivityRequest Completed for requestId:" + requestId.ToString());

            return response;
        }

        /// <summary>
        /// Get the FROM user table using ID or Email.
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="svcClient">svc client</param>
        /// <param name="userOrQueueExists">exists</param>
        /// <returns>EntityCollection</returns>
        private EntityCollection GetSenderPartyInfo(EmailActivity request, IOrganizationServiceAsync2 svcClient, out bool userOrQueueExists)
        {
            userOrQueueExists = false;
            EntityCollection fromlist = new ();
            EntityReference userEntiy = CrudCommon.GetUserOrQueueEntityReferenceById(svcClient, request.Sender, request.SenderEmail, out userOrQueueExists);

            if (userOrQueueExists)
            {
                Entity senderPartyEntity = new Entity("activityparty");
                senderPartyEntity["partyid"] = userEntiy;
                fromlist.Entities.Add(senderPartyEntity);
            }

            return fromlist;
        }

        /// <summary>
        /// GetLetterPartyInfo.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>EntityCollection</returns>
        private EntityCollection GetLetterPartyInfo(EmailActivity request)
        {
            EntityCollection fromlist = new ();

            if (request.RegardingEntityId.HasValue)
            {
                Entity senderPartyEntity = new Entity("activityparty");
                senderPartyEntity["partyid"] = new EntityReference(request.RegardingEntityName, request.RegardingEntityId.Value);
                fromlist.Entities.Add(senderPartyEntity);
            }

            return fromlist;
        }

        /// <summary>
        /// Upload attachments in D365.
        /// </summary>
        /// <param name="attachments">attachments bytes array</param>
        /// <param name="recordId">record Id</param>
        /// <param name="regardingEntityName">entity name</param>
        /// <param name="svcClient">svc Client</param>
        /// <returns>bool</returns>
        private static bool CreateAttachments(List<AttachmentNotification>? attachments, Guid recordId, string regardingEntityName, IOrganizationServiceAsync2 svcClient)
        {
            bool attachmentTaskComplete = false;

            if (attachments == null || attachments.Count() == 0)
            {
                attachmentTaskComplete = true;
                return attachmentTaskComplete;
            }

            EntityCollection activityMimeAttachments = new EntityCollection();

            if (attachments != null && attachments.Count() > 0)
            {
                foreach (AttachmentNotification attachmentNotification in attachments)
                {
                    if (attachmentNotification.Attachment != null && attachmentNotification.Attachment.Length > 0)
                    {
                        Entity item = new ("activitymimeattachment");
                        item["body"] = Convert.ToBase64String(attachmentNotification.Attachment);
                        item["objecttypecode"] = regardingEntityName;
                        item["objectid"] = new EntityReference(regardingEntityName, recordId);
                        item["filename"] = attachmentNotification.Filename;

                        if (attachmentNotification.Filename != null)
                        {
                            string? mimeType = MimeTypeHandler.GetMimeTypeBasedOnFileName(attachmentNotification.Filename);
                            if (mimeType != null)
                            {
                                item["mimetype"] = mimeType;
                            }
                        }

                        svcClient.Create(item);
                        activityMimeAttachments.Entities.Add(item);
                    }
                }

                attachmentTaskComplete = true;
            }

            return attachmentTaskComplete;
        }

        /// <summary>
        /// GetParties.
        /// </summary>
        /// <param name="recipients">recipients</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>EntityCollection</returns>
        private EntityCollection GetParties(List<RecipientInfo> recipients, IOrganizationServiceAsync2 svcClient)
        {
            EntityCollection partyList = new ();

            foreach (RecipientInfo p in recipients)
            {
                Entity activityPartyEntity = new Entity("activityparty");

                if (p.RecipientType == Const.Enums.RecipientType.Account)
                {
                    activityPartyEntity["partyid"] = new EntityReference("account", p.RecipientGuid);
                    partyList.Entities.Add(activityPartyEntity);
                }

                if (p.RecipientType == Const.Enums.RecipientType.Contact)
                {
                    activityPartyEntity["partyid"] = new EntityReference("contact", p.RecipientGuid);
                    partyList.Entities.Add(activityPartyEntity);
                }

                if (p.RecipientType == Const.Enums.RecipientType.SystemUser)
                {
                    Guid? d365UserId = EntityCommon.GetD365UserUsingCRM2011Guid(p.RecipientGuid, svcClient);

                    if (d365UserId.HasValue)
                    {
                        activityPartyEntity["partyid"] = new EntityReference("systemuser", d365UserId.Value);
                        partyList.Entities.Add(activityPartyEntity);
                    }
                }

                if (p.RecipientType == Const.Enums.RecipientType.Queue)
                {
                    activityPartyEntity["partyid"] = new EntityReference("queue", p.RecipientGuid);
                    partyList.Entities.Add(activityPartyEntity);
                }
            }

            return partyList;
        }
    }
}