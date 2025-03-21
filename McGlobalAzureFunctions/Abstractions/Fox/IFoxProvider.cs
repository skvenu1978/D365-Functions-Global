// <copyright file="IFoxProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Abstractions.Fox
{
    using McGlobalAzureFunctions.Models.Fox;
    using Microsoft.PowerPlatform.Dataverse.Client;

    /// <summary>
    /// Interface for FOX Data Reader.
    /// </summary>
    public interface IFoxProvider
    {
        /// <summary>
        /// Gets the accounts based on name search.
        /// </summary>
        /// <param name="nameCriteria">nameCriteria.</param>
        /// <returns>AccountSearchResultMc.</returns>
        AccountSearchResultMc GetSampleMFRMAccountData(string nameCriteria);

        /// <summary>
        /// Gets the accounts using name and optional owning business.
        /// </summary>
        /// <param name="nameCriteria">nameCriteria</param>
        /// <param name="owningBusiness">owningBusiness</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>List AccountSearchResultMc</returns>
        List<AccountSearchResultMc> SearchMFRMAccount(string nameCriteria, int? owningBusiness, IOrganizationServiceAsync2 svcClient, out string errMsg);

        /// <summary>
        /// Get the contacts linked to the specific account.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="contactIds">contactIds</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>AccountContactDetailedSearchResultMc</returns>
        AccountContactDetailedSearchResultMc GetAccountRelatedContacts(string accountId, List<string> contactIds, IOrganizationServiceAsync2 svcClient, out string errMsg);

        /// <summary>
        /// Gets the linked authorised
        /// contacts
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>ContactSearchResultMc List</returns>
        List<ContactSearchResultMc> GetLinkedAuthorisedContacts(string accountId, IOrganizationServiceAsync2 svcClient, out string errMsg);
    }
}