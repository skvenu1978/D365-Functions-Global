// <copyright file="ClientInfoRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ClientInfoRequest
    {
        public Guid AccountId { get; set; }
        public int CrmId { get; set; }
        public Guid CRMAccountGuid { get; set; }
        public bool IsActive { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid? PrimaryCRMContactGuid { get; set; }
        public Guid? MFRMAccountGuid { get; set; }
        public Guid? TTTAccountGuid { get; set; }
        public string? LegalEntityIdentifier { get; set; }
        public ClientType ClientType { get; set; }
        public CustomerTypeCode RelationshipType { get; set; }
        public TradingStatus TradingStatus { get; set; }
        public ClientLanguage ClientLanguage { get; set; }
        public string? ClientName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }
        public string? Country { get; set; }
        public string? CountryOfResidence { get; set; }
        public bool IsAuthorised { get; set; }
        public bool IsAwaitingClientCategoryLetter { get; set; }
        public bool IsGPSAccountSetup { get; set; }
        public bool IsGPSClientSignedTCs { get; set; }
        public bool IsGPSProspect { get; set; }
        public ClientStatus ClientStatus { get; set; }
        public TTFeePaymentInMethod TTFeePaymentInMethod { get; set; }
        public TTFeeStructure TTFeeStructure { get; set; }
        public ContactMethod ContactMethodPreference { get; set; }
        public string? Duns { get; set; }
        public int FedsId { get; set; }
        public Guid? OwnerId { get; set; }
        public bool SendEmail { get; set; }
        public bool SendMail { get; set; }
        public bool SendSMS { get; set; }
        public Guid? AccountExecutiveGuidLookup { get; set; }
        public Guid? DealerIdGuidLookup { get; set; }
        public Guid? MarketingCampaignGuidLookup { get; set; }
        public Guid? PrimaryReferringPartnerGuidLookup { get; set; }
        public MCOnlineSystem System { get; set; }
        public bool IsSpot { get; set; }
        public bool IsForward { get; set; }
        public decimal DebitCard { get; set; }
        public bool IsEnableDebitCard { get; set; }
        public decimal CreditCard { get; set; }
        public bool IsEnableCreditCard { get; set; }
        public decimal DirectDebit { get; set; }
        public bool IsEnableDirectDebit { get; set; }
        public decimal TradeLimit { get; set; }
        public decimal AggregateLimit { get; set; }
        public decimal Spread { get; set; }
        public decimal DirectDebitAggregateLimit { get; set; }
        public DirectDebitType DirectDebitType { get; set; }
        public ClientAccountModelType ClientAccountModelType { get; set; }
        public int? ParentCrmId { get; set; }
        public bool SendNotifications { get; set; }
        public string? NotificationContactEmail { get; set; }
        public Guid? AccountOpeningWebUserId { get; set; }
        public ApplicationSource ApplicationSource { get; set; }
        public string? LegacySystemId { get; set; }
        public LegacySystemName LegacySystemName { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public bool FinancialInstitution { get; set; }
        public bool HighValueDealer { get; set; }
        public bool MoneyServiceBusiness { get; set; }
        public bool HasLoyaltyCard { get; set; }
        public LoyaltyCardType LoyaltyCardType { get; set; }
        public MifidCategory MifidCategory { get; set; }
        public string? DefaultCurrency { get; set; }
        public string? DirectDebitCurrency { get; set; }
        public string? DefaultCountry { get; set; }
        public PaymentClassification PaymentClassification { get; set; }
        public bool IsForwardQuestionnaire { get; set; }
        public bool BulkPaymentsEnabled { get; set; }
        public bool ReverseWireEnabled { get; set; }
        public decimal PaymentInPerTransactionLimit { get; set; }
        public decimal PaymentOutPerTransactionLimit { get; set; }
        public bool IsCloudElementEnabled { get; set; }
        public bool IsDealerIncomingFundEnabled { get; set; }
        public bool IsPriorityCustomer { get; set; }
        public bool EditPaymentDate { get; set; }
    }
}