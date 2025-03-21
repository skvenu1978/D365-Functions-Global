// <copyright file="IActivityProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Responses
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// IActivityProvider.
    /// </summary>
    public interface IActivityProvider
    {
        /// <summary>
        /// ProcessCrmActivity.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        DynTaskResponse ProcessCrmActivity(CrmActivity request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnAttachmentActivityReader> logger);
    }
}
