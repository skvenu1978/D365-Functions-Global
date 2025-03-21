// <copyright file="DealSummary.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class DealSummary
    {
        public Guid AccountId { get; set; }
        public int OmniId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string OpportunityBuyCurrencyName { get; set; }
        public string OpportunitySellCurrencyName { get; set; }
        public double BuyAmount { get; set; }
        public double SellAmount { get; set; }
        public double OpportunityMargin { get; set; }
        public double SpotRate { get; set; }
        public DateTime DealDate { get; set; }
        public int ClientDealId { get; set; }
        public DateTime ValueDate { get; set; }
        public Guid ContactId { get; set; }
        public Guid BookedBy { get; set; }
        public ReasonsForTrade ReasonsForTrade { get; set; }
        public int PlanId { get; set; }
        public ContractType ContractType { get; set; }
        public bool IsAmendment { get; set; }
        public double RemainingBuyAmount { get; set; }
        public double RemainingSellAmount { get; set; }
        public DealStatus DealStatus { get; set; }
        public string SourceOfFunds { get; set; }
        public string FurtherDetails { get; set; }
        public bool IsOnline { get; set; }
    }
}