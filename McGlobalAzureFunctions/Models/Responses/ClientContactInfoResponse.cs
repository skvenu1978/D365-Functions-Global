// <copyright file="ClientContactInfoResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ClientContactInfoResponse
    {
        public ClientInfoResponse ClientResponse { get; set; }
        public ContactInfoResponse ContactResponse { get; set; }
    }
}