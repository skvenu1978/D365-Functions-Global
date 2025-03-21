// <copyright file="ActionResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using McGlobalAzureFunctions.Models.Fox;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ActionResponse Object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ActionResponse
    {
        /// <summary>
        /// Gets or Sets Success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or Sets MfrmAccounts.
        /// </summary>
        public List<AccountSearchResultMc>? MfrmAccounts { get; set; }

        /// <summary>
        /// Gets or Sets MfrmDetailedAccountContacts.
        /// </summary>
        public List<AccountContactDetailedSearchResultMc>? MfrmDetailedAccountContacts { get; set; }

        /// <summary>
        /// Gets or Sets MfrmAccountRelatedContacts.
        /// </summary>
        public List<ContactSearchResultMc>? MfrmAccountRelatedContacts { get; set; }

        /// <summary>
        /// Gets or Sets ErrorMessage.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}