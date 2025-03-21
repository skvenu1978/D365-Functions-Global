// <copyright file="DeleteCreditTierInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class DeleteCreditTierInfo
    {
        public Guid CreditTierId { get; set; }
    }
}