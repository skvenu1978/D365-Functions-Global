// <copyright file="Enums.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const.Enums
{
    using System.Runtime.Serialization;

    /// <summary>
    /// CreditLineStatus.
    /// </summary>
    public enum CreditLineStatus : int
    {
        /// <summary>
        /// Created.
        /// </summary>
        Created = 1,

        /// <summary>
        /// Inactive.
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// Submitted.
        /// </summary>
        Submitted = 3,

        /// <summary>
        /// SentToCreditCommittee.
        /// </summary>
        SentToCreditCommittee = 4,

        /// <summary>
        /// Approved
        /// </summary>
        Approved = 5,

        /// <summary>
        /// Rejected.
        /// </summary>
        Rejected = 6,

        /// <summary>
        /// Cancelled.
        /// </summary>
        Cancelled = 7,

        /// <summary>
        /// RejectedByCreditTeam.
        /// </summary>
        RejectedByCreditTeam = 8,

        /// <summary>
        /// Withdrawn.
        /// </summary>
        Withdrawn = 9,
    }

    public enum RecipientType
    {
        Account = 1,
        Contact = 2,
        SystemUser = 3,
        Queue = 4
    }
    public enum McAccountStatus
    {
        AwaitingTradingAgreement = 1,
        Authorised = 100000000,
        DeclinedAuthorisation = 100000001,
        Unauthorised = 100000002,
        AwaitingOnlineActivation = 100000003,
        Closed = 100000004,
        Inactive = 2,
        AwaitingClientCategoryLetter = 100000005,
        PrivateEventual = 100000006
    }

    public enum ContactStateCode
    {
        Active = 0,
        Inactive = 1
    }

    public enum MoneycorpAddressType
    {
        Primary = 1,
        Ship_To = 2,
        Bill_To = 3,
        Branch = 4,
        Referral_Coverage = 5,
        Registered = 7,
        Other = 6,
        Historic = 8,
    }

    public enum MoneycorpAddressObjectType
    {
        Account = 1,
        Contact = 2,
        Lead = 3
    }

    public enum FileSource
    {
        CRM = 1,
        OMNI = 2
    }

    public enum AttachmentType
    {
        /// <summary>
        /// MCOLAuthorizationDocument
        /// </summary>
        [EnumMember(Value = "MCOL Authorization Document")]
        MCOLAuthorizationDocument = 0,

        /// <summary>
        /// MCOLDocument
        /// </summary>
        [EnumMember(Value = "MCOL Document")]
        MCOLDocument = 1,
        [EnumMember(Value = "MCOL Proof of identity")]
        ProofOfIdentity = 2,
        [EnumMember(Value = "MCOL Proof of address")]
        ProofOfAddress = 3,
        [EnumMember(Value = "Email attachment")]
        EmailAttachment = 4,
        [EnumMember(Value = "Letter attachment")]
        LetterAttachment = 5,
        [EnumMember(Value = "Therefore Cfx attachment")]
        ThereforeCfxAttachment = 6
    }

    /// <summary>
    /// MCOLActivationStatus.
    /// </summary>
    public enum MCOLActivationStatus
    {
        NotActivated = 1,
        Activating = 2,
        Activated = 3
    }

    /// <summary>
    /// ContractType.
    /// </summary>
    public enum ContractType
    {
        Spot = 1,
        Forward = 2,
        RPP = 3
    }

    /// <summary>
    /// RppOption.
    /// </summary>
    public enum RppOption
    {
        Fixed_Settlement = 1,
        Fixed_Local = 2,
        Fixed = 3
    }

    /// <summary>
    /// SourceOfConsent.
    /// </summary>
    public enum SourceOfConsent
    {
        Online = 100000000,
        Telephone = 100000001,
        ApplicationForm = 100000002,
        Email = 100000003,
        App = 100000004,
        InWriting = 100000005,
        SMS = 100000006
    }

    /// <summary>
    /// OpportunityType.
    /// </summary>
    public enum OpportunityType
    {
        CallInTrade = 6,
        FirmLimit = 4,
        FirmOCO = 8,
        OCO = 13,
        FirmStopLoss = 7,
        MarketWatch = 3,
        OpenOnlineFacility = 9,
        OpenRequirement = 2,
        OpenTradingFacility = 1,
        RPPRequirement = 5,
        Deposit = 10,
        RateTracker = 11,
        OnlineTrade = 12
    }

    /// <summary>
    /// BaseCurrency.
    /// </summary>
    public enum BaseCurrency
    {
        /// <summary>
        /// Buy.
        /// </summary>
        Buy = 1,

        /// <summary>
        /// Sell.
        /// </summary>
        Sell = 2
    }

    /// <summary>
    /// StopOrLimit.
    /// </summary>
    public enum StopOrLimit
    {
        /// <summary>
        /// Stop.
        /// </summary>
        Stop = 1,

        /// <summary>
        /// Limit.
        /// </summary>
        Limit = 2
    }

    /// <summary>
    /// ReasonsForTrade.
    /// </summary>
    public enum ReasonsForTrade
    {
        Emigration = 1,
        PropertyPurchaseOrSale = 2,
        Other = 3,
        BusinessTransactions = 4,
        RegularPayments = 5,
        AdHocPayments = 6,
        LicensedEntity = 7,
        FamilyFriendPayment = 8,
        LivingExpenses = 10,
        PropertyMaintenance = 11,
        PropertyRental = 12,
        MortgagePayment = 13,
        PensionPayment = 14,
        Repatriation = 15,
        SalaryTransfer = 16,
        TuitionFees = 17,
        VehiclePurchase = 18,
        WeddingPayment = 19,
        HolidayExpenses = 21,
        MedicalCosts = 22,
        PurchaseOfGoodsServices = 25,
        Gift = 26,
        TaxBill = 27,
        BrazilianTaxDocument = 28,
        ServicesGoodsDirectInvestment = 29,
        BalanceSheetHedging = 30,
        Grants = 31,
        PurchaseOfGoodsGroceriesPropertyDeposit = 32,
        PurchaseOfServicesUtilitiesServiceCharge = 33,
        TransferToAccountOverseas = 34,
        ChildMaintenanceAlimony = 35,
        GiftNoPurchaseOfGoodsServiceBills = 36
    }
    public enum PlanType
    {
        Fixed = 1,
        FixedLocal = 2, 
        FixedSettlement = 3
    }

    public enum SummaryType
    {
        Deal = 1,
        Plan = 2
    }
    public enum Source
    {
        Online = 1,
        Phone = 2
    }
    public enum task_prioritycode
    {
        Low = 0,
        Normal = 1,
        High = 2
    }

    public enum CreditTierType : int
    {
        CreditTier = 1,
    }
    public enum PlanStatus
    {
        None = 0, // NB. Not serialised
        Created = 1,
        Active = 2,
        Inactive = 3,
        Cancelled = 4,
        Reversed = 5,
        Negotiating = 6,
        Completed = 7
    }
    public enum DealStatus
    {
        None = 0, // NB. Not serialised
        Created = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4,
        ClosedOut = 5
    }

    /// <summary>
    /// OpportunityState.
    /// </summary>
    public enum OpportunityState
    {
        Open = 0,
        Won = 1,
        Lost = 2,
    }

    /// <summary>
    /// OpportunityStatus.
    /// </summary>
    public enum OpportunityStatus
    {
        Open_OnHold = 100000005,
        Open_InProgress = 100000004,
        Open_Approved = 1,
        Open_Rejected = 2,
        Won_ClosedasApproved = 100000002,
        Won_Closed = 3,
        Won_OrderFilled = 100000006,
        Won_Traded = 100000007,
        Won_TradingFacilityOpened = 100000008,
        Won_Active = 100000009,
        Won_Inactive = 100000010,
        Won_Completed = 100000011,
        Lost_ClosedasRejected = 100000003,
        Lost_Canceled = 4,
        Lost_CancelledRequirement = 100000012,
        Lost_TradedElsewhere = 100000013,
        Lost_CannotContact = 100000014,
        Lost_Duplicated = 100000015,
        Lost_BulkDisqualify = 100000016,
        Lost_CoveredbyOverseasMortgage = 100000017,
        Lost_CoveredbyExistingForeignAssets = 100000018,
        Lost_OrderCancelled = 100000019,
        Lost_CannotTradeCurrency = 100000020
    }

    /// <summary>
    /// OpportunitySource.
    /// </summary>
    public enum OpportunitySource
    {
        CRM = 1,
        Website = 2,
        MCOL = 3
    }

    /// <summary>
    /// MarketWatchNotificationMethod.
    /// </summary>
    public enum MarketWatchNotificationMethod
    {
        Email = 1,
        EmailandSMS = 2,
        SMS = 3
    }

    /// <summary>
    /// CrmEntitySource.
    /// </summary>
    public enum CrmEntitySource
    {
        Contact = 1,
        Client = 2,
    }

    /// <summary>
    /// CrmActivityType.
    /// </summary>
    public enum CrmActivityType
    {
        Task = 1,
        PhoneCall = 2,
    }
    public enum CrmActivityState
    {
        Open = 1,
        Closed = 2,
    }
    public enum DirectDebitType
    {
        None = 1,
        OneDay = 2,
        ThreeDay = 3,
        FiveDay = 4,
    }
    public enum MCOnlineSystem : int
    {
        Basic = 1,
        Advanced = 2
    }
    public enum NotificationMethod
    {
        None = 0,
        Email = 1,
        Sms = 2,
        EmailSms = 3
    }
    public enum AccountOpeningStatusReason : int
    {
        Pass = 1,
        FailExperian = 2,
        FailFraudCheck = 3
    }
    public enum BusinessEntityType : int
    {
        Client = 10,
        Contact = 35
    }
    public enum ContactMethod : int
    {
        Any = 1,
        Email = 2,
        Phone = 3,
        Fax = 4,
        Mail = 5,
        None = 6,
    }
    public enum ClientType : int
    {
        Private = 1,
        ReferringPartner = 2,
        Corporate = 3,
    }
    public enum TradingStatus : int
    {
        None = 0,
        AccountClosed = 3,
        ActivelyTrading = 10,
        NeverNeededOurServices = 6,
        NoResponse = 9,
        NotBuyingAnymore = 5,
        NotYetTraded = 1,
        SmallDeals = 8,
        TradedElsewhere = 7,
        TradedinPart = 2,
        ReferredtoGPS = 11,
        NeedsRepitch = 12,
        Online = 19,
        Dormant = 20,
        AccountSuspended = 4,
        InsufficientKYC = 21
    }
    public enum McCustomerType
    {
        TMO_PRIVATE,
        TMO_CORPORATE,
        CFX_PRIVATE,
        CFX_CORPORATE,
        CFX_RP,
        WHOLESALE_PRIVATE,
        WHOLESALE_CORPORATE,
        MM_PRIVATE,
        MM_CORPORATE,
        MM_Advisory,
        SUPPLIER,
        MONEYCORP_PTY
    }
    public enum CustomerTypeCode : int
    {
        None = 0,
        Customer = 3,
        ReferringPartner = 5,
        Prospect = 8,
        Competitor = 1,
        Consultant = 2,
        Investor = 4,
        Influencer = 6,
        Press = 7,
        Reseller = 9,
        Supplier = 10,
        Vendor = 11,
        Other = 12,
        OnlineSeller = 13
    }
    public enum ClientLanguage : int
    {
        English = 1,
        Spanish = 2,
        French = 3,
        Romanian = 4,
    }
    public enum McClientLanguage
    {
        NotSet = 0,
        English = 1,
        Spanish = 2,
        French = 3,
        Romanian = 4
    }
    public enum McOwningBusiness
    {
        Moneycorp = 100000000,
        PostOffice = 100000001,
        ThomasCook = 100000002,
        Tempus = 100000003,
        MoneycorpPTY = 100000004,
        TMO = 100000005,
        Explorer = 100000006,
        ReferringPartner = 100000007,
        MoneycorpWholesale = 100000020,
        MoneycorpRetail = 100000009,
        MoneycorpMFRM = 100000010,
        MoneycorpBank = 100000011,
        NovoMundoCC = 100000012,
        MoneycorpSpain = 100000013,
        MoneycorpRomania = 100000014,
        SpainFRM = 100000015,
        Sainsbury = 100000017,
        MoneycorpUSInc = 100000018,
        MoneycorpHongKong = 100000019,
        MoneycorpEuropeIreland = 100000021,
        MoneycorpEurope = 100000022,
        MoneycorpEuropeSpain = 100000023,
        MoneycorpEuropeRomania = 100000024,
        MoneycorpBrazil = 100000025,
        MoneycorpBankMIFID = 100000026,
        MIFIDMoneycorpEuropeIreland = 100000027,
        MIFIDMoneycorpEuropeSpain = 100000028,
        MIFIDMoneycorpEurope = 100000029,
        MoneycorpEuropeFrance = 100000030
    }
    public enum PaymentClassification : int
    {
        None = 0,
        MSBPSPInstitution = 1,
        WhiteListCorporates = 2,
    }
    public enum MifidCategory : int
    {
        None = 0,
        RetailForwardOnly = 1,
        RetailBasic = 2,
        Retail = 3,
        ElectedProfessional = 4,
        Professional = 5,
        ElectedEligibleCounterparty = 6,
        EligibleCounterparty = 7,
        ProfessionalIncTARFs = 8,
        ElectedProfessionalInTARFs = 9
    }
    public enum ClientStatus : int
    {
        STP = 1,
        NonSTP = 2,
    }
    public enum TTFeePaymentInMethod : int
    {
        DirectDebit = 1,
        SelfFund = 2,
    }
    public enum OriginatingSystem
    {
        Unknown = 0,
        OmniWeb = 1,
        Manual = 2,
        GPS = 3,
    }
    public enum PaymentCategory
    {
        None = 0,
        CfxPaymentGateway = 1,
        RPP = 2,
        GPSPaymentGateway = 3,
        OldCFXPaymentGateway = 4,
        OldGPSPaymentGateway = 5
    }
    public enum PaymentChannel
    {
        SWIFT = 1,
        DirectDebit = 2,
        Tempo = 3,
        DebitCard = 4,
        CreditCard = 5,
        Cheque = 6,
        Unknown = 7,
        Internal = 8,
        BACS = 9,
        CHAPS = 10
    }
    public enum TTFeeStructure : int
    {
        Invoice = 1,
        PayAsYouGo = 2,
    }
    public enum AuthorisationStatus : int
    {
        None = 0,
        Authorised = 1,
        Unauthorised = 2,
        Rejected = 3,
        AwaitingDocuments = 4,
    }
    public enum McAuthorizationStatusReason
    {
        Pass = 1,
        FailExperian = 2,
        FailFraudCheck = 3
    }
    public enum Salutation : int
    {
        None = 0,
        Mr = 1,
        Mrs = 2,
        Miss = 3,
        Ms = 4,
        Sir = 5,
        Dr = 6,
        Professor = 7,
        Dame = 8,
        Lady = 9,
        Rev = 10,
        Lord = 11,
        Viscount = 12,
        Viscountess = 13,
    }
    public enum ContactWebAccessStatus : int
    {
        NotActivated = 1,
        Activating = 2,
        Activated = 3,
    }
    public enum ClientAlertSubjectType
    {
        None = 0,
        NewCardAddress = 1
    }
    public enum ClientAlertType : int
    {
        Warning = 1,
        Information = 2,
        Critical = 3,
    }
    public enum AlertType : int
    {
        Below = 1,
        Above = 2,
        OneCancelsOther = 3
    }
    public enum ClientAccountModelType
    {
        None = 0,
        MetaAccount = 1,
        SubAccount = 2
    }
    public enum McClientModule
    {
        Standard = 1,
        Managing = 2,
        Managed = 3
    }
    public enum ApplicationSource
    {
        Moneycorp = 100000000,
        JanusPostOffice = 100000001,
        ThomasCook = 100000002,
        Tempus = 100000003,
        MoneycorpBank = 100000011,
        SpainMFRM = 100000015,
        MoneycorpRomania = 100000014,
        NovoMundoCC = 100000012,
        MoneycorpPTY = 100000004,
        Sainsbury = 100000017,
        MoneycorpMFRM = 100000010,
        MoneycorpUSInc = 100000018,//100000024
        MoneycorpEuropeRomania = 100000024,
        MoneycorpEuropeIreland = 100000021,
        MoneycorpEurope = 100000022,
        MoneycorpEuropeSpain = 100000023,
        MoneycorpBrazil = 100000025,
        MoneycorpBankMIFID = 100000026,
        MIFIDMoneycorpEuropeIreland = 100000027,
        MIFIDMoneycorpEuropeSpain = 100000028,
        MIFIDMoneycorpEurope = 100000029,
        MoneycorpEuropeFrance = 100000030,
    }
    public enum LegacySystemName
    {
        None = 100000000,
        BVD = 100000001,
        Commonwealth = 100000002,
        FirstRate = 100000003,
        EuropeMigration = 100000004
    }
    public enum LoyaltyCardType
    {
        None = 0,
        Nectar = 1
    }
    public enum FNDARequestType
    {
        Create = 1,
        Update = 2,
        CheckExperian = 3,
        FullClosure = 4,
        UploadDocNotifyCompliance = 5
    }
    public enum ExperianStatus
    {
        None = 0,
        Pass = 1,
        Refer = 2,
        Fail = 3,
        Error = 4
    }
    public enum IdentityType
    {
        Others = 0,
        Passport = 1,
        DrivingLicense = 2,
        BankStatement = 3,
        NationalIdentityCard = 4,
        IrTaxNotification = 5,
        ShotGunLicense = 6,
        SocialSecurityNumber = 7,
        UsTemporaryDriverLicense = 8,
        ResidentAlienCard = 9,
        IDCard = 10,
        OldversionofROIDCard = 11,
        TemporaryID = 12,
        UsStateIdentificationCard = 13,
        None = 14,
    }
    public enum IdentityStatus
    {
        None = 0,
        Required = 1,
        NotRequired = 2,
        Recieved = 3
    }
    //public enum McAccountStatus
    //{
    //    AwaitingTradingAgreement = 1,
    //    Authorised = 100000000,
    //    DeclinedAuthorisation = 100000001,
    //    Unauthorised = 100000002,
    //    AwaitingOnlineActivation = 100000003,
    //    Closed = 100000004,
    //    Inactive = 2,
    //    AwaitingClientCategoryLetter = 100000005,
    //    PrivateEventual = 100000006
    //}

    public enum D365AccountStatus
    {
        AwaitingTradingAgreement = 1,
        Authorised = 256550001,
        DeclinedAuthorisation = 100000001,
        Unauthorised = 256550002,
        AwaitingOnlineActivation = 100000003,
        Closed = 100000004,
        Inactive = 2,
        AwaitingClientCategoryLetter = 100000005,
        PrivateEventual = 100000006,
        Suspended_Trigger_Event = 100000007,
        Suspended_OutstandingKyc = 100000008,
        Suspended_Inactivity = 100000009,
    }

    public enum McContactStatus
    {
        Active_Active = 1,
        Active_AwaitingExperian = 3,
        Active_AwaitingDocuments = 4,
        Active_AwaitingAuthorisation = 5,
        Active_AwaitingDocumentProduction = 6,
        Active_Incomplete = 7,
        Inactive_Inactive = 2
    }
    public enum McCurrency
    {
        NotSet = 0,
        AUD = 4,
        BBD = 5,
        BDT = 6,
        BGN = 8,
        BHD = 9,
        BMD = 10,
        BND = 11,
        BSD = 13,
        BWP = 14,
        BZD = 15,
        CAD = 16,
        CHF = 17,
        CLP = 18,
        CNY = 19,
        COP = 20,
        CRC = 21,
        CZK = 23,
        DKK = 25,
        DOP = 26,
        EEK = 27,
        EGP = 28,
        EUR = 30,
        FJD = 32,
        FKP = 33,
        GBP = 36,
        GIP = 37,
        GMD = 38,
        HKD = 40,
        HRK = 41,
        HUF = 42,
        IDR = 43,
        ILS = 45,
        INR = 46,
        ISK = 47,
        JMD = 49,
        JOD = 50,
        JPY = 51,
        KES = 52,
        KRW = 53,
        KWD = 54,
        KYD = 55,
        LBP = 56,
        LKR = 57,
        LTL = 58,
        LVL = 60,
        MAD = 61,
        MUR = 63,
        MXN = 64,
        MYR = 65,
        NGN = 66,
        NOK = 68,
        NZD = 69,
        OMR = 70,
        PGK = 71,
        PHP = 72,
        PKR = 73,
        PLN = 74,
        QAR = 76,
        RUB = 77,
        SAR = 78,
        SCR = 79,
        SEK = 80,
        SGD = 81,
        SYP = 84,
        THB = 85,
        TND = 86,
        TRL = 87,
        TTD = 88,
        TWD = 89,
        TZS = 90,
        USD = 91,
        USV = 92,
        VEB = 93,
        XAF = 94,
        XCD = 95,
        XPF = 96,
        XUS = 97,
        ZAR = 98,
        ZWD = 99,
        TRY = 100,
        NAD = 101,
        MVR = 102,
        ANG = 103,
        RON = 105,
        AED = 106,
        ARS = 107,
        BRL = 108,
        SKK = 109,
        UGX = 110,
    }
    public enum McAlertStatus
    {
        Active = 1,
        Inactive = 2
    }
    public enum SourceContact : int
    {
        Crm = 1,
        MoneycorpOnline = 2
    }
}