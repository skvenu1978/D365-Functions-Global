// <copyright file="ActivityProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// ActivityProvider.
    /// </summary>
    public class ActivityProvider : IActivityProvider
    {
        /// <summary>
        /// ProcessCrmActivity.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessCrmActivity(CrmActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnAttachmentActivityReader> logger)
        {
            logger.LogInformation("ProcessCrmActivity start");

            DynTaskResponse response = new();
            response.IsSuccess = false;
            response.RecordId = null;
            Entity entity = new();

            try
            {
                if (request.ObjectId.HasValue)
                {
                    if (request.Source == CrmEntitySource.Client)
                    {
                        entity = svcClient.Retrieve(AccountConstants.Account, request.ObjectId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet("ownerid"));
                    }
                    else
                    {
                        entity = svcClient.Retrieve(ContactConstants.Contact, request.ObjectId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet("ownerid"));
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "ActivityHandler-Failed to retrieve related account or contact:" + request.ObjectId.ToString());
                response.ErrorMessage = "ActivityHandler-Failed to retrieve related account or contact:" + request.ObjectId.ToString() +"-" +EntityCommon.GetErrorMessageFromFaultException(ex);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    if (request.ActivityType == CrmActivityType.Task)
                    {
                        logger.LogInformation("Creating Task start");
                        response.RecordId = CreateTask(request, entity, svcClient);
                        response.IsSuccess = true;
                        logger.LogInformation("Creating Task end");
                    }
                    else
                    {
                        logger.LogInformation("Creating Phone Call start");
                        response.RecordId = CreatePhoneCall(request, entity, svcClient);
                        response.IsSuccess = true;
                        logger.LogInformation("Creating Phone Call end");
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    response.ErrorMessage = "Failed to create activity for request id:" + requestId + "-" + EntityCommon.GetErrorMessageFromFaultException(ex);
                    logger.LogError(ex, response.ErrorMessage);
                    logger.LogInformation(response.ErrorMessage);
                    response.Exception = ex;
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = "Failed to create activity for request id:" + requestId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                    logger.LogError(ex, response.ErrorMessage);
                    logger.LogInformation(response.ErrorMessage);
                    response.Exception = ex;
                }
            }

            logger.LogInformation("ProcessCrmActivity complete");

            return response;
        }

        /// <summary>
        /// Create Task.
        /// </summary>
        /// <param name="crmActivity">crmActivity</param>
        /// <param name="regardingEntity">regardingEntity</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>Guid?</returns>
        private Guid? CreateTask(CrmActivity crmActivity, Entity regardingEntity, IOrganizationServiceAsync2 svcClient)
        {
            Entity activity = new(ActivityConstants.Task);
            activity[ActivityConstants.Description] = crmActivity.BodyDetails;
            activity[ActivityConstants.Subject] = crmActivity.Subject;
            activity[ActivityConstants.Regardingobjectid] = new EntityReference(regardingEntity.LogicalName, regardingEntity.Id);
            activity[ActivityConstants.Scheduledend] = crmActivity.DueDate == DateTime.MinValue ? DateTime.Now.ToUniversalTime() : crmActivity.DueDate.ToUniversalTime();
            activity[ActivityConstants.Prioritycode] = new OptionSetValue((int)task_prioritycode.High);
            activity[ActivityConstants.Ownerid] = regardingEntity[ActivityConstants.Ownerid];

            Guid? recordId = svcClient.Create(activity);

            if (recordId.HasValue)
            {
                if (crmActivity.ActivityState == CrmActivityState.Closed)
                {
                    CrudCommon.SetState(recordId.Value, ActivityConstants.Task, 1, 5, svcClient);
                }
            }

            return recordId;
        }

        /// <summary>
        /// Create phone call and set status to complete.
        /// </summary>
        /// <param name="crmActivity">crmActivity</param>
        /// <param name="regardingEntity">regardingEntity</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>Guid?</returns>
        private Guid? CreatePhoneCall(CrmActivity crmActivity, Entity regardingEntity, IOrganizationServiceAsync2 svcClient)
        {
            Entity activity = new Entity(ActivityConstants.Phonecall);
            activity[ActivityConstants.Description] = crmActivity.BodyDetails;
            activity[ActivityConstants.Subject] = crmActivity.Subject;
            activity[ActivityConstants.Regardingobjectid] = new EntityReference(regardingEntity.LogicalName, regardingEntity.Id);
            activity[ActivityConstants.Scheduledend] = crmActivity.DueDate == DateTime.MinValue ? DateTime.Now.ToUniversalTime() : crmActivity.DueDate.ToUniversalTime();
            activity[ActivityConstants.Prioritycode] = new OptionSetValue((int)task_prioritycode.High);
            activity[ActivityConstants.Ownerid] = regardingEntity[ActivityConstants.Ownerid];
            activity[ActivityConstants.Directioncode] = true;
            EntityReference regOwnerRef = regardingEntity.GetAttributeValue<EntityReference>(ActivityConstants.Ownerid);
            Entity party1 = new(ActivityConstants.Activityparty);
            party1[ActivityConstants.Partyid] = regardingEntity.ToEntityReference();
            EntityCollection to = new EntityCollection();
            to.Entities.Add(party1);
            activity[ActivityConstants.To] = to;
            Entity party2 = new(ActivityConstants.Activityparty);
            party2[ActivityConstants.Partyid] = regOwnerRef;
            EntityCollection from = new();
            from.Entities.Add(party2);
            activity[ActivityConstants.From] = to;

            Guid? recordId = svcClient.Create(activity);

            if (recordId.HasValue)
            {
                if (crmActivity.ActivityState == CrmActivityState.Closed)
                {
                    CrudCommon.SetState(recordId.Value, ActivityConstants.Phonecall, 1, 2, svcClient);
                }
            }

            return recordId;
        }
    }
}
