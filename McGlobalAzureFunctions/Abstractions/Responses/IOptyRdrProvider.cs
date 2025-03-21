// <copyright file="IOptyRdrProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for Opportunity Provider.
    /// </summary>
    public interface IOptyRdrProvider
    {
       
        /// <summary>
        /// ProcessDealSummary function
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns></returns>
        DynTaskResponse ProcessDealSummary(DealSummary request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger);

        /// <summary>
        /// ProcessPlanSummary function
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns></returns>
        DynTaskResponse ProcessPlanSummary(PlanSummary request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger);
    }
}