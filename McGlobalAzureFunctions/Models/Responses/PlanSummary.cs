// <copyright file="PlanSummary.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class PlanSummary
    {
        public Guid AccountId { get; set; }
        public int OmniId { get; set; }
        public int PlanId { get; set; }
        public DateTime PlanExpiryDate { get; set; }
        public string OpportunityBuyCurrencyName { get; set; }
        public string OpportunitySellCurrencyName { get; set; }
        public decimal SpotRate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public PlanType RppOption { get; set; }
        public Guid BookedBy { get; set; }
        public ReasonsForTrade ReasonsForTrade { get; set; }
        public PlanStatus PlanStatus { get; set; }
        public double BuyAmount { get; set; }
        public double SellAmount { get; set; }
        public double OpportunityMargin { get; set; }
        public string SourceOfFunds { get; set; }
        public bool IsOnline { get; set; }
    }
}