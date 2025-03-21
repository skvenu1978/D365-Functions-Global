// <copyright file="ObjConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Utilities;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// ObjConversion.
    /// </summary>
    public class ObjConversion
    {
        /// <summary>
        /// GetClientInfoRequest.
        /// </summary>
        /// <param name="contextEntity">contextEntity</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <param name="isContactBeingInactivated">isContactBeingInactivated</param>
        /// <param name="isContactTrigger">isContactTrigger</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>ClientContactInfoRequest</returns>
        public static ClientContactInfoRequest GetClientInfoRequest(Entity contextEntity, IOrganizationServiceAsync2 svcClient, ILogger logger, bool isContactBeingInactivated, bool isContactTrigger, out string errMsg)
        {
            ClientContactInfoRequest _request = new ClientContactInfoRequest();
            ClientInfoRequest accountRequest = new ClientInfoRequest();
            errMsg = string.Empty;

            try
            {
                Guid? accountId = Validators.GetAccountIdFromContext(contextEntity, svcClient);
                logger.LogInformation("Account Id:" + accountId);

                if (accountId.HasValue)
                {
                    accountRequest.AccountId = accountId.Value;

                    Entity accountEntity = svcClient.Retrieve(AccountConstants.Account, accountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                    int? owningBusiness = AttributeHelper.GetOptionSetValue(accountEntity, AccountConstants.Mcorp_owningbusiness);
                    int? clientType = AttributeHelper.GetOptionSetValue(accountEntity, AccountConstants.Mcorp_clienttype);
                    int? crmAccountNum = AttributeHelper.GetStringAttributeAsIntegerValue(accountEntity, AccountConstants.Accountnumber);
                    Guid? primaryContactId = AttributeHelper.GetGuidValue(accountEntity, AccountConstants.Primarycontactid);

                    if (!crmAccountNum.HasValue)
                    {
                        errMsg = "CRM Account Number is missing";
                    }

                    if (owningBusiness.HasValue && clientType.HasValue && crmAccountNum.HasValue)
                    {
                        logger.LogInformation("Owning Business:" + owningBusiness);
                        logger.LogInformation("clientType:" + clientType);
                        logger.LogInformation("crmAccountNum:" + crmAccountNum);

                        if (clientType.Value == (int)ClientType.Private || clientType.Value == (int)ClientType.Corporate)
                        {
                            accountRequest = AccountConversion.AccountToObject(accountEntity, svcClient, owningBusiness.Value, clientType.Value, crmAccountNum.Value);

                            McCustomerType custType;
                            List<ContactInfoDetails> lstContactnfo = [];
                            EntityCollection contacts = new EntityCollection();
                            int accountState = AttributeHelper.GetStateStatusOptionSetValue(accountEntity, AccountConstants.Statecode);

                            // contact inactivation trigger
                            if (isContactBeingInactivated)
                            {
                                // get only this contact
                                Entity deactivatedContact = svcClient.Retrieve(contextEntity.LogicalName, contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                                contacts.Entities.Add(deactivatedContact);
                            }
                            else
                            {
                                if (isContactTrigger)
                                {
                                    // send only the triggered contact
                                    Entity triggerContact = svcClient.Retrieve("contact", contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                                    contacts.Entities.Add(triggerContact);
                                }
                                else
                                {
                                    if (primaryContactId.HasValue)
                                    {
                                        Entity primaryContact = svcClient.Retrieve("contact", primaryContactId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                                        int contactState = AttributeHelper.GetStateStatusOptionSetValue(primaryContact, AccountConstants.Statecode);

                                        if (contactState == 0)
                                        {
                                            contacts.Entities.Add(primaryContact);
                                        }
                                        else
                                        {
                                            EntityCollection activeContacts = EntityCommon.GetAccountChildContacts(accountEntity.Id, svcClient, accountState, isContactBeingInactivated);
                                            contacts.Entities.Add(activeContacts.Entities[0]);
                                        }
                                    }
                                    else
                                    {
                                        errMsg = "Primary Contact is not set";
                                        return _request;
                                    }
                                }
                            }

                            if (contacts.Entities != null && contacts.Entities.Count > 0)
                            {
                                foreach (Entity contact in contacts.Entities)
                                {
                                    string firstName = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Firstname);

                                    if (!string.IsNullOrEmpty(firstName))
                                    {
                                        ContactInfoDetails contactInfo = new ContactInfoDetails();
                                        custType = EnumHelper.RetrieveCustomerType(owningBusiness.Value, clientType.Value);
                                        contactInfo = ContactDetailToObject(contact, accountRequest.CrmId, custType, svcClient);
                                        lstContactnfo.Add(contactInfo);
                                    }
                                    else
                                    {
                                        errMsg += "First Name is missing on related contacts";
                                    }
                                }
                            }
                            else
                            {
                                errMsg = "No contacts found with matching account statecode";
                                return _request;
                            }

                            _request.Client = accountRequest;

                            if (lstContactnfo != null && lstContactnfo.Count > 0)
                            {
                                ContactInfoRequest contactInfoRequest = new ContactInfoRequest();
                                contactInfoRequest.ContactsInfoDetails = lstContactnfo;
                                contactInfoRequest.SourceContact = "Crm";
                                _request.Contacts = contactInfoRequest;
                            }
                        }
                        else
                        {
                            errMsg = "Client Type is not valid for this request";
                        }
                    }
                    else
                    {
                        if (!owningBusiness.HasValue)
                        {
                            errMsg = "Owning Business is not set.";
                        }

                        if (!clientType.HasValue)
                        {
                            errMsg = "Client Type is not set.";
                        }

                        if (!crmAccountNum.HasValue)
                        {
                            errMsg = "CRM Account Number is not set.";
                        }
                    }
                }
                else
                {
                    errMsg = "Unable to get Account from context or contact is inactive";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to GetRequest for account:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                logger.LogError(ex, errMsg);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to GetRequest for account:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromException(ex);
                logger.LogError(ex, errMsg);
            }

            return _request;
        }

        /// <summary>
        /// ContactDetailToObject.
        /// </summary>
        /// <param name="contact">contact</param>
        /// <param name="accountnumber">accountnumber</param>
        /// <param name="customerType">customerType</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>ContactInfoDetails</returns>
        private static ContactInfoDetails ContactDetailToObject(Entity contact, int accountnumber, McCustomerType customerType, IOrganizationServiceAsync2 svcClient)
        {
            ContactInfoDetails contactInfo = new();

            Guid? aoWebUserId = AttributeHelper.GetStringAttributeAsGuid(contact, ContactConstants.Mcorp_accountopeninguserid);

            if (aoWebUserId.HasValue)
            {
                contactInfo.AccountOpeningWebUserId = aoWebUserId;
            }

            contactInfo.CRMId = accountnumber;
            contactInfo.CRMContactGuid = contact.Id;
            contactInfo.FirstName = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Firstname);
            contactInfo.ModifiedOn = DateTime.Now;
            contactInfo.AuthorisedForTrade = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_authorisedtotrade);
            int? salutationValue = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_title);

            if (salutationValue.HasValue)
            {
                contactInfo.Salutation = (Salutation)salutationValue.Value;
            }

            int? authorisationStatus = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_authorisationstatus);

            if (authorisationStatus.HasValue)
            {
                contactInfo.AuthorisationStatus = (AuthorisationStatus)authorisationStatus.Value;
            }
            else
            {
                contactInfo.AuthorisationStatus = AuthorisationStatus.None;
            }

            int statusreason = AttributeHelper.GetStateStatusOptionSetValue(contact, ContactConstants.Statuscode);

            if (statusreason == (int)McContactStatus.Inactive_Inactive)
            {
                contactInfo.IsActive = false;
            }
            else
            {
                contactInfo.IsActive = true;
            }

            int? accOpenStatusValue = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_accountopeningstatusreason);

            if (accOpenStatusValue.HasValue)
            {
                contactInfo.AccountOpeningStatusReason = (AccountOpeningStatusReason)accOpenStatusValue.Value;
            }

            //flags allow
            contactInfo.SendPopEmail = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_sendpop);
            contactInfo.SendEmail = AttributeHelper.GetInverseBooleanValueDefaultFalse(contact, ContactConstants.Donotemail);
            contactInfo.SendMail = AttributeHelper.GetInverseBooleanValueDefaultFalse(contact, ContactConstants.Donotpostalmail);
            contactInfo.SendSMS = AttributeHelper.GetInverseBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_donotallowsms);

            //country code
            string mobCountryCode = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_mobilephonecountrycodeid, CountryCodeConstants.Mcorp_countrycode, svcClient, CountryCodeConstants.Mcorp_countrycodealpha2);

            if (!string.IsNullOrEmpty(mobCountryCode))
            {
                contactInfo.CountryName = mobCountryCode;
                contactInfo.CountryCodeMobilePhone = mobCountryCode;
            }

            bool? reciptientNotification = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_newrecipientnotification);
            int? notificationType = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_recipientnotificationtype);
            contactInfo.NewRecipientNotification = EnumHelper.ConvertNotificationTypeToNotificationMethodEnum(notificationType);

            int? fundsNotification = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_incomingfundsnotificationtype);
            bool incomingNotfication = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_incomingfundsnotification);

            contactInfo.IncomingFundsNotification = EnumHelper.ConvertNotificationTypeToNotificationMethodEnum(fundsNotification);
            contactInfo.DateOfBirth = AttributeHelper.GetDateTimeAttributeValue(contact, ContactConstants.Birthdate);

            string contactPostCode = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Address1_postalcode);
            contactInfo.Postcode = contactPostCode;

            if (string.IsNullOrEmpty(contactPostCode))
            {
                contactInfo.Postcode = EntityCommon.GetContactPostCode(contact.Id, svcClient);
            }

            int? owningBusiness = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_owningbusiness);

            if (owningBusiness.HasValue)
            {
                contactInfo.ApplicationSource = EnumHelper.ConvertOwningBusinessToApplicationSourceEnum(owningBusiness.Value);
            }

            //optional
            string lastName = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Lastname);

            if (!string.IsNullOrEmpty(lastName))
            {
                contactInfo.LastName = lastName;
            }

            string jobTitle = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Jobtitle);

            if (!string.IsNullOrEmpty(jobTitle))
            {
                contactInfo.JobTitle = jobTitle;
            }

            string homePhone = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Telephone2);

            if (!string.IsNullOrEmpty(homePhone))
            {
                contactInfo.HomePhone = homePhone;
            }

            string mobPhone = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Mobilephone);

            if (!string.IsNullOrEmpty(mobPhone))
            {
                contactInfo.MobilePhone = mobPhone;
            }

            string busPhone = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Telephone1);

            if (!string.IsNullOrEmpty(busPhone))
            {
                contactInfo.BusinessPhone = busPhone;
            }

            string ccMobPhone = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_mobilephonecountrycodeid, CountryCodeConstants.Mcorp_countrycode, svcClient, CountryCodeConstants.Mcorp_countrycodealpha2);

            if (!string.IsNullOrEmpty(ccMobPhone))
            {
                contactInfo.CountryCodeMobilePhone = ccMobPhone;
            }

            string ccBusPhone = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_workphonecountrycodeid, CountryCodeConstants.Mcorp_countrycode, svcClient, CountryCodeConstants.Mcorp_countrycodealpha2);

            if (!string.IsNullOrEmpty(ccBusPhone))
            {
                contactInfo.CountryCodeBusinessPhone = ccBusPhone;
            }

            string ccHomePhone = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_homephonecountrycodeid, CountryCodeConstants.Mcorp_countrycode, svcClient, CountryCodeConstants.Mcorp_countrycodealpha2);

            if (!string.IsNullOrEmpty(ccHomePhone))
            {
                contactInfo.CountryCodeHomePhone = ccHomePhone;
            }

            //changes for OMNI email mapping
            if (customerType == McCustomerType.CFX_CORPORATE || customerType == McCustomerType.TMO_CORPORATE || customerType == McCustomerType.MM_CORPORATE)
            {
                string email1 = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Emailaddress1);
                if (!string.IsNullOrEmpty(email1)) contactInfo.BusinessEmail = email1;

                string email2 = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Emailaddress2);
                if (!string.IsNullOrEmpty(email2)) contactInfo.HomeEmail = email2;
            }

            if (customerType == McCustomerType.CFX_PRIVATE || customerType == McCustomerType.TMO_PRIVATE || customerType == McCustomerType.MM_PRIVATE)
            {
                string email1 = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Emailaddress1);
                if (!string.IsNullOrEmpty(email1)) contactInfo.HomeEmail = email1;

                string email2 = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Emailaddress2);
                if (!string.IsNullOrEmpty(email2)) contactInfo.BusinessEmail = email2;
            }

            //New fields added
            contactInfo.IsAuthorisePayment = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_authorisepayment);
            contactInfo.IsMlpaEnabled = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_multilevelauthorisationrequried);
            contactInfo.IsSuperUser = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_superuser);
            contactInfo.CanCreateUnlimited = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_ccpunlimited);
            contactInfo.CanApproveUnlimited = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_capunlimited);
            contactInfo.CreateLimit = AttributeHelper.GetMoneyAttributeDefaultZero(contact, ContactConstants.Mcorp_ccpamount);
            contactInfo.NumberOfApproversRequired = AttributeHelper.GetWholeNumberAttributeValueDefaultZero(contact, ContactConstants.Mcorp_numberofapprovers);
            contactInfo.ApproveLimit = AttributeHelper.GetMoneyAttributeDefaultZero(contact, ContactConstants.Mcorp_capamount);
            contactInfo.WebAccessStatus = ContactWebAccessStatus.NotActivated;

            int? mcolStatus = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_mcolactivationstatus);

            if (mcolStatus.HasValue)
            {
                contactInfo.WebAccessStatus = EnumHelper.ConvertContactWebAccessStatusEnum(mcolStatus.Value);
            }

            contactInfo.MayArrangeTransfer = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_authorisedtoarrangetransferto3rd);
            contactInfo.MayEnquireAboutAccount = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_authorisedtoenquireabout);
            contactInfo.IsSendContractNote = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_sendcontractnote);
            contactInfo.IsSendInvoice = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_sendinvoice);
            bool _canBulkUpload = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_canbulkupload);
            bool _canCreatePayment = AttributeHelper.GetBooleanValueDefaultTrue(contact, ContactConstants.Mcorp_createpayment);
            bool _canCreateBeneficiary = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_createbeneficiary);
            bool _canCreateDeal = AttributeHelper.GetBooleanValueDefaultTrue(contact, ContactConstants.Mcorp_createdeal);
            bool _canEditPlan = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_caneditplan);
            bool _canCharting = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_charting);
            bool _canLinkPrepaidCard = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_canlinkprepaidcard);
            bool _canViewLiveRates = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_canviewliverates);
            bool _canCreatePaymentExchange = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_createpaymentexchange);
            bool _canEnableTransferInstruction = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_enabletransferinstruction);
            bool _canCreateDraftPayment = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_cancreatedraftpayment);
            bool _canCreateCollectionMethodDdAch = AttributeHelper.GetBooleanValueDefaultTrue(contact, ContactConstants.Mcorp_cancreatecollectionmethodddach);
            bool _canCreateDealForOwnPayment = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_cancreatedealforownpayment);
            bool _canManageLineOfCredit = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_canmanagelineofcredit);
            bool _canApplyLineOfCredit = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_canapplylineofcredit);

            List<string> _granted = [];
            List<string> _denied = [];

            if (_canCreateBeneficiary) _granted.Add("CanCreateBeneficiary"); else _denied.Add("CanCreateBeneficiary");
            if (_canCreatePayment) _granted.Add("CanCreatePayment"); else _denied.Add("CanCreatePayment");
            if (_canCreateDeal) _granted.Add("CanCreateDeal"); else _denied.Add("CanCreateDeal");
            if (_canEditPlan) _granted.Add("Edit Plan"); else _denied.Add("Edit Plan");
            if (_canBulkUpload) _granted.Add("CanBulkUpload"); else _denied.Add("CanBulkUpload");
            if (_canCharting) _granted.Add("CanViewCharts"); else _denied.Add("CanViewCharts");
            if (_canViewLiveRates) _granted.Add("CanViewLiveRates"); else _denied.Add("CanViewLiveRates");
            if (_canLinkPrepaidCard) _granted.Add("CanLinkPrepaidCard"); else _denied.Add("CanLinkPrepaidCard");
            if (_canCreatePaymentExchange) _granted.Add("CreatePaymentExchange"); else _denied.Add("CreatePaymentExchange");
            if (_canEnableTransferInstruction) _granted.Add("EnableTransferInstruction"); else _denied.Add("EnableTransferInstruction");
            if (_canCreateDraftPayment) _granted.Add("CanCreateDraftPayment"); else _denied.Add("CanCreateDraftPayment");
            if (_canCreateCollectionMethodDdAch) _granted.Add("CanCreateCollectionMethod(DD/ACH)"); else _denied.Add("CanCreateCollectionMethod(DD/ACH)");
            if (_canCreateDealForOwnPayment) _granted.Add("CanCreateDealForOwnPayment"); else _denied.Add("CanCreateDealForOwnPayment");
            if (_canManageLineOfCredit) _granted.Add("CanManageLineOfCredit"); else _denied.Add("CanManageLineOfCredit");
            if (_canApplyLineOfCredit) _granted.Add("CanApplyLineOfCredit"); else _denied.Add("CanApplyLineOfCredit");

            string ctryResidence = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_countryofresidenceid, ContactConstants.Mcorp_country, svcClient, ContactConstants.Mcorp_name);
            if (!string.IsNullOrEmpty(ctryResidence)) contactInfo.CountryOfResidence = ctryResidence;

            contactInfo.WebUserPrivilegesDenied = _denied;
            contactInfo.WebUserPrivilegesGranted = _granted;

            bool? hasLtyCard = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_hasloyaltycard);
            if (hasLtyCard.HasValue) contactInfo.HasLoyaltyCard = hasLtyCard.Value;

            int? loyalCardType = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_loyaltycardtype);
            if (loyalCardType.HasValue) contactInfo.LoyaltyCardType = EnumHelper.ConvertLoyaltyCardTypeEnum(loyalCardType.Value);

            contactInfo.ReceivePaymentApprovalEmails = AttributeHelper.GetBooleanValueDefaultTrue(contact, ContactConstants.Mcorp_receivepaymentapprovalemails);

            contactInfo.SubscribeToPromotions = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_exclusiveofferspromotions);
            contactInfo.SubscribeToNewsAlerts = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_marketnewsratealerts);
            contactInfo.SubscribeToFeaturesTips = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_productfeaturestips);

            contactInfo.ContactViaEmail = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_marketing_email);
            contactInfo.ContactViaMail = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_marketing_post);
            contactInfo.ContactViaSms = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_marketing_sms);
            contactInfo.ContactViaPhone = AttributeHelper.GetBooleanValueDefaultFalse(contact, ContactConstants.Mcorp_marketing_telephone);

            contactInfo.ContactViaDisplayNotifications = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_marketing_displaynotifications);
            contactInfo.ThirdpartyEmail = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_thirdparty_email);
            contactInfo.ThirdpartyPost = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_thirdparty_post);
            contactInfo.ThirdpartySms = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_thirdparty_sms);
            contactInfo.ThirdpartyTelephone = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_thirdparty_telephone);
            contactInfo.ThirdParty = AttributeHelper.GetBooleanValue(contact, ContactConstants.Mcorp_thirdparty);
            contactInfo.IdentityCheck1_Type = IdentityType.None;
            contactInfo.IdentityCheck2_Type = IdentityType.None;

            int? idCheck1 = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_identitycheck1);

            if (idCheck1.HasValue)
            {
                contactInfo.IdentityCheck1_Type = EnumHelper.ConvertIdentityTypeEnum(idCheck1.Value);
            }

            int? idCheck2 = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_identitycheck2);

            if (idCheck2.HasValue)
            {
                contactInfo.IdentityCheck2_Type = EnumHelper.ConvertIdentityTypeEnum(idCheck2.Value);
            }

            int? idStatus1 = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_identitystatus1);
            contactInfo.IdentityCheck1_Status = EnumHelper.ConvertIdentityStatusEnum(idStatus1);

            int? idStatus2 = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_identitystatus2);
            contactInfo.IdentityCheck2_Status = EnumHelper.ConvertIdentityStatusEnum(idStatus2);

            contactInfo.IdentityCheck1_Number = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Mcorp_identitynumber1);
            contactInfo.IdentityCheck2_Number = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Mcorp_identitynumber2);

            contactInfo.IdentityCheck1_Origin = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_idorigin1id, ContactConstants.Mcorp_country, svcClient, ContactConstants.Mcorp_name);
            contactInfo.IdentityCheck2_Origin = EntityCommon.GetEntityAttributeValue(contact, ContactConstants.Mcorp_idorigin2id, ContactConstants.Mcorp_country, svcClient, ContactConstants.Mcorp_name);

            contactInfo.IdentityCheck1_ExpiryDate = AttributeHelper.GetFormattedDateTimeAttributeValue(contact, ContactConstants.Mcorp_identityexpirydate1);
            contactInfo.IdentityCheck2_ExpiryDate = AttributeHelper.GetFormattedDateTimeAttributeValue(contact, ContactConstants.Mcorp_identityexpirydate2);

            return contactInfo;
        }
    }
}