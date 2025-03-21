// <copyright file="AddressChange.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AddressChange Object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AddressChange
    {
        /// <summary>
        /// Gets or Sets AddressLine1.
        /// </summary>
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// Gets or Sets AddressLine2.
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Gets or Sets AddressLine3.
        /// </summary>
        public string? AddressLine3 { get; set; }

        /// <summary>
        /// Gets or Sets City.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or Sets County.
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// Gets or Sets PostCode.
        /// </summary>
        public string? PostCode { get; set; }

        /// <summary>
        /// Gets or Sets CountryAlpha3.
        /// </summary>
        public string? CountryAlpha3 { get; set; }

        /// <summary>
        /// Gets or Sets Attachments.
        /// </summary>
        public List<AttachmentRequest>? Attachments { get; set; }
    }
}