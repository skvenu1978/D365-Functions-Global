// <copyright file="Validators.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Validators class.
    /// </summary>
    public class Validators
    {
        /// <summary>
        /// Get context entity.
        /// </summary>
        /// <param name="contextEntity">contextEntity.</param>
        /// <param name="entityNameMatch">entityNameMatch.</param>
        /// <param name="svcClient">svcClient.</param>
        /// <returns>Guid.</returns>
        public static Guid? GetAccountIdFromContextAndCheckEntity(Entity contextEntity, string entityNameMatch, IOrganizationServiceAsync2 svcClient)
        {
            Guid? accountId = null;

            if (contextEntity != null && !string.IsNullOrEmpty(contextEntity.LogicalName))
            {
                if (Equals(contextEntity.LogicalName.ToLower(), entityNameMatch.ToLower()))
                {
                    accountId = GetAccountIdFromContext(contextEntity, svcClient);
                }
            }

            return accountId;
        }

        /// <summary>
        /// Get Account Id from Context entity.
        /// </summary>
        /// <param name="contextEntity">contextEntity.</param>
        /// <param name="svcClient">svcClient.</param>
        /// <returns>Nullable Guid.</returns>
        public static Guid? GetAccountIdFromContext(Entity contextEntity, IOrganizationServiceAsync2 svcClient)
        {
            Guid? accountId = null;

            if (contextEntity != null && !string.IsNullOrEmpty(contextEntity.LogicalName))
            {
                switch (contextEntity.LogicalName)
                {
                    case "account":
                        accountId = contextEntity.Id;
                        break;
                    case "contact":
                        Entity contactEntity = svcClient.Retrieve(ContactConstants.Contact, contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(ContactConstants.Parentcustomerid, ContactConstants.Statecode));
                        int contactStateCode = AttributeHelper.GetStateStatusOptionSetValue(contactEntity, ContactConstants.Statecode);
                        accountId = AttributeHelper.GetGuidFromEntityReference(contactEntity, ContactConstants.Parentcustomerid);
                        break;
                    case "mcorp_moneycorpaddress":
                        Entity addressEntity = svcClient.Retrieve("mcorp_moneycorpaddress", contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(MoneycorpAddressConstants.mcorp_parentaccount));
                        accountId = AttributeHelper.GetGuidFromEntityReference(addressEntity, MoneycorpAddressConstants.mcorp_parentaccount);
                        break;
                    case "mcorp_alert":
                        Entity alertEntity = svcClient.Retrieve(AlertConstants.Mcorp_alert, contextEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(AlertConstants.Mcorp_accountid));
                        accountId = AttributeHelper.GetGuidFromEntityReference(alertEntity, AlertConstants.Mcorp_accountid);
                        break;
                }
            }

            return accountId;
        }

        /// <summary>
        /// Check if Owning Business parameter is within the set number of values.
        /// </summary>
        /// <param name="owningBusiness">owningBusiness.</param>
        /// <returns>bool.</returns>
        public static bool IsValidOwningBusiness(int owningBusiness)
        {
            bool isValid = false;

            switch (owningBusiness)
            {
                case 100000010:
                    isValid = true;
                    break;
                case 100000015:
                    isValid = true;
                    break;
                case 100000018:
                    isValid = true;
                    break;
                case 100000026:
                    isValid = true;
                    break;
                case 100000027:
                    isValid = true;
                    break;
                case 100000028:
                    isValid = true;
                    break;
                case 100000029:
                    isValid = true;
                    break;
            }

            return isValid;
        }

        /// <summary>
        /// Check if string is valid guid.
        /// </summary>
        /// <param name="accountid">accountid.</param>
        /// <returns>bool.</returns>
        public static bool IsValidGuid(string accountid)
        {
            bool isValid = false;

            if (!string.IsNullOrEmpty(accountid))
            {
                Guid acId = Guid.Empty;
                bool isGuid = Guid.TryParse(accountid, out acId);

                if (isGuid)
                {
                    isValid = true;
                }
            }

            return isValid;
        }
    }
}