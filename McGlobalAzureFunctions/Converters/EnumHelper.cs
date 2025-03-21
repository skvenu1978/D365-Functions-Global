// <copyright file="EnumHelper.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;

    /// <summary>
    /// Enum store for the project
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Gets the SharePoint Site Id
        /// for uploading of documens
        /// </summary>
        /// <returns></returns>
        public static string GetSpSiteIdUsingSiteName()
        {
            string spSiteId = string.Empty;

            string? spSiteName = Environment.GetEnvironmentVariable(FnConstants.SharePointSiteName, EnvironmentVariableTarget.Process);

            if (!string.IsNullOrEmpty(spSiteName))
            {
                spSiteId = spSiteName switch
                {
                    "MicrosoftDynamics365-Development" => SharePointConstants.SpSiteId_Dev,
                    "MicrosoftDynamics365-CorpDev" => SharePointConstants.SpSiteId_CorpDev,
                    "MicrosoftDynamics365-QA" => SharePointConstants.SpSiteId_QA,
                    "MicrosoftDynamics365-QA-Auto" => SharePointConstants.SpSiteId_QA_Auto,
                    "MicrosoftDynamics365-UAT" => SharePointConstants.SpSiteId_UAT,
                    "MicrosoftDynamics365-PreProd" => SharePointConstants.SpSiteId_PreProd,
                    _ => SharePointConstants.SpSiteId_CorpDev,
                };
            }

            return spSiteId;
        }

        /// <summary>
        /// Gets ENUM string name
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumStringName(Type enumType, int enumValue)
        {
            string retValue = string.Empty;
            string? strTypeName = Enum.GetName(enumType, enumValue);

            if (!string.IsNullOrEmpty(strTypeName))
            {
                retValue = strTypeName;
            }

            return retValue;
        }

        /// <summary>
        /// Get the Mc Customer type
        /// based on client type and owning business
        /// </summary>
        /// <param name="owningBusiness"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static McCustomerType RetrieveCustomerType(int owningBusiness, int clientType)
        {
            return owningBusiness switch
            {
                100000000 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000002 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000001 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000011 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    4 => McCustomerType.WHOLESALE_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000014 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    4 => McCustomerType.WHOLESALE_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000013 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000015 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000003 => McCustomerType.CFX_PRIVATE,
                100000020 => clientType switch
                {
                    3 => McCustomerType.WHOLESALE_CORPORATE,
                    1 => McCustomerType.WHOLESALE_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000005 => clientType switch
                {
                    3 => McCustomerType.TMO_CORPORATE,
                    1 => McCustomerType.TMO_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000006 => McCustomerType.CFX_PRIVATE,
                100000010 => clientType switch
                {
                    3 => McCustomerType.MM_CORPORATE,
                    1 => McCustomerType.MM_PRIVATE,
                    5 => McCustomerType.MM_Advisory,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000004 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000017 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000018 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000019 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    4 => McCustomerType.WHOLESALE_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000021 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000022 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000023 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000024 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000025 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000026 => clientType switch
                {
                    3 => McCustomerType.MM_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000027 => clientType switch
                {
                    3 => McCustomerType.MM_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000028 => clientType switch
                {
                    3 => McCustomerType.MM_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000029 => clientType switch
                {
                    3 => McCustomerType.MM_CORPORATE,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                100000030 => clientType switch
                {
                    3 => McCustomerType.CFX_CORPORATE,
                    1 => McCustomerType.CFX_PRIVATE,
                    2 => McCustomerType.CFX_RP,
                    _ => throw new NotSupportedException(clientType.ToString()),
                },
                _ => throw new NotSupportedException(Enum.GetName(typeof(McOwningBusiness), (int)owningBusiness)),
            };
        }

        /// <summary>
        /// Converts Owning Busing
        /// to App Source string
        /// </summary>
        /// <param name="owningBusiness">Owning Business</param>
        /// <returns>AppSource Object</returns>
        public static ApplicationSource ConvertOwningBusinessToApplicationSourceEnum(int owningBusiness)
        {
            ApplicationSource appSource = ApplicationSource.Moneycorp;

            switch (owningBusiness)
            {
                case 100000000:
                    appSource = ApplicationSource.Moneycorp;
                    break;
                case 100000001:
                    appSource = ApplicationSource.JanusPostOffice;
                    break;
                case 100000002:
                    appSource = ApplicationSource.ThomasCook;
                    break;
                case 100000006:
                    appSource = ApplicationSource.Moneycorp;
                    break;
                case 100000003:
                    appSource = ApplicationSource.Tempus;
                    break;
                case 100000005:
                    appSource = ApplicationSource.Moneycorp;
                    break;
                case 100000011:
                    appSource = ApplicationSource.MoneycorpBank;
                    break;
                case 100000014:
                    appSource = ApplicationSource.MoneycorpRomania;
                    break;
                case 100000015:
                    appSource = ApplicationSource.SpainMFRM;
                    break;
                case 100000004:
                    appSource = ApplicationSource.MoneycorpPTY;
                    break;
                case 100000017:
                    appSource = ApplicationSource.Sainsbury;
                    break;
                case 100000010:
                    appSource = ApplicationSource.MoneycorpMFRM;
                    break;
                case 100000018:
                    appSource = ApplicationSource.MoneycorpUSInc;
                    break;
                case 100000022:
                    appSource = ApplicationSource.MoneycorpEurope;
                    break;
                case 100000021:
                    appSource = ApplicationSource.MoneycorpEuropeIreland;
                    break;
                case 100000024:
                    appSource = ApplicationSource.MoneycorpEuropeRomania;
                    break;
                case 100000023:
                    appSource = ApplicationSource.MoneycorpEuropeSpain;
                    break;
                case 100000025:
                    appSource = ApplicationSource.MoneycorpBrazil;
                    break;
                case 100000026:
                    appSource = ApplicationSource.MoneycorpBankMIFID;
                    break;
                case 100000027:
                    appSource = ApplicationSource.MIFIDMoneycorpEuropeIreland;
                    break;
                case 100000028:
                    appSource = ApplicationSource.MIFIDMoneycorpEuropeSpain;
                    break;
                case 100000029:
                    appSource = ApplicationSource.MIFIDMoneycorpEurope;
                    break;
                case 100000030:
                    appSource = ApplicationSource.MoneycorpEuropeFrance;
                    break;
            }

            return appSource;
        }

        /// <summary>
        /// Convert in MIFID Category
        /// to Mifid Category Object
        /// </summary>
        /// <param name="crmMifidCategory"></param>
        /// <returns></returns>
        public static MifidCategory ConvertMifidCategory(int crmMifidCategory)
        {
            return crmMifidCategory switch
            {
                100000005 => MifidCategory.ElectedEligibleCounterparty,
                100000004 => MifidCategory.ElectedProfessional,
                100000002 => MifidCategory.EligibleCounterparty,
                100000000 => MifidCategory.Professional,
                100000001 => MifidCategory.Retail,
                100000003 => MifidCategory.RetailBasic,
                100000006 => MifidCategory.RetailForwardOnly,
                100000007 => MifidCategory.ProfessionalIncTARFs,
                100000008 => MifidCategory.ElectedProfessionalInTARFs,
                0 => MifidCategory.None,
                _ => MifidCategory.None,
            };
        }

        /// <summary>
        /// Converts Payment Classification Enum
        /// </summary>
        /// <param name="accountPaymentClassification"></param>
        /// <returns></returns>
        public static PaymentClassification ConvertPaymentClassificationEnum(int? accountPaymentClassification)
        {
            PaymentClassification retValue = PaymentClassification.None;

            if (accountPaymentClassification.HasValue)
            {
                retValue = accountPaymentClassification switch
                {
                    0 => PaymentClassification.None,
                    1 => PaymentClassification.MSBPSPInstitution,
                    2 => PaymentClassification.WhiteListCorporates,
                    _ => PaymentClassification.None,
                };
            }
            else
            {
                retValue = PaymentClassification.None;
            }

            return retValue;
        }

        /// <summary>
        /// Convert Int Relationship type code 
        /// to ENUM object
        /// </summary>
        /// <param name="relationshipType">Int rel type</param>
        /// <returns>Cust Type Code Object</returns>
        public static CustomerTypeCode ConvertRelationshipTypeCodeEnum(int? relationshipType)
        {
            CustomerTypeCode enumValue = CustomerTypeCode.None;

            if (relationshipType.HasValue)
            {
                enumValue = relationshipType switch
                {
                    1 => CustomerTypeCode.Competitor,
                    2 => CustomerTypeCode.Consultant,
                    3 => CustomerTypeCode.Customer,
                    6 => CustomerTypeCode.Influencer,
                    4 => CustomerTypeCode.Investor,
                    13 => CustomerTypeCode.OnlineSeller,
                    12 => CustomerTypeCode.Other,
                    7 => CustomerTypeCode.Press,
                    8 => CustomerTypeCode.Prospect,
                    5 => CustomerTypeCode.ReferringPartner,
                    9 => CustomerTypeCode.Reseller,
                    10 => CustomerTypeCode.Supplier,
                    11 => CustomerTypeCode.Vendor,
                    _ => CustomerTypeCode.None,
                };
            }

            return enumValue;
        }

        /// <summary>
        /// Converts Trading Status Enum
        /// from Int
        /// </summary>
        /// <param name="tradeStatus"></param>
        /// <returns></returns>
        public static TradingStatus ConvertTradingStatusEnum(int? tradeStatus)
        {
            return tradeStatus switch
            {
                3 => TradingStatus.AccountClosed,
                4 => TradingStatus.AccountSuspended,
                10 => TradingStatus.ActivelyTrading,
                20 => TradingStatus.Dormant,
                21 => TradingStatus.InsufficientKYC,
                12 => TradingStatus.NeedsRepitch,
                6 => TradingStatus.NeverNeededOurServices,
                9 => TradingStatus.NoResponse,
                5 => TradingStatus.NotBuyingAnymore,
                1 => TradingStatus.NotYetTraded,
                19 => TradingStatus.Online,
                11 => TradingStatus.ReferredtoGPS,
                8 => TradingStatus.SmallDeals,
                7 => TradingStatus.TradedElsewhere,
                2 => TradingStatus.TradedinPart,
                _ => TradingStatus.None,
            };
        }

        /// <summary>
        /// Converts int enum to NotificationMethod
        /// object
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>NotificationMethod</returns>
        public static NotificationMethod ConvertNotificationTypeToNotificationMethodEnum(int? type)
        {
            if (type.HasValue)
            {
                if (type == 1)
                {
                    return NotificationMethod.Email;
                }
                else if (type == 3)
                {
                    return NotificationMethod.EmailSms;
                }
                else if (type == 2)
                {
                    return NotificationMethod.Sms;
                }
                else
                {
                    return NotificationMethod.None;
                }
            }
            else
            {
                return NotificationMethod.None;
            }
        }

        /// <summary>
        /// Converts in enum to ContactWebAccessStatus
        /// </summary>
        /// <param name="webStatus">webStatus</param>
        /// <returns>ContactWebAccessStatus</returns>
        public static ContactWebAccessStatus ConvertContactWebAccessStatusEnum(int webStatus)
        {
            if (webStatus == 1) return ContactWebAccessStatus.NotActivated;
            else if (webStatus == 2) return ContactWebAccessStatus.Activating;
            else if (webStatus == 3) return ContactWebAccessStatus.Activated;
            else return ContactWebAccessStatus.NotActivated;
        }

        /// <summary>
        ///  Converts int enum to
        ///  LoyaltyCardType
        /// </summary>
        /// <param name="crmLoyaltyCardType"></param>
        /// <returns>LoyaltyCardType</returns>
        public static LoyaltyCardType ConvertLoyaltyCardTypeEnum(int crmLoyaltyCardType)
        {
            return crmLoyaltyCardType switch
            {
                1 => LoyaltyCardType.Nectar,
                0 => LoyaltyCardType.None,
                _ => LoyaltyCardType.None,
            };
        }

        /// <summary>
        /// Converts int enum to
        /// IdentityType
        /// </summary>
        /// <param name="idType">idType</param>
        /// <returns>IdentityType</returns>
        public static IdentityType ConvertIdentityTypeEnum(int idType)
        {
            return idType switch
            {
                0 => IdentityType.Others,
                3 => IdentityType.BankStatement,
                2 => IdentityType.DrivingLicense,
                5 => IdentityType.IrTaxNotification,
                4 => IdentityType.NationalIdentityCard,
                1 => IdentityType.Passport,
                9 => IdentityType.ResidentAlienCard,
                6 => IdentityType.ShotGunLicense,
                7 => IdentityType.SocialSecurityNumber,
                8 => IdentityType.UsTemporaryDriverLicense,
                10 => IdentityType.IDCard,
                11 => IdentityType.OldversionofROIDCard,
                12 => IdentityType.TemporaryID,
                13 => IdentityType.UsStateIdentificationCard
            };
        }
       

        public static IdentityStatus ConvertIdentityStatusEnum(int? idStatus)
        {
            if (idStatus.HasValue)
            {
                return idStatus switch
                {
                    2 => IdentityStatus.NotRequired,
                    3 => IdentityStatus.Recieved,
                    1 => IdentityStatus.Required,
                    _ => IdentityStatus.None,
                };
            }
            else
            {
                return IdentityStatus.None;
            }
        }
        public static int ConvertDDClearance(int ddClearance)
        {
            return ddClearance switch
            {
                2 => 5,
                1 => 3,
                _ => 0,
            };
        }

        public static CreditLineStatus ConvertCreditLineStatus(int creditLineStatus)
        {
            return creditLineStatus switch
            {
                100000001 => CreditLineStatus.Submitted,
                100000002 => CreditLineStatus.SentToCreditCommittee,
                100000003 => CreditLineStatus.Approved,
                100000004 => CreditLineStatus.Rejected,
                100000005 => CreditLineStatus.Cancelled,
                100000006 => CreditLineStatus.RejectedByCreditTeam,
                100000007 => CreditLineStatus.Withdrawn,
                2 => CreditLineStatus.Inactive,
                _ => CreditLineStatus.Created,
            };
        }

        public static int? ConvertOmniCreditLineStatus(CreditLineStatus creditLineStatus)
        {
            int? retValue = null;

            retValue = creditLineStatus switch
            {
                CreditLineStatus.Submitted => 100000001,
                CreditLineStatus.SentToCreditCommittee => 100000002,
                CreditLineStatus.Approved => 100000003,
                CreditLineStatus.Rejected => 100000004,
                CreditLineStatus.Cancelled => 100000005,
                CreditLineStatus.RejectedByCreditTeam => 100000006,
                CreditLineStatus.Withdrawn => 100000007,
                _ => (int?)1,
            };
            return retValue;
        }

        public static D365AccountStatus ConvertAccountStatus(int accountstatus)
        {
            return accountstatus switch
            {
                100000000 => D365AccountStatus.Authorised,
                100000001 => D365AccountStatus.DeclinedAuthorisation,
                100000002 => D365AccountStatus.Unauthorised,
                100000003 => D365AccountStatus.AwaitingOnlineActivation,
                100000004 => D365AccountStatus.Closed,
                100000005 => D365AccountStatus.AwaitingClientCategoryLetter,
                100000006 => D365AccountStatus.PrivateEventual,
                2 => D365AccountStatus.Inactive,
                _ => D365AccountStatus.AwaitingTradingAgreement,
            };
        }
    }
}