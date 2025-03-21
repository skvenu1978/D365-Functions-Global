// <copyright file="FetchStrings.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Const;

    /// <summary>
    /// FetchStrings class.
    /// </summary>
    public class FetchStrings
    {
        /// <summary>
        /// GetRelatedDocumentLocation.
        /// </summary>
        /// <param name="entityName">entityName.</param>
        /// <param name="recordId">recordId.</param>
        /// <returns>string</returns>
        public static string GetRelatedDocumentLocation(string entityName, string recordId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sharepointdocumentlocation'>
                        <attribute name='name' />
                        <attribute name='regardingobjectid' />
                        <attribute name='parentsiteorlocation' />
                        <attribute name='relativeurl' />
                        <attribute name='absoluteurl' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='locationtype' operator='eq' value='0' />
                          <condition attribute='servicetype' operator='eq' value='0' />
                          <condition attribute='regardingobjectid' operator='eq' uitype='" + entityName + @"' value='{" + recordId + @"}' />
                        </filter>
                      </entity>
                    </fetch>";
        }

        public static string GetLinkedMainDocumentLocation(string entityName, string recordId, string recordDocLocationId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sharepointdocumentlocation'>
                        <attribute name='name' />
                        <attribute name='regardingobjectid' />
                        <attribute name='parentsiteorlocation' />
                        <attribute name='relativeurl' />
                        <attribute name='absoluteurl' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='locationtype' operator='eq' value='0' />
                          <condition attribute='servicetype' operator='eq' value='0' />
                          <condition attribute='regardingobjectid' operator='eq' uitype='" + entityName + @"' value='{" + recordId + @"}' />
                          <condition attribute='parentsiteorlocation' operator='eq' uitype='sharepointdocumentlocation' value='{" + recordDocLocationId + @"}' />
                        </filter>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// Fetch Xml for
        /// Omni Document location folder
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="recordId"></param>
        /// <param name="recordDocLocationId"></param>
        /// <returns></returns>
        public static string GetOmniDocumentsLocation(string entityName, string recordId, string recordDocLocationId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sharepointdocumentlocation'>
                        <attribute name='name' />
                        <attribute name='regardingobjectid' />
                        <attribute name='parentsiteorlocation' />
                        <attribute name='relativeurl' />
                        <attribute name='absoluteurl' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='locationtype' operator='eq' value='0' />
                          <condition attribute='servicetype' operator='eq' value='0' />
                            <condition attribute='name' operator='eq' value='" + SharePointConstants.SpOMNIFolder + @"' />
                          <condition attribute='regardingobjectid' operator='eq' uitype='" + entityName + @"' value='{" + recordId + @"}' />
                          <condition attribute='parentsiteorlocation' operator='eq' uitype='sharepointdocumentlocation' value='{" + recordDocLocationId + @"}' />
                        </filter>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// Fetch Xml for Parent Document Location.
        /// </summary>
        /// <param name="entityName">entityName</param>
        /// <returns>string</returns>
        public static string GetParentDocumentLocation(string entityName)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sharepointdocumentlocation'>
                        <attribute name='name' />
                        <attribute name='regardingobjectid' />
                        <attribute name='parentsiteorlocation' />
                        <attribute name='relativeurl' />
                        <attribute name='absoluteurl' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='locationtype' operator='eq' value='0' />
                          <condition attribute='servicetype' operator='eq' value='0' />
                          <condition attribute='relativeurl' operator='eq' value='" + entityName.ToLower() + @"' />
                        </filter>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// GetRootSharePointSite.
        /// </summary>
        /// <returns>string</returns>
        public static string GetRootSharePointSite()
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='sharepointsite'>
                        <attribute name='name' />
                        <attribute name='parentsite' />
                        <attribute name='relativeurl' />
                        <attribute name='absoluteurl' />
                        <attribute name='validationstatus' />
                        <attribute name='isdefault' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='servicetype' operator='eq' value='0' />
                          <condition attribute='isdefault' operator='eq' value='1' />
                          <condition attribute='validationstatus' operator='eq' value='4' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// GetMFRMAccountFetchCommon
        /// </summary>
        /// <param name="searchString">searchString</param>
        /// <param name="owningBusiness">owningBusiness</param>
        /// <returns>string</returns>
        public static string GetMFRMAccountFetchCommon(string searchString, int? owningBusiness)
        {
            string a = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='accountid' />
                        <attribute name='parentaccountid' />
                        <attribute name='mcorp_owningbusiness' />
                        <attribute name='mcorp_mifidcategory' />
                        <attribute name='mcorp_fedsid' />
                        <attribute name='mcorp_dealerid' />
                        <attribute name='mcorp_clienttype' />
                        <attribute name='mcorp_accountexecutiveid' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='statuscode' operator='eq' value='256550001' />
                          <condition attribute='statecode' operator='eq' value='0' />
                          <condition attribute='mcorp_mifidcategory' operator='ne' value='100000006' />
                           <condition attribute='name' operator='like' value='%" + searchString + @"%' />";

            string b = string.Empty;

            if (owningBusiness.HasValue)
            {
                b = @"<condition attribute='mcorp_owningbusiness'  operator='eq' value='" + owningBusiness.ToString() + @"' />";
            }

            string c = @"</filter>
                        <link-entity name='systemuser' from='systemuserid' to='mcorp_dealerid' visible='false' link-type='outer' alias='de'>
                          <attribute name='fullname' />
                        </link-entity>
                        <link-entity name='systemuser' from='systemuserid' to='mcorp_accountexecutiveid' visible='false' link-type='outer' alias='ae'>
                          <attribute name='fullname' />
                        </link-entity>
                      </entity>
                    </fetch>";

            return a + b + c;
        }

        /// <summary>
        /// GetRelatedContactsFetch
        /// </summary>
        /// <param name="accountid">accountid</param>
        /// <returns>string</returns>
        public static string GetRelatedContactsFetch(string accountid)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                  <entity name='contact'>
                    <attribute name='contactid' />
                    <attribute name='mcorp_omnicontactid' />
                    <attribute name='lastname' />
                    <attribute name='firstname' />
                    <attribute name='emailaddress2' />
                    <attribute name='emailaddress1' />
                    <attribute name='mobilephone' />
                    <order attribute='fullname' descending='false' />
                    <filter type='and'>
                      <condition attribute='mcorp_authorisationstatus' operator='eq' value='1' />
                      <condition attribute='statecode' operator='eq' value='0' />
                      <condition attribute='parentcustomerid' operator='eq' uitype='account' value='{" + accountid + @"}' />
                    </filter>
                    <link-entity name='account' from='accountid' to='parentcustomerid' visible='false' link-type='outer' alias='am'>
                      <attribute name='mcorp_mifidcategory' />
                    </link-entity>
                  </entity>
                </fetch>";
        }

        /// <summary>
        /// Gets Moneycorp Address
        /// Related to Account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string GetAddressRelatedToAccount(string accountId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                    <entity name='mcorp_moneycorpaddress'>
                        <attribute name='mcorp_moneycorpaddressid' />
                        <attribute name='mcorp_name' />
                        <attribute name='mcorp_mainphone' />
                        <attribute name='mcorp_street1' />
                        <attribute name='mcorp_street2' />
                        <attribute name='mcorp_street3' />
                        <attribute name='mcorp_city' />
                        <attribute name='mcorp_stateprovince' />
                        <attribute name='mcorp_zippostalcode' />
                        <attribute name='mcorp_country' />
                        <attribute name='mcorp_phone2' />
                        <order attribute='mcorp_name' descending='false' />
                        <filter type='and'>
                            <condition attribute='mcorp_parentaccount' operator='eq' uitype='account' value='{" + accountId + @"}' />
                            <condition attribute='mcorp_addresstype' operator='eq' value='1' />
                            <condition attribute='mcorp_objecttype' operator='eq' value='1' />
                        </filter>
                        <link-entity name='mcorp_country' from='mcorp_countryid' to='mcorp_country' visible='false' link-type='outer' alias='ct'>
                        <attribute name='mcorp_shortcode' />
                        </link-entity>
                    </entity>
                    </fetch>";
        }

        /// <summary>
        /// Get Account Fetch
        /// </summary>
        /// <param name="accountid"></param>
        /// <returns></returns>
        public static string GetDetailedAccountFetch(string accountid)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='account'>
                        <attribute name='name' />
                        <attribute name='accountid' />
                        <attribute name='mcorp_owningbusiness' />
                        <attribute name='mcorp_mifidcategory' />
                        <attribute name='mcorp_fedsid' />
                        <attribute name='mcorp_dealerid' />
                        <attribute name='mcorp_clienttype' />
                        <attribute name='mcorp_accountexecutiveid' />
                        <attribute name='accountnumber' />
                        <attribute name='emailaddress2' />
                        <attribute name='emailaddress1' />
                        <attribute name='telephone3' />
                        <attribute name='telephone2' />
                        <attribute name='telephone1' />
                        <attribute name='primarycontactid' />
                        <attribute name='parentaccountid' />
                        <order attribute='name' descending='false' />
                        <filter type='and'>
                          <condition attribute='accountid' operator='eq' uitype='account' value='{" + accountid + @"}' />
                        </filter>
                        <link-entity name='systemuser' from='systemuserid' to='mcorp_dealerid' visible='false' link-type='outer' alias='de'>
                          <attribute name='fullname' />
                        </link-entity>
                        <link-entity name='systemuser' from='systemuserid' to='mcorp_accountexecutiveid' visible='false' link-type='outer' alias='ae'>
                          <attribute name='fullname' />
                        </link-entity>
                        <link-entity name='contact' from='contactid' to='primarycontactid' visible='false' link-type='outer' alias='pc'>
                          <attribute name='fullname' />
                        </link-entity>
                      </entity>
                    </fetch>";
        }

        /// <summary>
        /// Get Detailed Contact.
        /// </summary>
        /// <param name="contactId">Contact Id.</param>
        /// <returns>FetchXml String.</returns>
        public static string GetDetailedContact(string contactId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='contact'>
                        <attribute name='fullname' />
                        <attribute name='contactid' />
                        <attribute name='mcorp_omnicontactid' />
                        <attribute name='lastname' />
                        <attribute name='firstname' />
                        <attribute name='emailaddress2' />
                        <attribute name='emailaddress1' />
                        <attribute name='mcorp_title' />
                        <attribute name='fax' />
                        <attribute name='donotpostalmail' />
                        <attribute name='donotemail' />
                        <attribute name='mcorp_clienttype' />
                        <attribute name='address1_postalcode' />
                        <attribute name='address1_line3' />
                        <attribute name='address1_line2' />
                        <attribute name='address1_line1' />
                        <attribute name='address1_stateorprovince' />
                        <attribute name='address1_county' />
                        <attribute name='address1_city' />
                         <attribute name='mobilephone' />
                         <attribute name='telephone2' />
                         <attribute name='telephone3' />
                            <attribute name='telephone1' />
                        <order attribute='fullname' descending='false' />
                        <filter type='and'>
                          <condition attribute='contactid' operator='eq' uitype='contact' value='{" + contactId + @"}' />
                          <condition attribute='mcorp_authorisationstatus' operator='eq' value='1' />
                          <condition attribute='statecode' operator='eq' value='0' />
                        </filter>
                        <link-entity name='account' from='accountid' to='parentcustomerid' visible='false' link-type='outer' alias='am'>
                          <attribute name='mcorp_mifidcategory' />
                        </link-entity>
                        <link-entity name='mcorp_countrycode' from='mcorp_countrycodeid' to='mcorp_homephonecountrycodeid' visible='false' link-type='outer' alias='hp'>
                          <attribute name='mcorp_countrydialingcode' />
                        </link-entity>
	                    <link-entity name='mcorp_countrycode' from='mcorp_countrycodeid' to='mcorp_workphonecountrycodeid' visible='false' link-type='outer' alias='wp'>
                          <attribute name='mcorp_countrydialingcode' />
                        </link-entity>
	                    <link-entity name='mcorp_countrycode' from='mcorp_countrycodeid' to='mcorp_mobilephonecountrycodeid' visible='false' link-type='outer' alias='mp'>
                          <attribute name='mcorp_countrydialingcode' />
                        </link-entity>
	                    <link-entity name='mcorp_countrycode' from='mcorp_countrycodeid' to='mcorp_otherphonecountrycodeid' visible='false' link-type='outer' alias='op'>
                          <attribute name='mcorp_countrydialingcode' />
                        </link-entity>
                        <link-entity name='mcorp_country' from='mcorp_countryid' to='mcorp_country' visible='false' link-type='outer' alias='ct'>
                           <attribute name='mcorp_shortcode' />
                         </link-entity>
                      </entity>
                    </fetch>";
        }

        public static string GetContactRelatedPrimaryAddress(string contactId)
        {
            return @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='mcorp_moneycorpaddress'>
                                    <attribute name='mcorp_moneycorpaddressid' />
                                    <attribute name='mcorp_name' />
                                    <attribute name='mcorp_addresstype' />
                                    <attribute name='mcorp_mainphone' />
                                    <attribute name='mcorp_phone2' />
                                    <order attribute='mcorp_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='mcorp_parentid' operator='eq' uitype='contact' value='{" + contactId + @"}' />
                                      <condition attribute='mcorp_addresstype' operator='eq' value='1' />
                                    </filter>
                                  </entity>
                                </fetch>";
        }
    }
}