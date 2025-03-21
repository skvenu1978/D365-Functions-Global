// <copyright file="ClientContactInfoRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ClientContactInfoRequest
    {
        public ClientInfoRequest? Client { get; set; }
        public ContactInfoRequest? Contacts { get; set; }
    }
}