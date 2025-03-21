// <copyright file="D365CreateOrUpdateCreditTierRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Models.MEX;

    /// <summary>
    /// D365CreateOrUpdateCreditTierRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365CreateOrUpdateCreditTierRequest
    {
        /// <summary>
        /// Gets or Sets D365CreditTierInfo.
        /// </summary>
        public D365CreditTierInfo? CreditTier { get; set; }
    }
}
