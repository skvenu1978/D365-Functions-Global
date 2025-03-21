// <copyright file="D365RateTrackerOpportunityInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// D365RateTrackerOpportunityInfo object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365RateTrackerOpportunityInfo
    {
        /// <summary>
        /// Opty Id
        /// </summary>
        public Guid? OpportunityId { get; set; }

        /// <summary>
        /// Account Id related to Opty
        /// </summary>
        public Guid? ClientCRMGuid { get; set; }

        /// <summary>
        /// Gets or Sets Contact Id related to Opty
        /// </summary>
        public Guid? ContactCRMGuid { get; set; }

        /// <summary>
        /// Contact First Name related to Opty
        /// </summary>
        public string? ContactFirstName { get; set; }

        /// <summary>
        /// Gets or Sets Contact Last Name related to Opty
        /// </summary>
        public string? ContactLastName { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public bool IsSendViaSMS { get; set; }

        /// <summary>
        /// Gets or Sets RateAlertOriginatingSystem
        /// </summary>
        public SourceContact RateAlertOriginatingSystem { get; set; }

        /// <summary>
        /// Gets or Sets BaseCCY
        /// </summary>
        public string? BaseCCY { get; set; }

        /// <summary>
        /// Gets or Sets TermsCCY
        /// </summary>
        public string? TermsCCY { get; set; }

        /// <summary>
        /// Gets or Sets Size
        /// </summary>
        public decimal Size { get; set; }

        /// <summary>
        /// Gets or Sets SubscriptionExpiry
        /// </summary>
        public DateTime? SubscriptionExpiry { get; set; }

        /// <summary>
        /// Gets or Sets AlertType
        /// </summary>
        public AlertType AlertType { get; set; }

        /// <summary>
        /// Gets or Sets Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or Sets MobileNumber
        /// </summary>
        public string? MobileNumber { get; set; }

        /// <summary>
        /// Gets or Sets MobileCountryName
        /// </summary>
        public string? MobileCountryName { get; set; }

        /// <summary>
        /// Gets or Sets ClientType
        /// </summary>
        public ClientType ClientType { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public bool DealtIsBase { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public decimal LimitRateLimitedRequested { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public decimal LimitInterbankRate { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public int LimitRateAlertSubscriptionId { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public decimal StopRateLimitedRequested { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public decimal StopInterbankRate { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public int StopRateAlertSubscriptionId { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public bool BaseIsSell { get; set; }

        /// <summary>
        /// Gets or Sets Send via SMS
        /// </summary>
        public ApplicationSource ApplicationSource { get; set; }
    }
}