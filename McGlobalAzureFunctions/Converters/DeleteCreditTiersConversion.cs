// <copyright file="DeleteCreditTiersConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Const;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Converts Credit Tier Deletion Request to Object.
    /// </summary>
    public class DeleteCreditTiersConversion
    {
        /// <summary>
        /// GetDeleteCreditTiersRequest.
        /// </summary>
        /// <param name="contextEntityRef">contextEntityRef</param>
        /// <param name="logger">logger</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>DeleteCreditTiersRequest</returns>
        public static DeleteCreditTiersRequest GetDeleteCreditTiersRequest(EntityReference contextEntityRef, ILogger logger, out string errMsg)
        {
            DeleteCreditTiersRequest deleteCreditTiersRequest = new DeleteCreditTiersRequest();
            List<Guid> cdTiers = [];
            errMsg = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(contextEntityRef.LogicalName) && contextEntityRef.LogicalName == CreditTierConstants.Mcorp_credittier)
                {
                    cdTiers.Add(contextEntityRef.Id);
                    deleteCreditTiersRequest.CreditTierIds = cdTiers;
                }
                else
                {
                    errMsg = "Credit Tier does not exists for the DeleteCreditTiers";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "Failed to GetRequest for DeleteCreditTiers:" + contextEntityRef.Id);
                errMsg = "Failed to GetRequest for DeleteCreditTiers:" + contextEntityRef.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }

            return deleteCreditTiersRequest;
        }
    }
}
