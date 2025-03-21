// <copyright file="AccountSearchResultMc.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Fox
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AccountSearchResultMc Object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AccountSearchResultMc
    {
        /// <summary>
        /// Gets or Sets McId.
        /// </summary>
        public Guid McId { get; set; }

        /// <summary>
        /// Gets or Sets AccountDealerFullName.
        /// </summary>
        public string? AccountDealerFullName { get; set; }

        /// <summary>
        /// Gets or Sets AccountDealerId.
        /// </summary>
        public Guid? AccountDealerId { get; set; }

        /// <summary>
        /// Gets or Sets AccountExecutiveFullName.
        /// </summary>
        public string? AccountExecutiveFullName { get; set; }

        /// <summary>
        /// Gets or Sets AccountExecutiveId.
        /// </summary>
        public Guid? AccountExecutiveId { get; set; }

        /// <summary>
        /// Gets or Sets ClientType.
        /// </summary>
        public int? ClientType { get; set; }

        /// <summary>
        /// Gets or Sets FedsId.
        /// </summary>
        public int? FedsId { get; set; }

        /// <summary>
        /// Gets or Sets MifidCategory.
        /// </summary>
        public int? MifidCategory { get; set; }

        /// <summary>
        /// Gets or Sets MifidCategoryString.
        /// </summary>
        public string? MifidCategoryString { get; set; }

        /// <summary>
        /// Gets or Sets Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or Sets OwningBusiness.
        /// </summary>
        public int? OwningBusiness { get; set; }

        /// <summary>
        /// Gets or Sets ParentAccountId.
        /// </summary>
        public Guid? ParentAccountId { get; set; }
    }
}