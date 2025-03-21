// <copyright file="ContactInfoResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ContactInfoResponse
    {
        public List<CrmCreateOrUpdateExchangeResults> Results { get; set; }
        public SourceContact SourceContact { get; set; }
    }
}