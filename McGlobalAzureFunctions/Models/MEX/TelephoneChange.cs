// <copyright file="TelephoneChange.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class TelephoneChange
    {
        public string? CountryCodeMobilePhone { get; set; }
        public string? CountryCodeBusinessPhone { get; set; }
        public string? CountryCodeHomePhone { get; set; }
        public string? MobilePhone { get; set; }
        public string? BusinessPhone { get; set; }
        public string? HomePhone { get; set; }
    }
}