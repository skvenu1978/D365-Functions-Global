// <copyright file="ContactProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Extensions.Logging;
    using System.ServiceModel;

    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using System.Text;

    /// <summary>
    /// Implementation for Contact
    /// Provider
    /// </summary>
    public partial class ContactProvider : IContactProvider
    {
        /// <summary>
        ///  Updates contact if
        /// OMNI Contact id does not exists
        /// </summary>
        /// <param name="omniResponse"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <returns></returns>
        public DynTaskResponse ProcessContactInfoResponse(ContactInfoResponse omniResponse, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;
            response.LogMessage += "ProcessContactInfoResponse start:" + requestId.ToString();

            if (omniResponse.Results != null)
            {
                if (omniResponse.Results.Count > 0)
                {
                    try
                    {
                        foreach (var contact in omniResponse.Results)
                        {
                            if (contact.CRMGuid.HasValue)
                            {
                                var logMessage = "Update ";
                                Entity d365Contact = svcClient.Retrieve(ContactConstants.Contact, contact.CRMGuid.Value, new ColumnSet(ContactConstants.Mcorp_omnicontactid));
                                string omniContactId = AttributeHelper.GetStringAttributeValue(d365Contact, ContactConstants.Mcorp_omnicontactid);

                                if (string.IsNullOrEmpty(omniContactId))
                                {
                                    Entity d365ContactToUpdate = new Entity(ContactConstants.Contact, contact.CRMGuid.Value);
                                    d365ContactToUpdate[ContactConstants.Mcorp_omnicontactid] = contact.Id.ToString();
                                    logMessage += string.Format(": mcorp_omnicontactid={0}", contact.Id.ToString());
                                    response.LogMessage += logMessage;
                                    svcClient.Update(d365ContactToUpdate);
                                }
                                else
                                {
                                    response.LogMessage += "No update required on contact:" + contact.CRMGuid.Value.ToString();
                                }
                            }
                        }

                        response.IsSuccess = true;
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        response.ErrorMessage = "ProcessContactInfoResponse failed in D365:" + EntityCommon.GetErrorMessageFromException(ex);
                        response.Exception = ex;
                        response.LogMessage += response.ErrorMessage;
                    }
                }
            }
            else
            {
                response.ErrorMessage = "Response does not contain contacts";
                response.LogMessage += response.ErrorMessage;
            }

            response.LogMessage += "ProcessContactInfoResponse complete:" + requestId.ToString();

            return response;
        }

        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <returns></returns>
        public DynTaskResponse UpdateUserPrivileges(UserPrivilegesInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new Entity();

            try
            {
                entity = svcClient.Retrieve(ContactConstants.Contact, omniRequest.CRMContactGuid, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "UpdateUserPrivileges-Unable to find contact with ID:" + omniRequest.ContactId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    Entity contact = new Entity(entity.LogicalName, entity.Id);
                    contact[ContactConstants.Mcorp_authorisepayment] = omniRequest.IsAuthorisePayment;
                    contact[ContactConstants.Mcorp_createbeneficiary] = omniRequest.IsCreateBeneficiary;
                    contact[ContactConstants.Mcorp_createpayment] = omniRequest.IsCreatePayment;
                    contact[ContactConstants.Mcorp_createdeal] = omniRequest.IsCreateDeal;

                    int? omniContactId = AttributeHelper.GetStringAttributeAsIntegerValue(entity, ContactConstants.Mcorp_omnicontactid);

                    if (!omniContactId.HasValue)
                    {
                        contact[ContactConstants.Mcorp_omnicontactid] = omniRequest.ContactId.ToString();
                    }

                    svcClient.Update(contact);

                    response.IsSuccess = true;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    response.ErrorMessage = "UpdateUserPrivileges-Failed to update opportunity:" + EntityCommon.GetErrorMessageFromException(ex);
                    response.Exception = ex;
                }
            }

            return response;
        }

        /// <summary>
        /// Updates the contact with MCOL permissions.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse UpdateContactActivation(ContactActivatingWorkflowInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateClientsReader> logger)
        {
            logger.LogInformation("UpdateContactActivation start for request id:" + requestId.ToString());

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new Entity();

            try
            {
                ColumnSet cols = new ColumnSet(true);
                entity = svcClient.Retrieve(ContactConstants.Contact, omniRequest.CRMContactGuid, cols);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "UpdateContactActivation-Unable to find contact with ID:" + omniRequest.ContactId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "UpdateContactActivation-Unable to find contact with ID:" + omniRequest.ContactId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    Entity contact = new (entity.LogicalName, entity.Id);
                    int? omniContactId = AttributeHelper.GetStringAttributeAsIntegerValue(entity, ContactConstants.Mcorp_omnicontactid);
                    Guid? activationUserId = null;

                    if (omniRequest.ActivationEmailFromId.HasValue)
                    {
                        activationUserId = EntityCommon.GetD365UserUsingCRM2011Guid(omniRequest.ActivationEmailFromId.Value, svcClient);
                    }

                    if (!omniContactId.HasValue)
                    {
                        contact[ContactConstants.Mcorp_omnicontactid] = omniRequest.ContactId.ToString();
                    }

                    switch (omniRequest.ContactWebAccessStatus)
                    {
                        case ContactWebAccessStatus.NotActivated:
                            contact[ContactConstants.Mcorp_mcolactivationstatus] = new OptionSetValue((int)MCOLActivationStatus.NotActivated);
                            break;
                        case ContactWebAccessStatus.Activating:
                            contact[ContactConstants.Mcorp_mcolactivationstatus] = new OptionSetValue((int)MCOLActivationStatus.Activating);

                            if (activationUserId.HasValue)
                            {
                                contact[ContactConstants.Mcorp_activationuserid] = new EntityReference(UserConstants.Systemuser, activationUserId.Value);
                            }

                            if (omniRequest.ActivationEmailSentDate.HasValue)
                            {
                                if (!entity.Contains(ContactConstants.Mcorp_activationfirstdate))
                                {
                                    contact[ContactConstants.Mcorp_activationfirstdate] = omniRequest.ActivationEmailSentDate != null ? omniRequest.ActivationEmailSentDate.Value.ToUniversalTime() : new Nullable<DateTime>();
                                }

                                contact[ContactConstants.Mcorp_activationlastdate] = omniRequest.ActivationEmailSentDate != null ? omniRequest.ActivationEmailSentDate.Value.ToUniversalTime() : new Nullable<DateTime>();
                            }

                            break;
                        case ContactWebAccessStatus.Activated:
                            contact[ContactConstants.Mcorp_mcolactivationstatus] = new OptionSetValue((int)MCOLActivationStatus.Activated);

                            if (activationUserId.HasValue)
                            {
                                contact[ContactConstants.Mcorp_activationuserid] = new EntityReference(UserConstants.Systemuser, activationUserId.Value);
                            }

                            if (omniRequest.DateActivated.HasValue)
                            {
                                if (!entity.Contains(ContactConstants.Mcorp_activatedfirstdate))
                                {
                                    contact[ContactConstants.Mcorp_activatedfirstdate] = omniRequest.DateActivated != null ? omniRequest.DateActivated.Value.ToUniversalTime() : new Nullable<DateTime>();
                                }

                                contact[ContactConstants.Mcorp_activatedlastdate] = omniRequest.DateActivated != null ? omniRequest.DateActivated.Value.ToUniversalTime() : new Nullable<DateTime>();
                            }
                            break;
                        default:
                            break;
                    }

                    svcClient.Update(contact);

                    response.IsSuccess = true;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    response.ErrorMessage = "UpdateContactActivation-Failed to update contact:" + EntityCommon.GetErrorMessageFromException(ex);
                    logger.LogError(ex, response.ErrorMessage);
                    response.Exception = ex;
                }
            }

            logger.LogInformation("UpdateContactActivation complete for request id:" + requestId.ToString());

            return response;
        }

        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse UpdateOmniRefAndCheckForErrors(ContactWebActivationInfoResponse omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            logger.LogInformation("UpdateOmniRefAndCheckForErrors start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            if (omniRequest.Results != null && omniRequest.Results.Count > 0)
            {
                foreach (var item in omniRequest.Results)
                {
                    if (item.CRMGuid.HasValue)
                    {
                        Entity entity = new Entity();

                        try
                        {
                            entity = svcClient.Retrieve(ContactConstants.Contact, item.CRMGuid.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(ContactConstants.Mcorp_omnicontactid, ContactConstants.Ownerid));
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            logger.LogError(ex, "UpdateOmniRefAndCheckForErrors-Unable to find contact with ID:" + item.CRMGuid.Value + "-" + EntityCommon.GetErrorMessageFromException(ex));
                            response.ErrorMessage = "UpdateOmniRefAndCheckForErrors-Unable to find contact with ID:" + item.CRMGuid.Value + "-" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                        }

                        if (!string.IsNullOrEmpty(entity.LogicalName))
                        {
                            try
                            {
                                Entity contact = new Entity(entity.LogicalName, entity.Id);
                                int? omniContactId = AttributeHelper.GetStringAttributeAsIntegerValue(entity, ContactConstants.Mcorp_omnicontactid);

                                if (!omniContactId.HasValue)
                                {
                                    //update contact with OMNI Contact Id
                                    contact[ContactConstants.Mcorp_omnicontactid] = item.Id.ToString();
                                    svcClient.Update(contact);
                                }

                                if (item.ExceptionMessages != null && item.ExceptionMessages.Count > 0)
                                {
                                    // Create email with template

                                    //WorkflowProvider.ExecuteWorkflow(Guid.Parse(CrmConfiguration.WorkflowIdContactMobilePhoneParsingFailureNotificationInfo), contact.McId);

                                    StringBuilder strMessage = new StringBuilder();
                                    strMessage.Append("Contact Web Activation Info Response CrmContactId: " + item.CRMGuid.ToString() + Environment.NewLine + Environment.NewLine);
                                    if (item.Id != 0)
                                        strMessage.Append("omnicontactid: " + item.Id.ToString() + Environment.NewLine);
                                    strMessage.Append("Message: ");
                                    if (item.ExceptionMessages != null && item.ExceptionMessages.Count > 0)
                                        item.ExceptionMessages.ForEach(s => strMessage.Append(s + Environment.NewLine));

                                    Guid? taskId = CreateTask(contact, svcClient, "Fail to send SMS", strMessage.ToString());
                                    Guid? phonecallId = CreatePhoneCall(contact, svcClient);
                                }

                                response.IsSuccess = true;
                            }
                            catch (FaultException<OrganizationServiceFault> ex)
                            {
                                logger.LogError(ex, "UpdateContactActivation-Failed to update contact:" + EntityCommon.GetErrorMessageFromException(ex));
                                response.ErrorMessage = "UpdateContactActivation-Failed to update contact:" + EntityCommon.GetErrorMessageFromException(ex);
                                response.Exception = ex;
                            }
                        }
                    }
                }
            }

            logger.LogInformation("UpdateContactActivation complete");

            return response;
        }

        /// <summary>
        /// Creates a phone call activity
        /// to notify the customer
        /// </summary>
        /// <param name="regardingEntity"></param>
        /// <param name="svcClient"></param>
        /// <returns></returns>
        private static Guid? CreatePhoneCall(Entity regardingEntity, IOrganizationServiceAsync2 svcClient)
        {
            Entity activity = new Entity(ActivityConstants.Phonecall);
            activity[ActivityConstants.Subject] = "Failed MCOL Activation";
            activity[ActivityConstants.Regardingobjectid] = new EntityReference(regardingEntity.LogicalName, regardingEntity.Id);
            activity[ActivityConstants.Scheduledend] = DateTime.Now.ToUniversalTime();
            activity[ActivityConstants.Prioritycode] = new OptionSetValue((int)task_prioritycode.Normal);
            activity[ActivityConstants.Ownerid] = regardingEntity[ActivityConstants.Ownerid];
            activity[ActivityConstants.Directioncode] = true;
            EntityReference regOwnerRef = regardingEntity.GetAttributeValue<EntityReference>(ActivityConstants.Ownerid);
            Entity party1 = new Entity(ActivityConstants.Activityparty);
            party1[ActivityConstants.Partyid] = regardingEntity.ToEntityReference();
            EntityCollection to = new EntityCollection();
            to.Entities.Add(party1);
            activity[ActivityConstants.To] = to;
            Entity party2 = new Entity(ActivityConstants.Activityparty);
            party2[ActivityConstants.Partyid] = regOwnerRef;
            EntityCollection from = new EntityCollection();
            from.Entities.Add(party2);
            activity[ActivityConstants.From] = to;
            Guid? recordId = svcClient.Create(activity);

            return recordId;
        }

        /// <summary>
        /// Create task and
        /// close it as completed
        /// this is for error logging
        /// </summary>
        /// <param name="regardingEntity"></param>
        /// <param name="svcClient"></param>
        /// <param name="subject"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static Guid? CreateTask(Entity regardingEntity, IOrganizationServiceAsync2 svcClient, string subject, string description)
        {
            Entity activity = new Entity(ActivityConstants.Task);
            activity[ActivityConstants.Description] = description;
            activity[ActivityConstants.Subject] = subject;
            activity[ActivityConstants.Regardingobjectid] = new EntityReference(regardingEntity.LogicalName, regardingEntity.Id);
            activity[ActivityConstants.Prioritycode] = new OptionSetValue((int)task_prioritycode.Normal);
            activity[ActivityConstants.Ownerid] = regardingEntity[ActivityConstants.Ownerid];

            Guid? recordId = svcClient.Create(activity);

            if (recordId.HasValue)
            {
                CrudCommon.SetState(recordId.Value, ActivityConstants.Task, 1, 5, svcClient);
            }

            return recordId;
        }
    }
}