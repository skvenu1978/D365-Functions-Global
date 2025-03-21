// <copyright file="SendAccountAndContactsToOmniRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class SendAccountAndContactsToOmniRequest
    {
        public Guid RequestId { get; set; }
        public Guid AccountId { get; set; }
        public List<Guid> Contacts { get; set; }
    }
}