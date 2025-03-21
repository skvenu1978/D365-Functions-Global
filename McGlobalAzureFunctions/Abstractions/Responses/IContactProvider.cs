// <copyright file="IContactProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// IContactProvider.
    /// </summary>
    public interface IContactProvider
    {
        /// <summary>
        /// ProcessContactInfoResponse.
        /// </summary>
        /// <param name="omniResponse">omniResponse</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessContactInfoResponse(ContactInfoResponse omniResponse, Guid requestId, IOrganizationServiceAsync2 svcClient);

        /// <summary>
        /// UpdateUserPrivileges.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse UpdateUserPrivileges(UserPrivilegesInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient);

        /// <summary>
        /// UpdateContactActivation.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse UpdateContactActivation(ContactActivatingWorkflowInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateClientsReader> logger);

        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        DynTaskResponse UpdateOmniRefAndCheckForErrors(ContactWebActivationInfoResponse omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger);
    }
}
