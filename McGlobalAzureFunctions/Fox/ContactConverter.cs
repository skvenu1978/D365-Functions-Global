// <copyright file="ContactConverter.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Fox
{
    using McGlobalAzureFunctions.Models.Fox;
    using McGlobalAzureFunctions.Const;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;

    public class ContactConverter
    {
        /// <summary>
        /// Converts Contact to
        /// ContactSearchResultMc object
        /// </summary>
        /// <param name="results"></param>
        /// <param name="svcClient"></param>
        /// <returns></returns>
        public static List<ContactSearchResultMc> ContactToObject(EntityCollection results, IOrganizationServiceAsync2 svcClient)
        {
            List<ContactSearchResultMc> collection = [];

            foreach (Entity entity in results.Entities)
            {
                ContactSearchResultMc data = new ContactSearchResultMc();

                data.McId = entity.Id;
                data.FirstName = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Firstname);
                data.LastName = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Lastname);
                data.OmniContactId = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Mcorp_omnicontactid);

                data.Email1 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Emailaddress1);
                data.Email2 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Emailaddress2);
                data.MobilePhone = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Mobilephone);

                data.MifidCategory = AttributeHelper.GetAliasedOptionSetValue(entity, ContactConstants.Acmcorp_mifidcategory);
                data.MifidCategoryString = AttributeHelper.GetAliasedOptionSetTextValue(entity, ContactConstants.Acmcorp_mifidcategory);

                if (!data.MifidCategory.HasValue)
                {
                    data.MifidCategory = 0;
                }

                collection.Add(data);
            }

            return collection;
        }

        /// <summary>
        /// Converts Contact entity
        /// to Contact Object.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>ContactDetailedSearchResultMc</returns>
        public static ContactDetailedSearchResultMc ContactDetailToObject(Entity entity)
        {
            ContactDetailedSearchResultMc data = new ContactDetailedSearchResultMc();
            data.McId = entity.Id;
            data.FirstName = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Firstname);
            data.LastName = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Lastname);
            data.Email1 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Emailaddress1);
            data.Email2 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Emailaddress2);
            data.Address_City = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_city);
            data.Address_Country = AttributeHelper.GetAliasedStringValue(entity, ContactConstants.Ctmcorp_shortcode);
            data.Address_County = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_stateorprovince);
            data.Address_Line_1 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_line1);
            data.Address_Line_2 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_line2);
            data.Address_Line_3 = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_line3);
            data.Address_Postal_Code = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Address1_postalcode);
            data.Fax = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Fax);
            data.Home_Country_Code = AttributeHelper.GetAliasedStringValue(entity, ContactConstants.HomeCountryCode);
            data.Home_Phone = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Telephone2);
            data.Work_Country_Code = AttributeHelper.GetAliasedStringValue(entity, ContactConstants.WorkCountryCode);
            data.Work_Phone = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Telephone1);
            data.Mobile_Country_Code = AttributeHelper.GetAliasedStringValue(entity, ContactConstants.MobileCountryCode);
            data.Mobile_Phone = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Mobilephone);
            data.Other_Country_Code = AttributeHelper.GetAliasedStringValue(entity, ContactConstants.OtherCountryCode);
            data.Other_Phone = AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Telephone3);
            data.OmniId = int.Parse(AttributeHelper.GetStringAttributeValue(entity, ContactConstants.Mcorp_omnicontactid));
            data.Title = AttributeHelper.GetOptionSetTextValue(entity, ContactConstants.Mcorp_title);
            data.ClientType = AttributeHelper.GetOptionSetValue(entity, ContactConstants.Mcorp_clienttype);
            data.DoNotEmail = AttributeHelper.GetBooleanValue(entity, ContactConstants.Donotemail);
            data.DoNotPostalMail = AttributeHelper.GetBooleanValue(entity, ContactConstants.Donotpostalmail);

            return data;
        }
    }
}
