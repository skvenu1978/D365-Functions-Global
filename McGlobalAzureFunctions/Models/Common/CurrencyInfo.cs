// <copyright file="CurrencyInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class CurrencyInfo
    {
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public Guid? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public string? CurrencyShortCode { get; set; }
    }
}