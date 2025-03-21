// <copyright file="EkyProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Ekyc
{
    using McGlobalAzureFunctions.Models.Ekyc.Experian;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using System;
    using System.Threading.Tasks;

    public interface IExperianProvider
    {
        Task<string?> GetToken(string authUrl, string bodyContent);

        ExperianCCRequest? BuildCCRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage);

        Task<bool> DoeKYCCheck(string token, string contactId, IOrganizationServiceAsync2 serviceClient, string sceeningApi);

        Guid SaveResponseToDynamics(IOrganizationServiceAsync2 serviceClient, string contactId, string responseMessage);

        CrmEKYCResult GetCrmExperianResult(ExperianCCResponse experianResponse);
    }
}
