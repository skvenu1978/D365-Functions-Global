// <copyright file="IntegrationFetchString.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Store for static
    /// fetch xml strings
    /// used in this project.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IntegrationFetchString
    {
        /// <summary>
        /// Fetch Xml string
        /// for Moneycorp Address
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetMoneycorpAddressFetch(string accountId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='mcorp_moneycorpaddress'>
                    <attribute name='mcorp_moneycorpaddressid'/>
                    <attribute name='mcorp_street3'/>
                    <attribute name='mcorp_street2'/>
                    <attribute name='mcorp_street1'/>
                    <attribute name='mcorp_city'/>
                    <attribute name='mcorp_county'/>
                    <attribute name='mcorp_zippostalcode'/>
                    <attribute name='mcorp_stateprovince'/>
                    <attribute name='mcorp_country'/>
                    <attribute name='mcorp_addresstype'/>
                    <order attribute='mcorp_name' descending='false'/>
                    <filter type='and'>
                         <condition attribute='mcorp_parentaccount' operator='eq' uitype='account' value='{" + accountId + @"}'/>                            
                        </filter>
                       <link-entity name='mcorp_country' from='mcorp_countryid' to='mcorp_country' visible='false' link-type='outer' alias='a_Country'>
                        <attribute name='statecode'/>
                        <attribute name='mcorp_runidv'/>
                        <attribute name='owningbusinessunit'/>
                        <attribute name='mcorp_name'/>
                        <attribute name='mcorp_iso2code'/>
                        </link-entity>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// Moneycorp address fetch
        /// linked to a contact
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public static string GetMoneycorpAddressContactFetch(string contactId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='mcorp_moneycorpaddress'>
                    <attribute name='mcorp_moneycorpaddressid'/>
                    <attribute name='mcorp_name'/>
                    <attribute name='createdon'/>
                    <attribute name='mcorp_zippostalcode'/>
                   <attribute name='mcorp_addresstype'/>
                    <order attribute='mcorp_name' descending='false'/>
                    <filter type='and'>
                         <condition attribute='mcorp_parentid' operator='eq' uitype='contact' value='{" + contactId + @"}'/>                            
                        </filter>
                       <link-entity name='mcorp_country' from='mcorp_countryid' to='mcorp_country' visible='false' link-type='outer' alias='a_Country'>
                        <attribute name='statecode'/>
                        <attribute name='mcorp_runidv'/>
                        <attribute name='owningbusinessunit'/>
                        <attribute name='mcorp_name'/>
                        <attribute name='mcorp_iso2code'/>
                        </link-entity>
                      </entity>
                    </fetch>";
        }

        public static string GetTradingCurrencyFetch(string accountid)
        {
            return @"<fetch version = '1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            <entity name='mcorp_tradingcurrency'>
            <attribute name='mcorp_tradingcurrencyid'/>
            <attribute name='mcorp_name'/>
            <attribute name='mcorp_typeoftransaction'/>
            <attribute name='mcorp_leadid'/>
            <attribute name='mcorp_accountid'/>
            <attribute name='mcorp_opportunityid'/>
            <attribute name='mcorp_currencyid'/>
            <attribute name='mcorp_transactionsfrequency'/>
            <attribute name='statuscode'/>
            <attribute name='statecode'/>
            <attribute name='owningbusinessunit'/>
            <attribute name='ownerid'/>
            <attribute name='mcorp_leadproduct'/>
            <attribute name='mcorp_customfrequency'/>
            <attribute name='mcorp_averagetransactionusd'/>
            <attribute name='mcorp_moneycorpaddressid'/>
            <attribute name='mcorp_accountproduct'/>
            <order attribute='mcorp_name' descending='false'/>
            <filter type = 'and'>
            <condition attribute='mcorp_accountid' operator= 'eq' uitype = 'account' value = '{" + accountid + @"}' />
            <condition attribute='statecode' operator='eq' value = '0'/>
            </filter>
            <link-entity name='transactioncurrency' from = 'transactioncurrencyid' to = 'mcorp_currencyid' visible = 'false' link-type = 'outer' alias = 'a_transactioncurrency'>
            <attribute name='isocurrencycode'/>
            <attribute name='currencysymbol'/>
            </link-entity>
            </entity>
            </fetch>";
        }
    }
}
