// <copyright file="ICreditTierProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Requests
{
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Interface for Credit Tier.
    /// </summary>
    public interface ICreditTierProvider
    {
        /// <summary>
        /// Gets the Credit Tier Request.
        /// </summary>
        /// <param name="contextEntity">Context Entity</param>
        /// <param name="svcClient">Service Client</param>
        /// <param name="isCreate">Bool is Create Message</param>
        /// <param name="isProd">Bool is PROD</param>
        /// <param name="errMsg">out Err Msg</param>
        /// <returns>D365CreateOrUpdateCreditTierRequest</returns>
        D365CreateOrUpdateCreditTierRequest GetCreateOrUpdateCreditTierRequest(Entity contextEntity, IOrganizationServiceAsync2 svcClient, bool isCreate, bool isProd, out string errMsg);

        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        DynTaskResponse UpdateCreditTier(UpdateCreditTierRequest omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger);
    }
}
