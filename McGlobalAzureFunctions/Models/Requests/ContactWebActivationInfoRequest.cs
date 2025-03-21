// <copyright file="ContactWebActivationInfoRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Models.MEX;

    [ExcludeFromCodeCoverage]
    public class ContactWebActivationInfoRequest
    {
        public List<ContactWebActivationInfo> ContactWebActivations { get; set; }
    }
}