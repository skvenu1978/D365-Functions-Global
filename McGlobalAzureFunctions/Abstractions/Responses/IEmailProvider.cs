// <copyright file="IEmailProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for Email Requests
    /// </summary>
    public interface IEmailProvider
    {
        /// <summary>
        /// Processes the attachment data
        /// and uploads the file to SharePoint
        /// </summary>
        /// <param name="request">Email request</param>
        /// <param name="requestId">Request Id</param>
        /// <param name="svcClient">Svc Client</param>
        /// <param name="isProd">Is PROD</param>
        /// <param name="logger">logger</param>
        /// <returns>response object</returns>
        DynTaskResponse ProcessEmailActivityRequest(EmailActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateEmailsReader> logger);

        /// <summary>
        /// Processes the attachment data
        /// and uploads the file to SharePoint
        /// </summary>
        /// <param name="request">Letter request</param>
        /// <param name="requestId">Request Id</param>
        /// <param name="svcClient">Svc Client</param>
        /// <param name="logger">logger</param>
        /// <returns>response object</returns>
        DynTaskResponse ProcessLetterActivityRequest(EmailActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateEmailsReader> logger);
    }
}
