// <copyright file="ClientResponseProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Implementation for ClientResponseProvider
    /// interface
    /// </summary>
    public class ClientResponseProvider : IClientResponseProvider
    {
        /// <summary>
        /// reads the response from OMNI
        /// and updates D365
        /// </summary>
        /// <param name="omniResponse">omniResponse</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessClientContactInfoResponse(ClientContactInfoResponse omniResponse, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            DynTaskResponse accResponse = this.ProcessClientInfoResponse(omniResponse.ClientResponse, requestId, svcClient);

            if (accResponse.IsSuccess)
            {
                DynTaskResponse contactResponse = DynUpdateContacts(omniResponse.ContactResponse, requestId, svcClient);
                response = contactResponse;
            }
            else
            {
                response = accResponse;
            }

            return response;
        }

        /// <summary>
        /// Updates the contact
        /// with Marketing preferences
        /// </summary>
        /// <param name="omniRequest">omniResponse</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessMarketingPrefsUpdate(ReadMarketingPreferenceRequest omniRequest, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new Entity();

            try
            {
                entity = svcClient.Retrieve(ContactConstants.Contact, omniRequest.ContactId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ProcessMarketingPrefsUpdate-Unable to find contact with ID:" + omniRequest.ContactId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    Entity contact = new Entity(entity.LogicalName, entity.Id);
                    contact[ContactConstants.Mcorp_productfeaturestips] = omniRequest.IsProductFeaturesTips;
                    contact[ContactConstants.Mcorp_marketnewsratealerts] = omniRequest.IsMarketNewsRateAlerts;
                    contact[ContactConstants.Mcorp_exclusiveofferspromotions] = omniRequest.IsExclusiveOffersPromotions;
                    contact[ContactConstants.Mcorp_postoffice] = omniRequest.IsPostOfficePreference;
                    contact[ContactConstants.Mcorp_travelmoney] = omniRequest.IsTravelMoneyPreference;
                    contact[ContactConstants.Donotbulkemail] = omniRequest.IsMarketingEmail;
                    contact[ContactConstants.Donotbulkpostalmail] = omniRequest.IsMarketingPost;
                    contact[ContactConstants.Mcorp_marketing_sms] = omniRequest.IsMarketingSms;
                    contact[ContactConstants.Mcorp_marketing_displaynotifications] = omniRequest.IsMarketingDisplayNotifications;
                    contact[ContactConstants.Mcorp_marketing_telephone] = omniRequest.IsMarketingTelephone;
                    contact[ContactConstants.Mcorp_marketing_sourceofconsent] = new OptionSetValue((int)SourceOfConsent.Online);
                    contact[ContactConstants.Mcorp_marketing_lastupdated] = DateTime.Now;
                    contact[ContactConstants.Mcorp_other_researchsurveys] = true;
                    contact[ContactConstants.Mcorp_other_sourceofconsent] = new OptionSetValue((int)SourceOfConsent.Online);
                    contact[ContactConstants.Mcorp_other_lastupdated] = DateTime.Now;
                    contact[ContactConstants.Mcorp_thirdparty] = (omniRequest.IsThirdPartyEmail || omniRequest.IsThirdPartyPost || omniRequest.IsThirdPartySms || omniRequest.IsThirdPartyTelephone || false);
                    contact[ContactConstants.Mcorp_thirdparty_email] = omniRequest.IsThirdPartyEmail;
                    contact[ContactConstants.Mcorp_thirdparty_post] = omniRequest.IsThirdPartyPost;
                    contact[ContactConstants.Mcorp_thirdparty_sms] = omniRequest.IsThirdPartySms;
                    contact[ContactConstants.Mcorp_thirdparty_telephone] = omniRequest.IsThirdPartyTelephone;
                    contact[ContactConstants.Mcorp_thirdparty_sourceofconsent] = new OptionSetValue((int)SourceOfConsent.Online);
                    contact[ContactConstants.Mcorp_thirdparty_lastupdated] = DateTime.Now;
                    response.IsSuccess = true;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    response.ErrorMessage = "ProcessMarketingPrefsUpdate-Failed to update opportunity:" + EntityCommon.GetErrorMessageFromException(ex);
                    response.Exception = ex;
                }
            }

            return response;
        }

        public DynTaskResponse ProcessClientInfoResponse(ClientInfoResponse omniData, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            try
            {
                ColumnSet cols = new ColumnSet(AccountConstants.Mcorp_fedsid, AccountConstants.Mcorp_omnipaymentreference);
                Entity d365Account = svcClient.Retrieve(AccountConstants.Account, omniData.AccountId, cols);
                Entity d365AccountToUpdate = new(AccountConstants.Account, omniData.AccountId);

                var logMessage = "Update ";
                bool canUpdateAccount = false;

                string fedsId = AttributeHelper.GetStringAttributeValue(d365Account, AccountConstants.Mcorp_fedsid);

                if (string.IsNullOrEmpty(fedsId))
                {
                    d365AccountToUpdate[AccountConstants.Mcorp_fedsid] = omniData.ClientId.ToString();
                    logMessage += string.Format(": Fedsid={0}", omniData.ClientId.ToString());
                    canUpdateAccount = true;
                }

                string omniPaymentReference = AttributeHelper.GetStringAttributeValue(d365Account, AccountConstants.Mcorp_omnipaymentreference);

                if (string.IsNullOrEmpty(omniPaymentReference))
                {
                    d365AccountToUpdate[AccountConstants.Mcorp_omnipaymentreference] = omniData.ReferenceKey;
                    logMessage += string.Format(": OMNIPAYmentReference = {0}", omniData.ReferenceKey);
                    canUpdateAccount = true;
                }

                if (canUpdateAccount)
                {
                    svcClient.Update(d365AccountToUpdate);
                    //logger.LogInformation("DynUpdateAccount update successful");
                }
                else
                {
                    //logger.LogInformation("No update required on ACCOUNT");
                }

                response.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ClientContactInfoResponse-Failed to update account:" + omniData.AccountId.ToString() + "-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            return response;
        }

        private DynTaskResponse DynUpdateContacts(ContactInfoResponse contactsData, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            //logger.LogInformation("DynUpdateContacts start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            if (contactsData.Results != null)
            {
                if (contactsData.Results.Count > 0)
                {
                    foreach (var contact in contactsData.Results)
                    {
                        try
                        {
                            if (contact.CRMGuid.HasValue)
                            {
                                var logMessage = "Update ";
                                Entity d365Contact = svcClient.Retrieve(ContactConstants.Contact, contact.CRMGuid.Value,
                                    new Microsoft.Xrm.Sdk.Query.ColumnSet(ContactConstants.Mcorp_omnicontactid));

                                string omniContactId = AttributeHelper.GetStringAttributeValue(d365Contact, ContactConstants.Mcorp_omnicontactid);

                                if (string.IsNullOrEmpty(omniContactId))
                                {
                                    Entity d365ContactToUpdate = new Entity(ContactConstants.Contact, contact.CRMGuid.Value);
                                    d365ContactToUpdate[ContactConstants.Mcorp_omnicontactid] = contact.Id.ToString();
                                    logMessage += string.Format(": mcorp_omnicontactid={0}", contact.Id.ToString());
                                    //logger.LogInformation(logMessage);
                                    svcClient.Update(d365ContactToUpdate);
                                }
                                else
                                {

                                    //logger.LogInformation("No update required on contact:" + contact.CRMGuid.Value.ToString());
                                }

                                response.IsSuccess = true;
                            }
                        }
                        catch (FaultException<OrganizationServiceFault> ex)
                        {
                            response.ErrorMessage = "DynUpdateContact failed in D365:" + EntityCommon.GetErrorMessageFromException(ex);
                            response.Exception = ex;
                            //logger.LogError(ex, response.ErrorMessage, requestId);
                        }
                    }
                }
            }
            else
            {
                response.ErrorMessage = "Response does not contain contacts";
            }

            //logger.LogInformation("DynUpdateContacts end");
            return response;
        }
    }
}