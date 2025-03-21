// <copyright file="FoxContactReader.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Fox
{
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;
    using McGlobalAzureFunctions.Models.Fox;

    /// <summary>
    /// FoxContactReader Class.
    /// </summary>
    public class FoxContactReader
    {
        /// <summary>
        /// Get contacts linked to
        /// an account.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>ContactSearchResultMc list</returns>
        public static List<ContactSearchResultMc> GetLinkedAuthorisedContacts(string accountId, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            List<ContactSearchResultMc> results = [];
            errMsg = string.Empty;

            // read from D365
            string fetchXml = FetchStrings.GetRelatedContactsFetch(accountId);

            try
            {
                EntityCollection eColl = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

                if (eColl != null && eColl.Entities.Count > 0)
                {
                    results = ContactConverter.ContactToObject(eColl, svcClient);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to get Related Contacts:" + ex.Message;

                if (ex.InnerException != null)
                {
                    errMsg = "Failed to get Related Contacts:" + ex.InnerException.Message;
                }
            }

            return results;
        }
    }
}