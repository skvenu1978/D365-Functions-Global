// <copyright file="AccountContactDetailedSearchResultMc.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Fox
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object to capture Search results.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AccountContactDetailedSearchResultMc
    {
        /// <summary>
        /// Gets or sets Account data object.
        /// </summary>
        public AccountDetailedSearchResultMc? Account { get; set; }

        /// <summary>
        /// Gets or sets Contacts Object.
        /// </summary>
        public List<ContactDetailedSearchResultMc>? Contacts { get; set; }
    }
}
