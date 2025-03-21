// <copyright file="UpdateCreditTierRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Models.MEX;

    [ExcludeFromCodeCoverage]
    public class UpdateCreditTierRequest
    {
        public Guid CreditTierId { get; set; }
        public D365CreditTierInfo CreditTier { get; set; }
    }
}