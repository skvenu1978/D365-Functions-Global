// <copyright file="FoxProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Fox
{
    using Abstractions.Fox;
    using Fox;
    using Models.Fox;
    using Utilities;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;
    using McGlobalAzureFunctions.Fox;

    /// <summary>
    /// Implementation for IFoxProvider
    /// </summary>
    public class FoxProvider : IFoxProvider
    {
        /// <summary>
        /// Gets sample data for
        /// testing
        /// </summary>
        /// <param name="nameCriteria">search name</param>
        /// <returns>AccountSearchResultMc object</returns>
        public AccountSearchResultMc GetSampleMFRMAccountData(string nameCriteria)
        {
            AccountSearchResultMc data = new AccountSearchResultMc() { Name = nameCriteria };
            data.McId = Guid.Parse("058ed2cc-9860-eb11-8145-002248409736");
            data.AccountDealerFullName = "Alfie Mew";
            data.AccountDealerId = Guid.Parse("0CD66E4A-D0E8-EC11-84A2-000D3A7FCF99");
            data.AccountExecutiveFullName = "Andrew Wary";
            data.AccountExecutiveId = Guid.Parse("B5C3F0C4-CBFE-DE11-BBFA-00215A495D80");
            data.ClientType = 3;
            data.FedsId = 500781;
            data.MifidCategory = 100000003;
            data.MifidCategoryString = "Retail-Basic";
            data.Name = nameCriteria;
            data.OwningBusiness = 100000018;
            data.ParentAccountId = null;

            return data;
        }

        /// <summary>
        /// Searches the accounts table using the
        /// name keyword and optionally
        /// owning business value
        /// </summary>
        /// <param name="nameCriteria"></param>
        /// <param name="owningBusiness"></param>
        /// <param name="svcClient"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<AccountSearchResultMc> SearchMFRMAccount(string nameCriteria, int? owningBusiness, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            errMsg = string.Empty;

            List<AccountSearchResultMc> results = [];

            if (string.IsNullOrEmpty(nameCriteria))
            {
                errMsg = "Search Account Name is missing";
                return results;
            }

            if (owningBusiness.HasValue)
            {
                bool isValid = Validators.IsValidOwningBusiness(owningBusiness.Value);

                if (!isValid)
                {
                    errMsg = "Valid Owning Business is not passed";
                    return results;
                }
            }

            // read from D365
            string fetchXml = FetchStrings.GetMFRMAccountFetchCommon(nameCriteria, owningBusiness);

            EntityCollection eColl = new();

            try
            {
                eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

                if (eColl.Entities.Count > 0)
                {
                    results = FoxConverter.EntityToObject(eColl, svcClient);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to get Account data:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to get Account data:" + EntityCommon.GetErrorMessageFromException(ex);
            }

            return results;
        }

        /// <summary>
        /// Search selected 
        /// contacts for specific
        /// account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="contactIds"></param>
        /// <param name="svcClient"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public AccountContactDetailedSearchResultMc GetAccountRelatedContacts(string accountId, List<string> contactIds, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            errMsg = string.Empty;
            AccountContactDetailedSearchResultMc result = new AccountContactDetailedSearchResultMc();
            bool isValidGuid = Validators.IsValidGuid(accountId);

            if (!isValidGuid)
            {
                errMsg = "Account Id parameter is missing or not valid";
                return result;
            }

            List<ContactDetailedSearchResultMc> results = [];
            string accFetchXml = FetchStrings.GetDetailedAccountFetch(accountId);
            EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, accFetchXml);

            if (eColl.Entities.Count <= 0)
            {
                return result;
            }

            AccountDetailedSearchResultMc accData = new();

            try
            {
                accData = FoxConverter.AccountToObject(eColl.Entities[0], svcClient);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to get Account data:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to get Account data:" + EntityCommon.GetErrorMessageFromException(ex);
            }

            List<ContactDetailedSearchResultMc> linkedContacts = [];

            if (string.IsNullOrEmpty(errMsg))
            {
                if (contactIds.Count > 0)
                {
                    linkedContacts = GetDetailedContacts(contactIds, svcClient, out errMsg);
                }
            }

            result.Account = accData;
            result.Contacts = linkedContacts;

            return result;
        }

        /// <summary>
        /// Get all Authorised
        /// Contacts linked to the Account
        /// specified in the query
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="svcClient"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<ContactSearchResultMc> GetLinkedAuthorisedContacts(string accountId, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            List<ContactSearchResultMc> results = [];
            errMsg = string.Empty;

            bool isValidGuid = Validators.IsValidGuid(accountId);

            if (!isValidGuid)
            {
                errMsg = "Parameter account id empty or not valid guid";
                return results;
            }

            // read from D365
            string fetchXml = FetchStrings.GetRelatedContactsFetch(accountId);

            try
            {
                EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

                if (eColl.Entities.Count > 0)
                {
                    results = ContactConverter.ContactToObject(eColl, svcClient);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to get Related Contacts:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to get Related Contacts:" + EntityCommon.GetErrorMessageFromException(ex);
            }

            return results;
        }

        /// <summary>
        /// Get Account Contacts
        /// loaded into an object
        /// collection
        /// </summary>
        /// <param name="contactIds"></param>
        /// <param name="svcClient"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private static List<ContactDetailedSearchResultMc> GetDetailedContacts(List<string> contactIds, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            List<ContactDetailedSearchResultMc> result = [];

            errMsg = string.Empty;

            try
            {
                foreach (string contactId in contactIds)
                {
                    string cFetchXml = FetchStrings.GetDetailedContact(contactId);

                    EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, cFetchXml);

                    if (eColl.Entities.Count <= 0)
                    {
                        continue;
                    }

                    ContactDetailedSearchResultMc accData = ContactConverter.ContactDetailToObject(eColl.Entities[0]);
                    result.Add(accData);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to get Related Contacts:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to get Related Contacts:" + EntityCommon.GetErrorMessageFromException(ex);
            }

            return result;
        }
    }
}