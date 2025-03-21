// <copyright file="ContactWebActivationInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ContactWebActivationInfo
    {
        public Guid CRMContactGuid { get; set; }
        public int ClientId { get; set; }
        public int ContactId { get; set; }
    }
}