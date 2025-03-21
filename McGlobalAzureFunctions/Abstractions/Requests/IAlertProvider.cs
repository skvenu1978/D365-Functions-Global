// <copyright file="IAlertProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Requests
{
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for Alert Data.
    /// </summary>
    public interface IAlertProvider
    {
        /// <summary>
        /// Gets the list of alerts linked to the account.
        /// </summary>
        /// <param name="accountId">Record Id.</param>
        /// <param name="svcClient">Service Client.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="errMsg">out err message.</param>
        /// <returns></returns>
        AlertInfoRequest GetAlertRequest(Guid accountId, IOrganizationServiceAsync2 svcClient, ILogger logger, out string errMsg);
    }
}