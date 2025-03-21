// <copyright file="ContactWebActionResponseHandler.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.OmniResponse
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;
    using System.Text;

    internal class ContactWebActionResponseHandler
    {
        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static DynTaskResponse UpdateOmniRefAndCheckForErrors(ContactWebActivationInfoResponse omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger)
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