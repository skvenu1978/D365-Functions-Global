// <copyright file="IAttachmentProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for Attachment Methods.
    /// </summary>
    public interface IAttachmentProvider
    {
        /// <summary>
        /// Processes the attachment data and uploads the file to SharePoint.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="creds">creds</param>
        /// <param name="spSiteName">spSiteName</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessAttachmentRequest(AttachmentRequest request, SpCredentials creds, string spSiteName, Guid requestId, IOrganizationServiceAsync2 svcClient);

       
    }
}