// <copyright file="ILexisNexisUSProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Ekyc
{
    using McGlobalAzureFunctions.Models.Ekyc;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for InstantId LexisNexisUSProvider.
    /// </summary>
    public interface ILexisNexisUSProvider
    {
        /// <summary>
        /// CreateRequest.
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        IDnVRequest? CreateRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage);

        /// <summary>
        /// DoeKYC.
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="restURL"></param>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        Task<ScreeningApiResult> DoeKYC(string base64String, string restURL, string contactId, IOrganizationServiceAsync2 serviceClient);

        /// <summary>
        /// SaveResponseToDynamics
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <param name="contactId"></param>
        /// <param name="responseMessage"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        Guid SaveResponseToDynamics(IOrganizationServiceAsync2 serviceClient, string contactId, string responseMessage, out string errorMessage);
    }
}
