// <copyright file="AccountDetailedSearchResultMc.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Fox
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object for holding detailed search results.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AccountDetailedSearchResultMc
    {
        /// <summary>
        /// Gets or Sets Entity Id field.
        /// </summary>
        public Guid McId { get; set; }

        /// <summary>
        /// Gets or Sets Name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or Sets Address_City.
        /// </summary>
        public string? Address_City { get; set; }

        /// <summary>
        /// Gets or Sets Address_Country.
        /// </summary>
        public string? Address_Country { get; set; }

        /// <summary>
        /// Gets or Sets Address_County.
        /// </summary>
        public string? Address_County { get; set; }

        /// <summary>
        /// Gets or Sets Address_Line_1.
        /// </summary>
        public string? Address_Line_1 { get; set; }

        /// <summary>
        /// Gets or Sets Address_Line_2.
        /// </summary>
        public string? Address_Line_2 { get; set; }

        /// <summary>
        /// Gets or Sets Address_Line_3.
        /// </summary>
        public string? Address_Line_3 { get; set; }

        /// <summary>
        /// Gets or Sets Address_Postal_Code.
        /// </summary>
        public string? Address_Postal_Code { get; set; }

        /// <summary>
        /// Gets or Sets CRM_Customer_ID.
        /// </summary>
        public string? CRM_Customer_ID { get; set; }

        /// <summary>
        /// Gets or Sets Email_1.
        /// </summary>
        public string? Email_1 { get; set; }

        /// <summary>
        /// Gets or Sets Email_2.
        /// </summary>
        public string? Email_2 { get; set; }

        /// <summary>
        /// Gets or Sets Fax on Account.
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// Gets or Sets Main Phone on Account.
        /// </summary>
        public string? Main_Phone { get; set; }

        /// <summary>
        /// Gets or Sets Main Phone short code on Account.
        /// </summary>
        public string? Main_Phone_Alpha2 { get; set; }

        /// <summary>
        /// Gets or Sets Mobile Phone on Account.
        /// </summary>
        public string? Mobile_Phone { get; set; }

        /// <summary>
        /// Gets or Sets Mobile Phone short code on Account.
        /// </summary>
        public string? Mobile_Phone_Alpha2 { get; set; }

        /// <summary>
        /// Gets or Sets Other_Phone.
        /// </summary>
        public string? Other_Phone { get; set; }

        /// <summary>
        /// Gets or Sets Other_Phone Short Code.
        /// </summary>
        public string? Other_Phone_Alpha2 { get; set; }

        /// <summary>
        /// Gets or Sets Primary_Contact_Name.
        /// </summary>
        public string? Primary_Contact_Name { get; set; }

        /// <summary>
        /// Gets or Sets Primary_Contact_ID.
        /// </summary>
        public Guid? Primary_Contact_ID { get; set; }

        /// <summary>
        /// Gets or Sets OmniId.
        /// </summary>
        public string? OmniId { get; set; }

        /// <summary>
        /// Gets or Sets AccountDealerId.
        /// </summary>
        public Guid? AccountDealerId { get; set; }

        /// <summary>
        /// Gets or Sets AccountDealerFullName.
        /// </summary>
        public string? AccountDealerFullName { get; set; }

        /// <summary>
        /// Gets or Sets AccountExecutiveId.
        /// </summary>
        public Guid? AccountExecutiveId { get; set; }

        /// <summary>
        /// Gets or Sets AccountExecutiveFullName.
        /// </summary>
        public string? AccountExecutiveFullName { get; set; }

        /// <summary>
        /// Gets or Sets MifidCategory.
        /// </summary>
        public int? MifidCategory { get; set; }

        /// <summary>
        /// Gets or Sets MifidCategoryString.
        /// </summary>
        public string? MifidCategoryString { get; set; }

        /// <summary>
        /// Gets or Sets ClientType.
        /// </summary>
        public int? ClientType { get; set; }
    }
}