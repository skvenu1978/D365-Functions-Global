// <copyright file="ReadMarketingPreferenceRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ReadMarketingPreferenceRequest
    {
        public Guid ContactId { get; set; }
        public bool IsProductFeaturesTips { get; set; }
        public bool IsMarketNewsRateAlerts { get; set; }
        public bool IsExclusiveOffersPromotions { get; set; }
        public bool IsMarketingEmail { get; set; }
        public bool IsMarketingPost { get; set; }
        public bool IsMarketingSms { get; set; }
        public bool IsMarketingTelephone { get; set; }
        public bool IsMarketingDisplayNotifications { get; set; }
        public bool IsPostOfficePreference { get; set; }
        public bool IsTravelMoneyPreference { get; set; }
        public bool IsThirdPartyEmail { get; set; }
        public bool IsThirdPartySms { get; set; }
        public bool IsThirdPartyTelephone { get; set; }
        public bool IsThirdPartyPost { get; set; }
    }
}