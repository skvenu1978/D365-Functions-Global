// <copyright file="PersonalDetailsChange.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PersonalDetailsChange
    {
        public Salutation Salutation { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<AttachmentRequest>? Attachments { get; set; }
    }
}