// <copyright file="DeleteCreditTiersRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// DeleteCreditTiersRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DeleteCreditTiersRequest
    {
        /// <summary>
        /// Gets or Sets the CreditTierIds
        /// </summary>
        public List<Guid> CreditTierIds { get; set; }
    }
}