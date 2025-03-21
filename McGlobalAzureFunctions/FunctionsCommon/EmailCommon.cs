// <copyright file="EmailCommon.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.FunctionsCommon
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Common;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.ServiceModel;

    /// <summary>
    /// Email Common.
    /// </summary>
    public class EmailCommon
    {
        /// <summary>
        /// ProcessRateTrackerNotificationEmail.
        /// </summary>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isProd">isProd</param>
        /// <param name="logger">logger</param>
        /// <param name="entity">entity</param>
        /// <returns>DynTaskResponse</returns>
        public static DynTaskResponse ProcessRateTrackerNotificationEmail(IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger, Entity entity)
        {
            DynTaskResponse taskResponse = new DynTaskResponse();

            try
            {
                // get notification method from OPPT
                Guid? contactId = AttributeHelper.GetGuidValue(entity, OptyConstants.mcorp_notificationcontactid);

                if (contactId.HasValue)
                {
                    int? notificationMethod = AttributeHelper.GetOptionSetValue(entity, OptyConstants.mcorp_marketwatchnotificationmethod);

                    if (notificationMethod.HasValue)
                    {
                        if (notificationMethod.Value == 1 || notificationMethod.Value == 2)
                        {
                            logger.LogInformation("Notification by email is allowed");

                            // send email
                            // get to email address from Contact if mcorp_emailnotificationcontact is not set on the opty
                            string templateContent = TemplateHelper.GetTemplateContentRateTrackerCorpNotification();
                            taskResponse = SendOpportunityEmail(contactId.Value, templateContent, "Exchange Rate Notification", entity, svcClient, isProd, logger);
                        }
                    }
                }
                else
                {
                    taskResponse.ErrorMessage = "Notification contact is not set";
                    logger.LogInformation(taskResponse.ErrorMessage);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                taskResponse.ErrorMessage = "ProcessRateTrackerNotificationEmail failed:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                taskResponse.Exception = ex;
            }
            catch (Exception ex)
            {
                taskResponse.ErrorMessage = "ProcessRateTrackerNotificationEmail failed:" + EntityCommon.GetErrorMessageFromException(ex);
                taskResponse.Exception = ex;
            }

            return taskResponse;
        }

        /// <summary>
        /// ProcessRateTrackerNotificationEmail.
        /// </summary>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isProd">isProd</param>
        /// <param name="logger">logger</param>
        /// <param name="entity">entity</param>
        /// <returns>DynTaskResponse</returns>
        public static DynTaskResponse ProcessRateTrackerNotificationAEEmail(IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger, Entity entity)
        {
            DynTaskResponse taskResponse = new() { IsSuccess = false };

            try
            {
                // get notification method from OPPT
                Guid? accountId = AttributeHelper.GetGuidValue(entity, OptyConstants.parentaccountid);

                if (accountId.HasValue)
                {
                    // get the AE and DEALER
                    Entity account = svcClient.Retrieve(AccountConstants.Account, accountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "ownerid", "mcorp_accountexecutiveid", "mcorp_dealerid"));

                    if (!string.IsNullOrEmpty(account.LogicalName))
                    {
                        Guid? aeId = AttributeHelper.GetGuidValue(account, "mcorp_accountexecutiveid");
                        Guid? dealerId = AttributeHelper.GetGuidValue(account, "mcorp_dealerid");
                        string accountName = AttributeHelper.GetStringAttributeValue(account, "name");

                        if (aeId.HasValue || dealerId.HasValue)
                        {
                            EntityReference toRef = new EntityReference();

                            if (aeId.HasValue && !dealerId.HasValue)
                            {
                                toRef = new EntityReference("systemuser", aeId.Value);
                            }

                            if (!aeId.HasValue && dealerId.HasValue)
                            {
                                toRef = new EntityReference("systemuser", dealerId.Value);
                            }

                            string fromEmail = "Corporate_Auto@moneycorp.com";
                            bool senderExists = false;
                            EntityReference fromEntRef = EntityCommon.GetUserOrQueueUsingEmailAddress(svcClient, fromEmail, out senderExists);

                            if (senderExists)
                            {
                                Entity email = new Entity(EmailConstants.Email);
                                email[EmailConstants.Regardingobjectid] = entity.ToEntityReference();

                                if (dealerId.HasValue && aeId.HasValue)
                                {
                                    EntityReference ccRef = new EntityReference("systemuser", dealerId.Value);
                                    email["cc"] = GetPartyCollection(ccRef);
                                    toRef = new EntityReference("systemuser", aeId.Value);
                                }

                                email["to"] = GetPartyCollection(toRef);
                                email["from"] = GetPartyCollection(fromEntRef);
                                email["actualend"] = DateTime.Now.ToUniversalTime();
                                email["subject"] = "Rate tracker notification Order has hit for account: " + accountName;
                                string emailBody = AENotificationContent(entity.Id, accountName);
                                email["description"] = emailBody;
                                email["directioncode"] = false;
                                email["trackingtoken"] = string.Empty;
                                email["ownerid"] = account["ownerid"];
                                Guid? createdEmailId = svcClient.Create(email);

                                if (createdEmailId.HasValue)
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
                            }
                        }
                    }
                }
                else
                {
                    taskResponse.ErrorMessage = "Notification contact is not set";
                    logger.LogInformation(taskResponse.ErrorMessage);
                }

                taskResponse.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                taskResponse.ErrorMessage = "ProcessRateTrackerNotificationEmail failed:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                taskResponse.Exception = ex;
            }
            catch (Exception ex)
            {
                taskResponse.ErrorMessage = "ProcessRateTrackerNotificationEmail failed:" + EntityCommon.GetErrorMessageFromException(ex);
                taskResponse.Exception = ex;
            }

            return taskResponse;
        }

        private static string AENotificationContent(Guid optyId, string accountName)
        {
            string? xrmConnection = Environment.GetEnvironmentVariable(FnConstants.D365ConnectionString, EnvironmentVariableTarget.Process);
            string recordUrlPrefix = "https://moneycorp-global-qa.crm4.dynamics.com/main.aspx?appid=d0df4603-9086-ef11-ac20-7c1e5229e1e3&pagetype=entityrecord&etn=opportunity&id=";

            if (!string.IsNullOrEmpty(xrmConnection))
            {
                if (xrmConnection.Contains("moneycorp-global-uat.crm4"))
                {
                    recordUrlPrefix = "https://moneycorp-global-uat.crm4.dynamics.com/main.aspx?appid=5c1ff93c-838a-ef11-ac21-000d3aaae346&pagetype=entityrecord&etn=opportunity&id=";
                }

                if (xrmConnection.Contains("moneycorp-global-uat.crm4"))
                {
                    recordUrlPrefix = "https://moneycorp-global-prod.crm4.dynamics.com/main.aspx?pagetype=entityrecord&etn=opportunity&id=";
                }
            }

            return @"<!DOCTYPE html><html><head><base target='_new'><style type='text/css''> body { font-family: Tahoma, Verdana, Arial; font-size: 12px; } 
                    pre.mscrmpretag { word-wrap: break-word; } </style>
                    </head><span><span><span><div><div><span><span><span><span><span><span><div><font color='#ff0000' size=3><strong>
                    <u>RATE TRACKER NOTIFICATION</u></strong></font></div>
                <div> </div><div><p>Rate tracker notification Order has hit for account :  " + accountName + "</p>" +
                "<p>Please <a id='1link' href='" + recordUrlPrefix + optyId.ToString().Replace("{", "").Replace("}", "") + @"'>click here</a> to open the Opportunity.</p></div></span></span></span></span></span></span></div></div></span></span></span></html>";
        }

        /// <summary>
        /// SendOpportunityEmail
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="templateContent"></param>
        /// <param name="emailSubject"></param>
        /// <param name="entity"></param>
        /// <param name="svcClient"></param>
        /// <param name="isProd"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static DynTaskResponse SendOpportunityEmail(Guid contactId, string templateContent, string emailSubject, Entity entity, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger)
        {
            DynTaskResponse taskResponse = new DynTaskResponse() { IsSuccess = false };
            DynTaskResponse resp = new DynTaskResponse() { IsSuccess = false };

            try
            {
                resp = TemplateHelper.GetEmailBody(entity, svcClient, templateContent, emailSubject);

                if (resp.TemplateData != null && resp.IsSuccess)
                {
                    logger.LogInformation("TemplateData is set");
                    resp.TemplateData.ToEmail = AttributeHelper.GetStringAttributeValue(entity, OptyConstants.mcorp_emailnotificationcontact);

                    if (string.IsNullOrEmpty(resp.TemplateData.ToEmail))
                    {
                        Entity ct = svcClient.Retrieve(ContactConstants.Contact, contactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(ContactConstants.Emailaddress1));
                        resp.TemplateData.ToEmail = AttributeHelper.GetStringAttributeValue(ct, ContactConstants.Emailaddress1);
                    }

                    resp.TemplateData.FromEmail = "Corporate_Auto@moneycorp.com";
                    bool senderExists = false;

                    EntityReference fromEntRef = EntityCommon.GetUserOrQueueUsingEmailAddress(svcClient, resp.TemplateData.FromEmail, out senderExists);

                    if (senderExists)
                    {
                        resp.TemplateData.FromEntityRef = fromEntRef;
                        resp.TemplateData.ToEntityRef = AttributeHelper.GetEntityReference(entity, OptyConstants.mcorp_notificationcontactid);
                        taskResponse = SendNotificationEmail(entity, resp.TemplateData, svcClient, isProd, logger);
                    }
                    else
                    {
                        logger.LogInformation("Sender email:" + resp.TemplateData.FromEmail + " does not exist in target");
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                taskResponse.ErrorMessage = "SendOpportunityEmail failed to create email:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                taskResponse.Exception = ex;
            }
            catch (Exception ex)
            {
                taskResponse.ErrorMessage = "SendOpportunityEmail failed to create email:" + EntityCommon.GetErrorMessageFromException(ex);
                taskResponse.Exception = ex;
            }

            taskResponse.BuyCurrency = resp.BuyCurrency;
            taskResponse.SellCurrency = resp.SellCurrency;
            taskResponse.TargetRate = resp.TargetRate;

            return taskResponse;
        }

        /// <summary>
        /// SendNotificationEmail.
        /// </summary>
        /// <param name="optyEntity">optyEntity</param>
        /// <param name="emailData">emailData</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isProd">isProd</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        private static DynTaskResponse SendNotificationEmail(Entity optyEntity, TemplateItem emailData, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger)
        {
            DynTaskResponse emailResp = new DynTaskResponse();
            emailResp.IsSuccess = false;
            logger.LogInformation("SendNotificationEmail start");
            Guid? createdEmailId = null;

            logger.LogInformation("Creating email start");
            try
            {
                if (emailData.ToEntityRef != null && emailData.FromEntityRef != null)
                {
                    // create Email
                    Entity email = new Entity(EmailConstants.Email);
                    email[EmailConstants.Regardingobjectid] = optyEntity.ToEntityReference();
                    email["to"] = GetPartyCollection(emailData.ToEntityRef);
                    email["from"] = GetPartyCollection(emailData.FromEntityRef);

                    email["actualend"] = DateTime.Now.ToUniversalTime();
                    email["subject"] = emailData.TemplateSubject;
                    email["description"] = emailData.ContentHtml;
                    email["directioncode"] = false;
                    email["trackingtoken"] = string.Empty;
                    email["ownerid"] = optyEntity["ownerid"];

                    createdEmailId = svcClient.Create(email);
                }
                else
                {
                    emailResp.ErrorMessage = "From or To Email Refence is missing";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                emailResp.ErrorMessage = "SendNotificationEmail failed to create email:" + EntityCommon.GetErrorMessageFromException(ex);
                emailResp.Exception = ex;
            }

            logger.LogInformation("Creating email complete");
            logger.LogInformation("Send email or set state start");

            try
            {
                if (createdEmailId.HasValue)
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

                    emailResp.IsSuccess = true;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                emailResp.ErrorMessage = "SendNotificationEmail failed to set state or send email:" + EntityCommon.GetErrorMessageFromException(ex);
                emailResp.Exception = ex;
            }
            catch (Exception ex)
            {
                emailResp.ErrorMessage = "SendNotificationEmail failed to set state or send email:" + EntityCommon.GetErrorMessageFromException(ex);
                emailResp.Exception = ex;
            }
            logger.LogInformation("Send email or set state start");
            logger.LogInformation("SendNotificationEmail complete");

            return emailResp;
        }

        /// <summary>
        /// GetPartyCollection.
        /// </summary>
        /// <param name="entRef">entRef</param>
        /// <returns>EntityCollection</returns>
        private static EntityCollection GetPartyCollection(EntityReference entRef)
        {
            EntityCollection fromlist = new();
            Entity senderPartyEntity = new Entity("activityparty");
            senderPartyEntity["partyid"] = entRef;
            fromlist.Entities.Add(senderPartyEntity);

            return fromlist;
        }
    }
}
