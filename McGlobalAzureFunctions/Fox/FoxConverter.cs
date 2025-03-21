// <copyright file="FoxConverter.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Fox
{
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Fox;
    using McGlobalAzureFunctions.Const;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// FoxConverter class
    /// </summary>
    public class FoxConverter
    {
        /// <summary>
        /// Convert Account Entity
        /// collection to object.
        /// </summary>
        /// <param name="results">results</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>AccountSearchResultMc List</returns>
        public static List<AccountSearchResultMc> EntityToObject(EntityCollection results, IOrganizationServiceAsync2 svcClient)
        {
            List<AccountSearchResultMc> collection = [];

            foreach (Entity entity in results.Entities)
            {
                AccountSearchResultMc data = new AccountSearchResultMc();

                data.McId = entity.Id;
                data.Name = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Name);

                data.ClientType = AttributeHelper.GetOptionSetValue(entity, AccountConstants.Mcorp_clienttype);
                data.ParentAccountId = AttributeHelper.GetGuidValue(entity, AccountConstants.Parentaccountid);

                if (data.ParentAccountId.HasValue)
                {
                    Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, data.ParentAccountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Mcorp_fedsid));

                    if (!string.IsNullOrEmpty(parentAccount.LogicalName))
                    {
                        data.FedsId = EntityCommon.GetFedsId(parentAccount);
                    }
                }
                else
                {
                    data.FedsId = EntityCommon.GetFedsId(entity);
                }

                data.AccountDealerId = AttributeHelper.GetGuidValue(entity, AccountConstants.Mcorp_dealerid);

                if (data.AccountDealerId.HasValue)
                {
                    data.AccountDealerFullName = AttributeHelper.GetAliasedStringValue(entity, AccountConstants.Defullname);
                }

                data.AccountExecutiveId = AttributeHelper.GetGuidValue(entity, AccountConstants.Mcorp_accountexecutiveid);

                if (data.AccountExecutiveId.HasValue)
                {
                    data.AccountExecutiveFullName = AttributeHelper.GetAliasedStringValue(entity, AccountConstants.Aefullname);
                }

                data.MifidCategory = AttributeHelper.GetOptionSetValue(entity, AccountConstants.Mcorp_mifidcategory);
                data.MifidCategoryString = AttributeHelper.GetOptionSetTextValue(entity, AccountConstants.Mcorp_mifidcategory);

                if (!data.MifidCategory.HasValue)
                {
                    data.MifidCategory = 0;
                }

                data.OwningBusiness = AttributeHelper.GetOptionSetValue(entity, AccountConstants.Mcorp_owningbusiness);

                collection.Add(data);
            }

            return collection;
        }

        /// <summary>
        /// Convert Account Entity to Object.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>AccountDetailedSearchResultMc</returns>
        public static AccountDetailedSearchResultMc AccountToObject(Entity entity, IOrganizationServiceAsync2 svcClient)
        {
            AccountDetailedSearchResultMc data = new()
            {
                McId = entity.Id,
                Name = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Name),
                CRM_Customer_ID = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Accountnumber),
                Primary_Contact_ID = AttributeHelper.GetGuidValue(entity, AccountConstants.Primarycontactid)
            };

            if (data.Primary_Contact_ID.HasValue)
            {
                data.Primary_Contact_Name = AttributeHelper.GetAliasedStringValue(entity, AccountConstants.Pcfullname);
            }

            data.ClientType = AttributeHelper.GetOptionSetValue(entity, AccountConstants.Mcorp_clienttype);
            Guid? parentAccountId = AttributeHelper.GetGuidValue(entity, AccountConstants.Parentaccountid);

            if (parentAccountId.HasValue)
            {
                Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, parentAccountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Mcorp_fedsid));

                if (!string.IsNullOrEmpty(parentAccount.LogicalName))
                {
                    int? fedsID = EntityCommon.GetFedsId(parentAccount);

                    if (fedsID.HasValue)
                    {
                        data.OmniId = fedsID.Value.ToString();
                    }
                }
            }
            else
            {
                int? fedsID = EntityCommon.GetFedsId(entity);

                if (fedsID.HasValue)
                {
                    data.OmniId = fedsID.Value.ToString();
                }
            }

            data.AccountDealerId = AttributeHelper.GetGuidValue(entity, AccountConstants.Mcorp_dealerid);

            if (data.AccountDealerId.HasValue)
            {
                data.AccountDealerFullName = AttributeHelper.GetAliasedStringValue(entity, AccountConstants.Defullname);
            }

            data.AccountExecutiveId = AttributeHelper.GetGuidValue(entity, AccountConstants.Mcorp_accountexecutiveid);

            if (data.AccountExecutiveId.HasValue)
            {
                data.AccountExecutiveFullName = AttributeHelper.GetAliasedStringValue(entity, AccountConstants.Aefullname);
            }

            data.MifidCategory = AttributeHelper.GetOptionSetValue(entity, AccountConstants.Mcorp_mifidcategory);
            data.MifidCategoryString = AttributeHelper.GetOptionSetTextValue(entity, AccountConstants.Mcorp_mifidcategory);

            if (!data.MifidCategory.HasValue)
            {
                data.MifidCategory = 0;
            }

            data.Email_1 = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Emailaddress1);
            data.Email_2 = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Emailaddress2);
            data.Fax = AttributeHelper.GetStringAttributeValue(entity, AccountConstants.Fax);

            // get related moneycorp address
            string fetchXml = FetchStrings.GetAddressRelatedToAccount(entity.Id.ToString());
            EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

            if (eColl != null && eColl.Entities.Count > 0)
            {
                Entity addressEntity = eColl.Entities[0];
                data.Main_Phone = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_mainphone);
                data.Other_Phone = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_phone2);
                data.Address_Line_1 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street1);
                data.Address_Line_2 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street2);
                data.Address_Line_3 = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_street3);
                data.Address_City = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_city);
                data.Address_County = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_stateprovince);
                data.Address_Postal_Code = AttributeHelper.GetStringAttributeValue(addressEntity, AddressConstants.Mcorp_zippostalcode);
                data.Address_Country = AttributeHelper.GetAliasedStringValue(addressEntity, AddressConstants.Ctmcorp_shortcode);
            }

            return data;
        }
    }
}