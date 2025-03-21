// <copyright file="ContactDetailedSearchResultMc.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Fox
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ContactDetailedSearchResultMc
    {
        public Guid? McId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email1 { get; set; }
        public string? Email2 { get; set; }
        public string? Address_City { get; set; }

        public string? Address_Country { get; set; }
        public string? Address_County { get; set; }
        public string? Address_Line_1 { get; set; }
        public string? Address_Line_2 { get; set; }
        public string? Address_Line_3 { get; set; }

        public string? Address_Postal_Code { get; set; }
        public string? Fax { get; set; }
        public string? Home_Country_Code { get; set; }
        public string? Home_Phone { get; set; }
        public string? Mobile_Country_Code { get; set; }

        public string? Mobile_Phone { get; set; }
        public int? OmniId { get; set; }
        public string? Other_Country_Code { get; set; }
        public string? Other_Phone { get; set; }
        public string? Title { get; set; }

        public string? Work_Country_Code { get; set; }
        public string? Work_Phone { get; set; }
        public int? ClientType { get; set; }
        public bool? DoNotEmail { get; set; }
        public bool? DoNotPostalMail { get; set; }
    }
}