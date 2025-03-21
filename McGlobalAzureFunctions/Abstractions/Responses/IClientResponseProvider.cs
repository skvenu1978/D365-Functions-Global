// <copyright file="IClientResponseProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for Client Response
    /// </summary>
    public interface IClientResponseProvider
    {
        /// <summary>
        /// Method to process client contact response.
        /// </summary>
        /// <param name="omniResponse">Omni Response</param>
        /// <param name="requestId">Request Id</param>
        /// <param name="svcClient">Service Client</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessClientContactInfoResponse(ClientContactInfoResponse omniResponse, Guid requestId, IOrganizationServiceAsync2 svcClient);

        /// <summary>
        /// Method to process Client Info Reponse.
        /// </summary>
        /// <param name="omniData">Omni Response</param>
        /// <param name="requestId">Request Id</param>
        /// <param name="svcClient">Service Client</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessClientInfoResponse(ClientInfoResponse omniData, Guid requestId, IOrganizationServiceAsync2 svcClient);

        /// <summary>
        /// Method to process Marketing Prefs Update.
        /// </summary>
        /// <param name="omniRequest">Omni Response</param>
        /// <param name="svcClient">Service Client</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessMarketingPrefsUpdate(ReadMarketingPreferenceRequest omniRequest, IOrganizationServiceAsync2 svcClient);
    }
}