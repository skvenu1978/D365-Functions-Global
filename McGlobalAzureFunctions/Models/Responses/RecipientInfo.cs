// <copyright file="RecipientInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Email Recipient
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RecipientInfo
    {
        /// <summary>
        /// Email Recipient Guid
        /// </summary>
        public Guid RecipientGuid { get; set; }

        /// <summary>
        /// Email Recipient Entity Type
        /// </summary>
        public RecipientType RecipientType { get; set; }

        /// <summary>
        /// Email Recipient AlternateEmailAddress
        /// </summary>
        public string? AlternateEmailAddress { get; set; }
    }
}
