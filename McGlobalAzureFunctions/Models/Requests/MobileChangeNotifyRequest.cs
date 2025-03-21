// <copyright file="MobileChangeNotifyRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class MobileChangeNotifyRequest
    {
        public Guid RequestId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ContactId { get; set; }
        public string OldNumber { get; set; }
        public string NewNumber { get; set; }
    }
}