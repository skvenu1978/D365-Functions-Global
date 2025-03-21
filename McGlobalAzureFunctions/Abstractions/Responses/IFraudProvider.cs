// <copyright file="IFraudProvider.cs" company="moneycorp">
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
    /// Fraud Client Provider Interface.
    /// </summary>
    public interface IFraudProvider
    {
        /// <summary>
        /// reads the request from OMNI and updates D365.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns></returns>
        DynTaskResponse ProcessFraudClientAuthorisationStatusChangeRequest(FraudClientAuthorisationStatusChangeRequest omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger);
    }
}
