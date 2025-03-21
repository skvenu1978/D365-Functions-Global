// <copyright file="ClientInfoResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ClientInfoResponse
    {
        public Guid AccountId { get; set; }
        public int ClientId { get; set; }
        public string ReferenceKey { get; set; }
    }
}