// <copyright file="UpdateContactDetailsRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Models.MEX;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class UpdateContactDetailsRequest
    {
        public Guid ContactId { get; set; }
        public AddressChange AddressChange { get; set; }
        public PersonalDetailsChange PersonalDetailsChange { get; set; }
        public TelephoneChange TelephoneChange { get; set; }
        public EmailChange EmailChange { get; set; }
    }
}