// <copyright file="CrudCommon.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.ServiceModel;

    using McGlobalAzureFunctions.Const.Enums;

    public class CrudCommon
    {
        public static void SetState(Guid recordId, string entityName, int stateCode, int statusCode, IOrganizationServiceAsync2 svcClient)
        {
            SetStateRequest req = new SetStateRequest();
            req.State = new OptionSetValue(stateCode);
            req.Status = new OptionSetValue(statusCode);
            req.EntityMoniker = new EntityReference(entityName, recordId);
            svcClient.Execute(req);
        }

        public static Entity GetEntityByCondition(string entityName, IOrganizationServiceAsync2 svcClient, string attributeName, object attributeValue, bool checkOnlyActive)
        {
            Entity retValue = new Entity();

            EntityCollection e = new EntityCollection();

            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = attributeName;
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(attributeValue);

            ConditionExpression condition2 = new ConditionExpression();

            if (checkOnlyActive)
            {
                condition2.AttributeName = "statecode";
                condition2.Operator = ConditionOperator.Equal;
                condition2.Values.Add(0);
            }

            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);

            if (checkOnlyActive)
            {
                filter1.Conditions.Add(condition2);
            }

            filter1.FilterOperator = LogicalOperator.And;

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddFilter(filter1);

            EntityCollection result = svcClient.RetrieveMultiple(query);

            if (result.Entities != null && result.Entities.Count > 0)
            {
                retValue = result.Entities[0];
            }

            return retValue;
        }

        public static EntityCollection GetEntitiesByCondition(string entityName, IOrganizationServiceAsync2 svcClient, string attributeName, object attributeValue, bool checkOnlyActive)
        {
            EntityCollection retValue = new EntityCollection();

            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = attributeName;
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(attributeValue);

            ConditionExpression condition2 = new ConditionExpression();

            if (checkOnlyActive)
            {
                condition2.AttributeName = "statecode";
                condition2.Operator = ConditionOperator.Equal;
                condition2.Values.Add(0);
            }

            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);

            if (checkOnlyActive)
            {
                filter1.Conditions.Add(condition2);
            }

            filter1.FilterOperator = LogicalOperator.And;

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddFilter(filter1);

            EntityCollection result = svcClient.RetrieveMultiple(query);

            if (result.Entities != null && result.Entities.Count > 0)
            {
                retValue = result;
            }

            return retValue;
        }

        /// <summary>
        /// Gets the record Id
        /// using the conditions from
        /// the parameters
        /// </summary>
        /// <param name="svcClient"></param>
        /// <param name="colName"></param>
        /// <param name="colValue"></param>
        /// <param name="entityName"></param>
        /// <param name="checkActive"></param>
        /// <returns></returns>
        public static Guid? GetEntityIdByCondition(IOrganizationServiceAsync2 svcClient, string colName, object colValue, string entityName, bool checkActive)
        {
            Guid? entityId = null;
            Entity entity = GetEntityByCondition(entityName, svcClient, colName, colValue, checkActive);

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                entityId = entity.Id;
            }

            return entityId;
        }

        public static Entity CheckEntityExistsById(IOrganizationServiceAsync2 svcClient, string entityName, Guid entityId, out bool exists)
        {
            Entity entity = new Entity();
            exists = false;

            try
            {
                entity = svcClient.Retrieve(entityName, entityId, new ColumnSet(entityName + "id", "ownerid"));

                if (!string.IsNullOrEmpty(entity.LogicalName))
                {
                    exists = true;
                }
            }
            catch (FaultException<OrganizationServiceFault>)
            { }

            return entity;
        }

        /// <summary>
        /// Get EntityReference by Id
        /// for activities.
        /// </summary>
        /// <param name="svcClient"></param>
        /// <param name="entityId"></param>
        /// <param name="senderEmail">senderEmail</param>
        /// <param name="exists"></param>
        /// <returns>EntityReference</returns>
        public static EntityReference GetUserOrQueueEntityReferenceById(IOrganizationServiceAsync2 svcClient, Guid entityId, string senderEmail,out bool exists)
        {
            Entity entity = new ();
            exists = false;

            try
            {
                entity = svcClient.Retrieve("systemuser", entityId, new ColumnSet("systemuserid"));

                if (!string.IsNullOrEmpty(entity.LogicalName))
                {
                    exists = true;
                }
            }
            catch (FaultException<OrganizationServiceFault>)
            { 
            }

            if (!exists)
            {
                // check in queue

                try
                {
                    entity = svcClient.Retrieve("queue", entityId, new ColumnSet("queueid"));

                    if (!string.IsNullOrEmpty(entity.LogicalName))
                    {
                        exists = true;
                    }
                }
                catch (FaultException<OrganizationServiceFault>)
                {
                }
            }

            if (!exists)
            {
                // check user by email address
                try
                {
                    entity = GetEntityByCondition("systemuser", svcClient, "internalemailaddress",senderEmail,false);

                    if (!string.IsNullOrEmpty(entity.LogicalName))
                    {
                        exists = true;
                    }
                }
                catch (FaultException<OrganizationServiceFault>)
                {
                }
            }

            if (!exists)
            {
                // check in queue
                try
                {
                    entity = GetEntityByCondition("queue", svcClient, "emailaddress", senderEmail, false);

                    if (!string.IsNullOrEmpty(entity.LogicalName))
                    {
                        exists = true;
                    }
                }
                catch (FaultException<OrganizationServiceFault>)
                {
                }
            }

            return entity.ToEntityReference();
        }

        public static void CloseOpportunity(Guid opportunityId, Entity entity, OpportunityState mcOpportunityState, OpportunityStatus mcOpportunityStatus, string subject, decimal revenue, IOrganizationServiceAsync2 svcClient)
        {
            try
            {
                Entity closeActivity = new Entity("opportunityclose");
                closeActivity["opportunityid"] = new EntityReference("opportunity", opportunityId);
                closeActivity["subject"] = subject;
                //closeActivity["description"] = content;
                if (revenue > decimal.MinValue)
                {
                    closeActivity["actualrevenue"] = new Money(revenue);
                }

                closeActivity["actualend"] = DateTime.Now.ToLocalTime();
                closeActivity["ownerid"] = entity["ownerid"];

                // Execute
                if (mcOpportunityState == OpportunityState.Won)
                {
                    WinOpty(closeActivity, mcOpportunityStatus, svcClient);
                }
                else if (mcOpportunityState == OpportunityState.Lost)
                {
                    LoseOpty(closeActivity, mcOpportunityStatus, svcClient);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception("FaultException: " + ex.Detail.Message);
            }
            catch (TimeoutException ex)
            {
                throw new Exception("TimeoutException: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Exception: " + ex.Message);
            }
        }

        private static void WinOpty(Entity activity, OpportunityStatus status, IOrganizationServiceAsync2 svcClient)
        {
            WinOpportunityRequest req = new WinOpportunityRequest();
            req.OpportunityClose = activity;
            req.Status = new OptionSetValue((int)status);
            svcClient.Execute(req);
        }

        private static void LoseOpty(Entity activity, OpportunityStatus status, IOrganizationServiceAsync2 svcClient)
        {
            LoseOpportunityRequest req = new LoseOpportunityRequest();
            req.OpportunityClose = activity;
            req.Status = new OptionSetValue((int)status);

            svcClient.Execute(req);
        }

        public static Guid GetExecutingUser(IOrganizationServiceAsync2 svcClient)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)svcClient.Execute(request);
            return response.UserId;
        }

        public static Guid GetExecutingUserDI(IOrganizationServiceAsync2 svcClient)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)svcClient.Execute(request);
            return response.UserId;
        }
    }
}