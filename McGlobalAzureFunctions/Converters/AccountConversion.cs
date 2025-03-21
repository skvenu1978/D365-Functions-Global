// <copyright file="AccountConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// AccountConversion.
    /// </summary>
    public class AccountConversion
    {
        /// <summary>
        /// AccountToObject.
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="owningBusiness">owningBusiness</param>
        /// <param name="clientType">clientType</param>
        /// <param name="accountNum">accountNum</param>
        /// <returns>ClientInfoRequest</returns>
        public static ClientInfoRequest AccountToObject(Entity account, IOrganizationServiceAsync2 svcClient, int owningBusiness, int clientType, int accountNum)
        {
            ClientInfoRequest client = new()
            {
                AccountOpeningWebUserId = AttributeHelper.GetStringAttributeAsGuid(account, AccountConstants.Mcorp_accountopeninguserid),
                CrmId = accountNum,
                AccountId = account.Id,
                CRMAccountGuid = account.Id,
                IsActive = true,
                IsAuthorised = false,
                IsAwaitingClientCategoryLetter = false,
                LegacySystemName = LegacySystemName.None,
                DefaultCurrency = CurrencyConstants.GBP
            };

            int accountStatus = AttributeHelper.GetStateStatusOptionSetValue(account, AccountConstants.Statuscode);

            if (accountStatus == ((int)D365AccountStatus.Inactive) || accountStatus == ((int)D365AccountStatus.Closed))
            {
                client.IsActive = false;
            }

            if (accountStatus == (int)D365AccountStatus.Authorised)
            {
                client.IsAuthorised = true;
            }

            if (Equals((int)D365AccountStatus.AwaitingClientCategoryLetter, accountStatus))
            {
                client.IsAwaitingClientCategoryLetter = true;
            }

            client.ModifiedOn = DateTime.Now;
            client.PrimaryCRMContactGuid = AttributeHelper.GetGuidValue(account, AccountConstants.Primarycontactid);
            client.ClientType = (ClientType)clientType;
            int? prefCtMethod = AttributeHelper.GetOptionSetValue(account, AccountConstants.Preferredcontactmethodcode);
            string typeName = string.Empty;

            if (prefCtMethod.HasValue)
            {
                typeName = EnumHelper.GetEnumStringName(typeof(ContactMethod), prefCtMethod.Value);
            }
            else
            {
                typeName = EnumHelper.GetEnumStringName(typeof(ContactMethod), (int)ContactMethod.Any);
            }

            client.ContactMethodPreference = (ContactMethod)Enum.Parse(typeof(ContactMethod), typeName);
            client.FedsId = EntityCommon.GetFedsId(account);
            string acName = AttributeHelper.GetStringAttributeValue(account, AccountConstants.Name);

            if (!string.IsNullOrEmpty(acName))
            {
                client.ClientName = acName;
            }

            ClientInfoRequest addressData = EntityCommon.GetAccountAddress(account, svcClient);

            if (!string.IsNullOrEmpty(addressData.AddressLine1))
            {
                client.AddressLine1 = addressData.AddressLine1;
            }

            if (!string.IsNullOrEmpty(addressData.AddressLine2))
            {
                client.AddressLine2 = addressData.AddressLine2;
            }

            if (!string.IsNullOrEmpty(addressData.AddressLine3))
            {
                client.AddressLine3 = addressData.AddressLine3;
            }

            if (!string.IsNullOrEmpty(addressData.City))
            {
                client.City = addressData.City;
            }

            if (!string.IsNullOrEmpty(addressData.County))
            {
                client.County = addressData.County;
            }

            if (!string.IsNullOrEmpty(addressData.State))
            {
                client.State = addressData.State;
            }

            if (!string.IsNullOrEmpty(addressData.PostCode))
            {
                client.PostCode = addressData.PostCode;
            }

            if (!string.IsNullOrEmpty(addressData.Country))
            {
                client.Country = addressData.Country;
            }

            string residingCountry = EntityCommon.GetEntityAttributeValue(account, AccountConstants.Mcorp_countryofresidenceid, CountryConstants.Mcorp_country, svcClient, CountryConstants.Mcorp_name);

            if (!string.IsNullOrEmpty(residingCountry))
            {
                client.CountryOfResidence = residingCountry;
            }

            string duns = AttributeHelper.GetStringAttributeValue(account, AccountConstants.Mcorp_duns);
            if (!string.IsNullOrEmpty(duns)) client.Duns = duns;

            bool? isGPSAccountSetup = AttributeHelper.GetBooleanValue(account, AccountConstants.Mcorp_gpsaccountsetup);
            if (isGPSAccountSetup.HasValue)
            {
                client.IsGPSAccountSetup = isGPSAccountSetup.Value;
            }

            bool? isGPSClientSignedTCs = AttributeHelper.GetBooleanValue(account, AccountConstants.Mcorp_gpsclientsignedtcs);

            if (isGPSClientSignedTCs.HasValue)
            {
                client.IsGPSClientSignedTCs = isGPSClientSignedTCs.Value;
            }

            bool? isGPSProspect = AttributeHelper.GetBooleanValue(account, AccountConstants.Mcorp_gpsprospect);

            if (isGPSProspect.HasValue)
            {
                client.IsGPSProspect = isGPSProspect.Value;
            }

            client.SendEmail = AttributeHelper.GetInverseBooleanValueDefaultFalse(account, AccountConstants.Accountdonotemail);
            client.SendMail = AttributeHelper.GetInverseBooleanValueDefaultFalse(account, AccountConstants.Accountdonotpostalmail);
            client.SendSMS = false;
            client.ClientStatus = ClientStatus.STP;
            client.TTFeePaymentInMethod = TTFeePaymentInMethod.SelfFund;
            client.TTFeeStructure = TTFeeStructure.Invoice;
            client.IsSpot = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_spot);
            client.IsForward = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_forward);
            client.DebitCard = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_debitcard);
            client.IsEnableDebitCard = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_enableddebitcard);
            client.CreditCard = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_creditcard);
            client.IsEnableCreditCard = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_enabledcreditcard);
            client.DirectDebit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_directdebit);
            client.IsEnableDirectDebit = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_enableddirectdebit);
            client.TradeLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_tradelimit);
            client.Spread = AttributeHelper.GetDecimalAttributeValueDefaultZero(account, AccountConstants.Mcorp_spread);
            client.ApplicationSource = EnumHelper.ConvertOwningBusinessToApplicationSourceEnum(owningBusiness);
            int? acSystem = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_system);

            if (acSystem.HasValue)
            {
                if (acSystem.Value == (int)MCOnlineSystem.Basic)
                {
                    client.System = MCOnlineSystem.Basic;
                }
                else
                {
                    client.System = MCOnlineSystem.Advanced;
                }
            }
            else
            {
                if (clientType == 3)
                {
                    client.System = MCOnlineSystem.Advanced;
                }
                else
                {
                    client.System = MCOnlineSystem.Basic;
                }
            }

            client.AggregateLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_aggregatelimit);
            client.TradeLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_tradelimit);

            if (clientType == 3)
            {
                if (client.TradeLimit == 0)
                {
                    client.TradeLimit = (decimal)100000;
                }

                if (client.AggregateLimit == 0)
                {
                    client.AggregateLimit = (decimal)250000;
                }
            }

            Guid? aeGuid = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_accountexecutiveid);

            if (aeGuid.HasValue)
            {
                client.AccountExecutiveGuidLookup = EntityCommon.GetCRM2011UserIdFromD365(aeGuid.Value, svcClient);
            }

            Guid? dealerIdGuidLookup = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_dealerid);

            if (dealerIdGuidLookup.HasValue)
            {
                client.DealerIdGuidLookup = EntityCommon.GetCRM2011UserIdFromD365(dealerIdGuidLookup.Value, svcClient);
            }

            Guid? mcCampId = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_campaignid);
            if (mcCampId.HasValue) client.MarketingCampaignGuidLookup = mcCampId;

            Guid? refPartId = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_primarypartnerreferralid);
            if (refPartId.HasValue) client.PrimaryReferringPartnerGuidLookup = refPartId;

            Guid? currentOwnerId = AttributeHelper.GetGuidValue(account, AccountConstants.Ownerid);

            if (currentOwnerId.HasValue)
            {
                client.OwnerId = EntityCommon.GetCRM2011UserIdFromD365(currentOwnerId.Value, svcClient);
            }

            decimal? ddAgLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_directdebitaggregatelimit);

            if (ddAgLimit.HasValue)
                client.DirectDebitAggregateLimit = ddAgLimit.Value;

            int? ddType = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_directdebittype);

            if (ddType.HasValue)
                client.DirectDebitType = (DirectDebitType)ddType.Value;
            else
                client.DirectDebitType = DirectDebitType.None;

            client.PaymentInPerTransactionLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_expectedpaymentinamountpertransaction);
            client.PaymentOutPerTransactionLimit = AttributeHelper.GetMoneyAttributeDefaultZero(account, AccountConstants.Mcorp_expectedamountpertransaction);

            CurrencyInfo currencyInfo = EntityCommon.GetFromToCurrency(account, svcClient);
            if (!string.IsNullOrEmpty(currencyInfo.FromCurrency)) client.FromCurrency = currencyInfo.FromCurrency;

            if (!string.IsNullOrEmpty(currencyInfo.ToCurrency)) client.ToCurrency = currencyInfo.ToCurrency;

            client.ParentCrmId = null;

            int? model = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_clientmodel);

            if (model.HasValue)
            {
                if (model.Value == (int)McClientModule.Standard)
                {
                    client.ClientAccountModelType = ClientAccountModelType.None;
                }
                else if (model.Value == (int)McClientModule.Managing)
                {
                    client.ClientAccountModelType = ClientAccountModelType.MetaAccount;
                }
                else if (model.Value == (int)McClientModule.Managed)
                {
                    Guid? parentAccountId = AttributeHelper.GetGuidValue(account, AccountConstants.Parentaccountid);

                    if (parentAccountId.HasValue)
                    {
                        Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, parentAccountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Accountnumber));

                        if (!string.IsNullOrEmpty(parentAccount.LogicalName))
                        {
                            client.ParentCrmId = AttributeHelper.GetStringAttributeAsIntegerValue(parentAccount, AccountConstants.Accountnumber);
                            client.ClientAccountModelType = ClientAccountModelType.SubAccount;
                        }
                    }
                }
            }
            else
            {
                client.ClientAccountModelType = ClientAccountModelType.None;
            }

            client.SendNotifications = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_kpsendnotifications);
            client.NotificationContactEmail = AttributeHelper.GetStringAttributeValue(account, AccountConstants.Mcorp_kpnotificationcontactemail);
            client.MoneyServiceBusiness = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_moneyservicebusiness);
            client.FinancialInstitution = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_financialinstitution);
            client.HighValueDealer = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_highvaluedealer);

            int? clLang = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_clientlanguage);

            if (clLang.HasValue)
            {
                client.ClientLanguage = (ClientLanguage)clLang;
            }
            else
            {
                client.ClientLanguage = ClientLanguage.English;
            }

            bool? hasLtCard = AttributeHelper.GetBooleanValue(account, AccountConstants.Mcorp_hasloyaltycard);
            if (hasLtCard.HasValue) client.HasLoyaltyCard = hasLtCard.Value;

            int? ltCardType = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_loyaltycardtype);

            if (ltCardType.HasValue)
            {
                client.LoyaltyCardType = (LoyaltyCardType)ltCardType;
            }
            else
            {
                client.LoyaltyCardType = LoyaltyCardType.None;
            }

            string legIdent = AttributeHelper.GetStringAttributeValue(account, AccountConstants.Mcorp_legalentityidentifier);
            if (!string.IsNullOrEmpty(legIdent)) client.LegalEntityIdentifier = legIdent;

            Guid? mfrmAcId = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_mfrmaccountid);
            if (mfrmAcId.HasValue) client.MFRMAccountGuid = mfrmAcId;

            Guid? tttAccountGuid = AttributeHelper.GetGuidValue(account, AccountConstants.Mcorp_tttaccountlink);
            if (tttAccountGuid.HasValue) client.TTTAccountGuid = tttAccountGuid;

            int? mifidCategory = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_mifidcategory);

            if (mifidCategory.HasValue)
            {
                client.MifidCategory = (MifidCategory)(int)EnumHelper.ConvertMifidCategory(mifidCategory.Value);
            }

            int? legacySystemName = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_legacysystemname);

            if (legacySystemName.HasValue)
            {
                client.LegacySystemName = (LegacySystemName)legacySystemName.Value;
            }

            string legSysId = AttributeHelper.GetStringAttributeValue(account, AccountConstants.Mcorp_legacysystemid);
            if (!string.IsNullOrEmpty(legSysId)) client.LegacySystemId = legSysId;

            string defCurr = EntityCommon.GetEntityAttributeValue(account, CurrencyConstants.Transactioncurrencyid, CurrencyConstants.Transactioncurrency, svcClient, CurrencyConstants.Isocurrencycode);

            if (!string.IsNullOrEmpty(defCurr))
            {
                client.DefaultCurrency = defCurr;
            }

            if (owningBusiness == (int)McOwningBusiness.MoneycorpUSInc)
            {
                client.DirectDebitCurrency = CurrencyConstants.USD;
            }
            else
            {
                client.DirectDebitCurrency = CurrencyConstants.GBP;
            }

            client.DefaultCountry = GetDefaultCountry(owningBusiness);

            int? payClassification = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_paymentclassification);
            if (payClassification.HasValue) client.PaymentClassification = EnumHelper.ConvertPaymentClassificationEnum(payClassification);

            int? customerCode = AttributeHelper.GetOptionSetValue(account, AccountConstants.Customertypecode);
            client.RelationshipType = EnumHelper.ConvertRelationshipTypeCodeEnum(customerCode);

            int? tradeStatus = AttributeHelper.GetOptionSetValue(account, AccountConstants.Mcorp_tradingstatus);
            client.TradingStatus = EnumHelper.ConvertTradingStatusEnum(tradeStatus);

            client.IsForwardQuestionnaire = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_forwardquestionnaire);
            client.BulkPaymentsEnabled = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_enabledbulkpayments);
            client.ReverseWireEnabled = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_reversewireenabled);
            client.IsCloudElementEnabled = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_cloudelementsenabled);
            client.IsDealerIncomingFundEnabled = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_dealerincomingfund);
            client.IsPriorityCustomer = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_prioritycustomer);
            client.EditPaymentDate = AttributeHelper.GetBooleanValueDefaultFalse(account, AccountConstants.Mcorp_editpaymentdate);

            client.AccountId = account.Id;

            //if (!isProd)
            //{
            //    //TODO: Write plugin to set MCOL defaults for Account and Contact
            //    if (clientType == 3)
            //    {
            //        client.TradingStatus = TradingStatus.NotYetTraded;
            //        client.RelationshipType = CustomerTypeCode.Customer;
            //        client.Spread = 0;
            //        client.System = MCOnlineSystem.Advanced;
            //        client.TradeLimit = (decimal)100000;
            //        client.AggregateLimit = (decimal)250000;
            //        client.DirectDebit = (decimal)100000;
            //        client.DirectDebitAggregateLimit = (decimal)100000;
            //        client.DirectDebitType = DirectDebitType.ThreeDay;
            //    }
            //}

            return client;
        }

        /// <summary>
        /// Get Country based on 
        /// Owning Business Value
        /// </summary>
        /// <param name="owningBusiness"></param>
        /// <returns></returns>
        private static string GetDefaultCountry(int owningBusiness)
        {
            string? countryShortCode = "GBR";

            switch (owningBusiness)
            {
                case (int)McOwningBusiness.MoneycorpUSInc:
                    countryShortCode = "USA";
                    break;
                case (int)McOwningBusiness.MoneycorpEuropeIreland:
                case (int)McOwningBusiness.MIFIDMoneycorpEuropeIreland:
                case (int)McOwningBusiness.MoneycorpEurope:
                    countryShortCode = "IRL";
                    break;
                case (int)McOwningBusiness.MoneycorpEuropeRomania:
                    countryShortCode = "ROU";
                    break;
                case (int)McOwningBusiness.MoneycorpEuropeSpain:
                case (int)McOwningBusiness.MIFIDMoneycorpEuropeSpain:
                    countryShortCode = "ESP";
                    break;
                case (int)McOwningBusiness.MoneycorpEuropeFrance:
                    countryShortCode = "FRA";
                    break;
                case (int)McOwningBusiness.Moneycorp:
                    countryShortCode = "GBR";
                    break;
            }

            return countryShortCode;
        }
    }
}