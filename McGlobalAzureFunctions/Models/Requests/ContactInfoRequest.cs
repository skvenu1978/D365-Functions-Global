// <copyright file="ContactInfoRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Models.MEX;

    [ExcludeFromCodeCoverage]
    public class ContactInfoRequest
    {
        public List<ContactInfoDetails>? ContactsInfoDetails { get; set; }
        public string? SourceContact { get; set; }
    }
}
