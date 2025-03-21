// <copyright file="IRateProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Rate Provider Interface
    /// </summary>
    public interface IRateProvider
    {
        /// <summary>
        /// Handle Rate Tracker Deletion.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessRateTrackerDeletion(DeleteRateTrackerOpportunityInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateRateTracker> logger);

        /// <summary>
        /// GetRateTrackerOpportunityRequest
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="isCreate">isCreate</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="_logger">_logger</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>D365RateTrackerOpportunityRequest</returns>
        D365RateTrackerOpportunityRequest GetRateTrackerOpportunityRequest(Entity entity, bool isCreate, IOrganizationServiceAsync2 svcClient, ILogger<FnD365RateTrackerOpportunityRequestToAsb> _logger, out string errMsg);

        /// <summary>
        /// Updates existing opportunity
        /// with Raise Alert to TRUE
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="isProd"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        DynTaskResponse ProcessRateTrackerNotification(RateTrackerNotification omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger);

        /// <summary>
        /// Process Rate Tracker Opty from OMNI.
        /// </summary>
        /// <param name="optyId">optyId</param>
        /// <param name="contract">contract</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isProd">isProd</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessRateTrackerOptyInfo(Guid? optyId, D365RateTrackerOpportunityInfo contract, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger);
    }
}
