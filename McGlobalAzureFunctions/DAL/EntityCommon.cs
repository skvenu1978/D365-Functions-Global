// <copyright file="EntityCommon.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL
{
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Const;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.ServiceModel;

    /// <summary>
    /// EntityCommon class.
    /// </summary>
    public class EntityCommon
    {
        /// <summary>
        /// Get EntityReference by Id
        /// for activities.
        /// </summary>
        /// <param name="svcClient"></param>
        /// <param name="senderEmail">senderEmail</param>
        /// <param name="exists"></param>
        /// <returns>EntityReference</returns>
        public static EntityReference GetUserOrQueueUsingEmailAddress(IOrganizationService svcClient, string senderEmail, out bool exists)
        {
            Entity entity = new Entity();
            exists = false;

            if (!exists)
            {
                // check in queue
                try
                {
                    entity = GetEntityByCondition("queue", svcClient, "emailaddress", senderEmail, false);

                    if (!string.IsNullOrEmpty(entity.LogicalName))
                    {
                        exists = true;
                    }
                }
                catch (FaultException<OrganizationServiceFault>)
                {
                }
            }

            if (!exists)
            {
                // check user by email address
                if (!string.IsNullOrEmpty(senderEmail))
                {
                    try
                    {
                        entity = GetEntityByCondition("systemuser", svcClient, "internalemailaddress", senderEmail, false);

                        if (!string.IsNullOrEmpty(entity.LogicalName))
                        {
                            exists = true;
                        }
                    }
                    catch (FaultException<OrganizationServiceFault>)
                    {
                    }
                }
            }

            return entity.ToEntityReference();
        }

        /// <summary>
        /// GetEntity ByCondition
        /// </summary>
        /// <param name="entityName">entityName</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="attributeName">attributeName</param>
        /// <param name="attributeValue">attributeValue</param>
        /// <param name="checkOnlyActive">checkOnlyActive</param>
        /// <returns></returns>
        public static Entity GetEntityByCondition(string entityName, IOrganizationService svcClient, string attributeName, object attributeValue, bool checkOnlyActive)
        {
            Entity retValue = new Entity();

            EntityCollection e = new EntityCollection();

            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = attributeName;
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(attributeValue);

            ConditionExpression condition2 = new ConditionExpression();

            if (checkOnlyActive)
            {
                condition2.AttributeName = "statecode";
                condition2.Operator = ConditionOperator.Equal;
                condition2.Values.Add(0);
            }

            FilterExpression filter1 = new FilterExpression();
            filter1.Conditions.Add(condition1);

            if (checkOnlyActive)
            {
                filter1.Conditions.Add(condition2);
            }

            filter1.FilterOperator = LogicalOperator.And;

            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria.AddFilter(filter1);

            EntityCollection result = svcClient.RetrieveMultiple(query);

            if (result.Entities != null && result.Entities.Count > 0)
            {
                retValue = result.Entities[0];
            }

            return retValue;
        }

        /// <summary>
        /// GetAlertsRelatedToAccount.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>EntityCollection</returns>
        public static EntityCollection GetAlertsRelatedToAccount(Guid accountId, IOrganizationServiceAsync2 svcClient)
        {
            EntityCollection collAlerts = new EntityCollection();

            try
            {
                QueryExpression query = new QueryExpression(AlertConstants.Mcorp_alert);
                query.ColumnSet = new ColumnSet(true);
                FilterExpression qryFilter = new FilterExpression();
                qryFilter.AddCondition(AlertConstants.Mcorp_accountid, ConditionOperator.Equal, accountId);
                query.Criteria.AddFilter(qryFilter);
                collAlerts = svcClient.RetrieveMultiple(query);
            }
            catch (Exception)
            { }

            return collAlerts;
        }

        /// <summary>
        /// GetAccountChildContacts.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="stateCode">stateCode</param>
        /// <param name="isContactInactive">isContactInactive</param>
        /// <returns>EntityCollection</returns>
        public static EntityCollection GetAccountChildContacts(Guid accountId, IOrganizationServiceAsync2 svcClient, int stateCode, bool isContactInactive)
        {
            EntityCollection collContacts = new EntityCollection();

            try
            {
                QueryExpression query = new QueryExpression(ContactConstants.Contact);
                query.ColumnSet = new ColumnSet(true);
                FilterExpression qryFilter = new FilterExpression();
                qryFilter.AddCondition(ContactConstants.Parentcustomerid, ConditionOperator.Equal, accountId);

                if (!isContactInactive)
                {
                    qryFilter.AddCondition(ContactConstants.Statecode, ConditionOperator.Equal, stateCode);
                }

                query.Criteria.AddFilter(qryFilter);
                collContacts = svcClient.RetrieveMultiple(query);
            }
            catch (Exception)
            { }

            return collContacts;
        }

        /// <summary>
        /// RetrieveCurrencyIdByCode.
        /// </summary>
        /// <param name="currencyCode">currencyCode</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>CurrencyInfo</returns>
        public static CurrencyInfo RetrieveCurrencyIdByCode(string currencyCode, IOrganizationServiceAsync2 svcClient)
        {
            CurrencyInfo currencyInfo = new CurrencyInfo();

            try
            {
                Entity cur = CrudCommon.GetEntityByCondition(CurrencyConstants.Transactioncurrency, svcClient, CurrencyConstants.Isocurrencycode, currencyCode, false);

                if (!string.IsNullOrEmpty(cur.LogicalName))
                {
                    currencyInfo.CurrencyId = cur.Id;
                    currencyInfo.CurrencyName = AttributeHelper.GetStringAttributeValue(cur, CurrencyConstants.Currencyname);
                    currencyInfo.CurrencyShortCode = AttributeHelper.GetStringAttributeValue(cur, CurrencyConstants.Isocurrencycode);
                }
            }
            catch (Exception)
            {
            }

            return currencyInfo;
        }

        /// <summary>
        /// RetrieveCountryCodeByAlpha2Code.
        /// </summary>
        /// <param name="searchValue">searchValue</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>Guid</returns>
        public static Guid? RetrieveCountryCodeByAlpha2Code(string searchValue, IOrganizationServiceAsync2 svcClient)
        {
            return CrudCommon.GetEntityIdByCondition(svcClient, CountryCodeConstants.Mcorp_countrycodealpha2, searchValue, CountryCodeConstants.Mcorp_countrycode, false);
        }

        /// <summary>
        /// GetFromToCurrency.
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>CurrencyInfo</returns>
        public static CurrencyInfo GetFromToCurrency(Entity account, IOrganizationServiceAsync2 svcClient)
        {
            CurrencyInfo cr = new ();
            string tradingSellXml = IntegrationFetchString.GetTradingCurrencyFetch(account.Id.ToString());
            EntityCollection linkedCurrencies = FetchXmlHelper.GetDataUsingFetch(svcClient, tradingSellXml);

            if (linkedCurrencies != null && linkedCurrencies.Entities.Count > 0)
            {
                foreach (Entity linkedCurrency in linkedCurrencies.Entities)
                {
                    int? transactionType = AttributeHelper.GetOptionSetValueFromCollection(linkedCurrency, "mcorp_typeoftransaction");

                    if (transactionType.HasValue)
                    {
                        string? isoCurrencyCode = AttributeHelper.GetAliaseAttributeStringValue(linkedCurrency, "a_transactioncurrency.isocurrencycode");

                        if (!string.IsNullOrEmpty(isoCurrencyCode))
                        {
                            if (transactionType.Value == 100000001) // sell
                            {
                                cr.FromCurrency = isoCurrencyCode;
                                break;
                            }
                        }
                    }
                }

                foreach (Entity linkedCurrency in linkedCurrencies.Entities)
                {
                    int? transactionType = AttributeHelper.GetOptionSetValueFromCollection(linkedCurrency, "mcorp_typeoftransaction");

                    if (transactionType.HasValue)
                    {
                        string? isoCurrencyCode = AttributeHelper.GetAliaseAttributeStringValue(linkedCurrency, "a_transactioncurrency.isocurrencycode");

                        if (!string.IsNullOrEmpty(isoCurrencyCode))
                        {
                            if (transactionType.Value == 100000000) // buy
                            {
                                cr.ToCurrency = isoCurrencyCode;
                                break;
                            }
                        }
                    }
                }
            }

            return cr;
        }

        /// <summary>
        /// GetEntityAttributeValue.
        /// </summary>
        /// <param name="contextEntity">contextEntity</param>
        /// <param name="lookupAttributeName">lookupAttributeName</param>
        /// <param name="entityName">entityName</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string GetEntityAttributeValue(Entity contextEntity, string lookupAttributeName, string entityName, IOrganizationServiceAsync2 svcClient, string attributeName)
        {
            string retValue = string.Empty;
            Guid? lookUpId = AttributeHelper.GetGuidValue(contextEntity, lookupAttributeName);

            if (lookUpId.HasValue)
            {
                Entity entity = svcClient.Retrieve(entityName, lookUpId.Value, new ColumnSet(attributeName));

                if (!string.IsNullOrEmpty(entity.LogicalName))
                {
                    retValue = AttributeHelper.GetStringAttributeValue(entity, attributeName);
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetAccountAddress.
        /// </summary>
        /// <param name="account">account</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>ClientInfoRequest</returns>
        public static ClientInfoRequest GetAccountAddress(Entity account, IOrganizationServiceAsync2 svcClient)
        {
            string fetchXml = IntegrationFetchString.GetMoneycorpAddressFetch(account.Id.ToString());
            ClientInfoRequest client = new ClientInfoRequest();
            EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

            if (eColl != null && eColl.Entities.Count > 0)
            {
                foreach (Entity addressEntity in eColl.Entities)
                {
                    int? addressType = AttributeHelper.GetOptionSetValue(addressEntity, AddressConstants.Mcorp_addresstype);

                    if (addressType.HasValue && addressType.Value == 1)
                    {
                        client.AddressLine1 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street1);
                        client.AddressLine2 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street2);
                        client.AddressLine3 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street3);
                        client.City = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_city);
                        client.County = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_county);
                        client.State = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_stateprovince);
                        client.PostCode = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_zippostalcode);
                        client.Country = AttributeHelper.GetAliaseAttributeStringValue(addressEntity, AddressConstants.A_Country_mcorp_name);
                        break;
                    }
                }
            }

            return client;
        }

        /// <summary>
        /// GetContactPostCode.
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>string</returns>
        public static string GetContactPostCode(Guid contactId, IOrganizationServiceAsync2 svcClient)
        {
            string retValue = string.Empty;
            string fetchXml = IntegrationFetchString.GetMoneycorpAddressContactFetch(contactId.ToString());
            EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

            if (eColl != null && eColl.Entities.Count > 0)
            {
                foreach (Entity entity in eColl.Entities)
                {
                    int? addressType = AttributeHelper.GetOptionSetValue(entity, AddressConstants.Mcorp_addresstype);

                    if (addressType.HasValue && addressType.Value == 1)
                    {
                        retValue = AttributeHelper.GetStringAttributeValue(entity, AddressConstants.Mcorp_zippostalcode);
                        break;
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetFedsId.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>int</returns>
        public static int GetFedsId(Entity entity)
        {
            int result = 0;

            string fedsId = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Mcorp_fedsid);

            if (!string.IsNullOrEmpty(fedsId))
            {
                result = int.Parse(fedsId);
            }

            return result;
        }

        /// <summary>
        /// GetErrorMessageFromException.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>string</returns>
        public static string GetErrorMessageFromFaultException(FaultException<OrganizationServiceFault> ex)
        {
            string errMsg = ex.Message + "-";

            if (ex.InnerException != null)
            {
                errMsg += ex.InnerException.Message + "-";
            }

            if (ex.StackTrace != null)
            {
                errMsg += ex.StackTrace;
            }

            return errMsg;
        }

        /// <summary>
        /// GetErrorMessageFromException.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>string</returns>
        public static string GetErrorMessageFromException(Exception ex)
        {
            string errMsg = ex.Message + "-";

            if (ex.InnerException != null)
            {
                errMsg += ex.InnerException.Message + "-";
            }

            if (ex.StackTrace != null)
            {
                errMsg += ex.StackTrace;
            }

            return errMsg;
        }

        /// <summary>
        /// GetCRM2011UserIdFromD365.
        /// </summary>
        /// <param name="currentUserLkp">currentUserLkp</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>Guid</returns>
        public static Guid GetCRM2011UserIdFromD365(Guid currentUserLkp, IOrganizationServiceAsync2 svcClient)
        {
            Guid retUser = FnConstants.StaticUserId;

            try
            {
                Entity userAe = svcClient.Retrieve(UserConstants.Systemuser, currentUserLkp, new ColumnSet(UserConstants.Mcorp_crm2011userguid));

                if (!string.IsNullOrEmpty(userAe.LogicalName))
                {
                    retUser = AttributeHelper.GetStringAttributeAsGuidDefault(userAe, UserConstants.Mcorp_crm2011userguid, FnConstants.StaticUserId);
                }
            }
            catch (Exception)
            { }

            return retUser;
        }

        /// <summary>
        /// GetD365UserUsingCRM2011Guid.
        /// </summary>
        /// <param name="crm2011Guid">crm2011Guid</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>Guid</returns>
        public static Guid? GetD365UserUsingCRM2011Guid(Guid crm2011Guid, IOrganizationServiceAsync2 svcClient)
        {
            Guid? retUser = null;

            try
            {
                Entity d365User = CrudCommon.GetEntityByCondition(UserConstants.Systemuser, svcClient, UserConstants.Mcorp_crm2011userguid, crm2011Guid.ToString().Replace("{", "").Replace("}", ""), false);

                if (!string.IsNullOrEmpty(d365User.LogicalName))
                {
                    retUser = d365User.Id;
                }
            }
            catch (Exception)
            {
            }

            return retUser;
        }
    }
}