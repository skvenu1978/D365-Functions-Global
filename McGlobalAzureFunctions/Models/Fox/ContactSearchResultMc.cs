// <copyright file="ContactSearchResultMc.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Fox
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ContactSearchResultMc object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContactSearchResultMc
    {
        /// <summary>
        ///  Gets or sets McId
        /// </summary>
        public Guid? McId { get; set; }

        /// <summary>
        ///  Gets or sets OmniContactId
        /// </summary>
        public string? OmniContactId { get; set; }

        /// <summary>
        /// Gets or sets FirstName
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        ///  Gets or sets LastName
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        ///  Gets or sets Email1
        /// </summary>
        public string? Email1 { get; set; }

        /// <summary>
        ///  Gets or sets Email2
        /// </summary>
        public string? Email2 { get; set; }

        /// <summary>
        ///  Gets or sets CountryCode
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        ///  Gets or sets MobilePhone
        /// </summary>
        public string? MobilePhone { get; set; }

        /// <summary>
        ///  Gets or sets MifidCategoryString
        /// </summary>
        public string? MifidCategoryString { get; set; }

        /// <summary>
        ///  Gets or sets MifidCategory
        /// </summary>
        public int? MifidCategory { get; set; }
    }
}