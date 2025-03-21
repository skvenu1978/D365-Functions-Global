// <copyright file="DeleteRateTrackerOpportunityConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Processes Rate Tracker Deletion task.
    /// </summary>
    public class DeleteRateTrackerOpportunityConversion
    {
        /// <summary>
        /// GetDeleteRateTrackerOpportunityRequest.
        /// </summary>
        /// <param name="contextEntity">contextEntity</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>DeleteRateTrackerOpportunityRequest</returns>
        public static DeleteRateTrackerOpportunityRequest GetDeleteRateTrackerOpportunityRequest(Entity contextEntity, IOrganizationServiceAsync2 svcClient, ILogger logger, out string errMsg)
        {
            DeleteRateTrackerOpportunityRequest deleteRateTrackerInfoRequest = new DeleteRateTrackerOpportunityRequest();
            errMsg = string.Empty;
            Entity entity = new ();

            try
            {
                entity = svcClient.Retrieve(OptyConstants.opportunity, contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(OptyConstants.mcorp_opportunitytype, OptyConstants.mcorp_checkalerts, OptyConstants.statecode));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to GetRequest for DeleteRateTrackerOpportunity:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                logger.LogError(ex, errMsg);
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                int? optyType = AttributeHelper.GetOptionSetValue(entity, OptyConstants.mcorp_opportunitytype);
                bool checkAlerts = AttributeHelper.GetBooleanValueDefaultFalse(contextEntity, OptyConstants.mcorp_checkalerts);

                if (optyType.HasValue)
                {
                    if (checkAlerts == false &&
                        (
                            optyType.Value == (int)OpportunityType.RateTracker
                            || optyType.Value == (int)OpportunityType.FirmOCO
                            || optyType.Value == (int)OpportunityType.FirmLimit
                            || optyType.Value == (int)OpportunityType.MarketWatch
                            || optyType.Value == (int)OpportunityType.FirmStopLoss

                        ))
                    {
                        deleteRateTrackerInfoRequest.OpportunityId = contextEntity.Id;
                    }
                    else
                    {
                        errMsg = "Opportunity is not valid to be sent to OMNI";
                    }
                }
                else
                {
                    errMsg = "Opportunity Type is not set";
                }
            }
            else
            {
                errMsg = "Opportunity does not exists for the DeleteRateTracker";
            }

            return deleteRateTrackerInfoRequest;
        }
    }
}