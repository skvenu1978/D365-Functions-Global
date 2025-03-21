// <copyright file="AddressChangeInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Models.MEX;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class AddressChangeInfo
    {
        public AddressChange? AddressChangeData { get; set; }

        public Guid ContactId { get; set; }

        public Guid AddressChangeId { get; set; }
    }
}