// <copyright file="D365CreateOrUpdateCreditLineRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Models.MEX;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// D365CreateOrUpdateCreditLineRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365CreateOrUpdateCreditLineRequest
    {
        /// <summary>
        /// Gets or Sets D365CreditLineInfo.
        /// </summary>
        public D365CreditLineInfo? CreditLine { get; set; }
    }
}