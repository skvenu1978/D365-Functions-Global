// <copyright file="LexisNexisProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Ekyc
{
    using McGlobalAzureFunctions.Models.Ekyc;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for LexisNexisProvider.
    /// </summary>
    public interface ILexisNexisProvider
    {
        /// <summary>
        /// CreateRequest.
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <param name="serviceClient">serviceClient</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <returns>LexisNexisRequest</returns>
        LexisNexisRequest? CreateRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage);

        /// <summary>
        /// GetToken.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        Task<string>? GetToken(string? endpointUrl, string? clientId, string? clientSecret);

        /// <summary>
        /// GetScreeningRecord.
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="restURL"></param>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        Task<ScreeningApiResult> GetScreeningRecord(string base64String, string restURL, string contactId, IOrganizationServiceAsync2 serviceClient);

        /// <summary>
        /// SaveResponseToDynamics.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <param name="contactId"></param>
        /// <param name="responseMessage"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        Guid SaveResponseToDynamics(IOrganizationServiceAsync2 serviceClient, string contactId, string responseMessage, out string errorMessage);
    }
}
