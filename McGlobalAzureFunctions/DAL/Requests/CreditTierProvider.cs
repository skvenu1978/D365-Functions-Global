// <copyright file="CreditTierProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Requests
{
    using McGlobalAzureFunctions.Abstractions.Requests;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.Utilities;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Newtonsoft.Json;
    using System.ServiceModel;

    /// <summary>
    /// CreditTierProvider.
    /// </summary>
    public class CreditTierProvider : ICreditTierProvider
    {
        /// <summary>
        /// Processes Credit Tier from OMNI.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isUpdateFromOMNI">isUpdateFromOMNI</param>
        /// <param name="isProd">isProd</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>D365CreateOrUpdateCreditTierRequest</returns>
        public D365CreateOrUpdateCreditTierRequest GetCreateOrUpdateCreditTierRequest(Entity entity, IOrganizationServiceAsync2 svcClient, bool isUpdateFromOMNI, bool isProd, out string errMsg)
        {
            D365CreateOrUpdateCreditTierRequest createUpdateCreditTierInfoRequest = new D365CreateOrUpdateCreditTierRequest();
            D365CreditTierInfo creditTier = new D365CreditTierInfo();

            errMsg = string.Empty;

            try
            {
                Entity contextEntity = svcClient.Retrieve(CreditTierConstants.Mcorp_credittier, entity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (!string.IsNullOrEmpty(contextEntity.LogicalName) && contextEntity.LogicalName == CreditTierConstants.Mcorp_credittier)
                {
                    if (!isUpdateFromOMNI)
                    {
                        int? creditStatus = AttributeHelper.GetOptionSetValue(contextEntity, CreditTierConstants.Statuscode);

                        decimal? splitGPS = AttributeHelper.GetDecimalAttributeValue(contextEntity, CreditTierConstants.Mcorp_splitgps);
                        decimal? splitOMNI = AttributeHelper.GetDecimalAttributeValue(contextEntity, CreditTierConstants.Mcorp_splitomni);
                        decimal? splitOPTrade = AttributeHelper.GetDecimalAttributeValue(contextEntity, CreditTierConstants.Mcorp_splitoptrade);
                        decimal? deposit = AttributeHelper.GetDecimalAttributeValue(contextEntity, CreditTierConstants.Mcorp_deposit);
                        decimal? excessUtilisationOmni = AttributeHelper.GetDecimalAttributeValue(contextEntity, CreditTierConstants.Mcorp_excessutilisationomni);

                        DateTime? modified = AttributeHelper.GetDateTimeAttributeValue(contextEntity, CreditTierConstants.Modifiedon);
                        bool? deleted = AttributeHelper.GetBooleanValue(contextEntity, CreditTierConstants.Mcorp_isdeleted);
                        int? creditTerm = CalculateForwardTermDays(AttributeHelper.GetWholeNumberAttributeValue(contextEntity, CreditTierConstants.Mcorp_term));
                        int? creditLineId = AttributeHelper.GetWholeNumberAttributeValue(contextEntity, CreditTierConstants.Mcorp_creditlineid);
                        Guid? creditLineGuid = AttributeHelper.GetGuidFromEntityReference(contextEntity, CreditTierConstants.Mcorp_creditline);
                        Guid? createdById = AttributeHelper.GetGuidValue(contextEntity, CreditTierConstants.Createdby);
                        Guid? modifiedById = AttributeHelper.GetGuidValue(contextEntity, CreditTierConstants.Modifiedby);

                        decimal? amount = AttributeHelper.GetMoneyAttributeAsDecimal(contextEntity, CreditTierConstants.Mcorp_amount);
                        DateTime? created = AttributeHelper.GetDateTimeAttributeValue(contextEntity, CreditTierConstants.Createdon);

                        creditTier.CrmCreditTierId = contextEntity.Id;
                        
                        if (createdById.HasValue)
                        {
                            creditTier.CreatedBy = EntityCommon.GetCRM2011UserIdFromD365(createdById.Value, svcClient);
                        }

                        if (modifiedById.HasValue)
                        {
                            creditTier.ModifiedBy = EntityCommon.GetCRM2011UserIdFromD365(modifiedById.Value, svcClient);
                        }

                        if (!creditLineId.HasValue)
                        {
                            //This means that it is a create request on OMNI side so sending all 

                            if (amount.HasValue) { creditTier.Amount = amount.Value; }

                            if (created.HasValue) creditTier.CreatedOn = created.Value.ToLocalTime();
                            if (deposit.HasValue) creditTier.DepositPercentage = deposit.Value;
                            if (deleted.HasValue) creditTier.IsDeleted = deleted.Value;
                            creditTier.LineType = CreditTierType.CreditTier;
                           
                            if (modified.HasValue) creditTier.ModifiedOn = modified.Value.ToLocalTime();
                            if (splitGPS.HasValue) creditTier.SplitGps = splitGPS.Value;
                            if (splitOMNI.HasValue) creditTier.SplitOmni = splitOMNI.Value;
                            if (splitOPTrade.HasValue) creditTier.SplitOpTrade = splitOPTrade.Value;
                            if (creditStatus.HasValue) creditTier.Status = EnumHelper.ConvertCreditLineStatus(creditStatus.Value);
                            if (creditTerm.HasValue) creditTier.Term = creditTerm.Value;
                            if (creditLineGuid.HasValue)
                            {
                                creditTier.CrmCreditLineId = creditLineGuid.Value;
                            }
                        }
                        else
                        {
                            if (creditStatus.HasValue && creditStatus.Value == (int)CreditLineStatus.Approved)
                            {
                                if (splitGPS.HasValue) creditTier.SplitGps = splitGPS.Value;
                                if (splitOMNI.HasValue) creditTier.SplitOmni = splitOMNI.Value;
                                if (splitOPTrade.HasValue) creditTier.SplitOpTrade = splitOPTrade.Value;
                                if (modified.HasValue) creditTier.ModifiedOn = modified.Value.ToLocalTime();
                                if (creditLineId.HasValue) creditTier.CreditLineId = creditLineId.Value;
                                if (deleted.HasValue) creditTier.IsDeleted = deleted.Value;
                                creditTier.LineType = CreditTierType.CreditTier;
                                if (creditStatus.HasValue) creditTier.Status = CreditLineStatus.Approved;
                                if (creditTerm.HasValue) creditTier.Term = creditTerm.Value;
                                if (creditLineGuid.HasValue)
                                {
                                    creditTier.CrmCreditLineId = creditLineGuid.Value;
                                }
                            }
                            else
                            {
                                if (amount.HasValue) creditTier.Amount = amount.Value;
                                
                                if (created.HasValue) creditTier.CreatedOn = created.Value.ToLocalTime();
                                if (deposit.HasValue) creditTier.DepositPercentage = deposit.Value;

                                if (excessUtilisationOmni.HasValue) creditTier.ExcessUtilisationOmni = excessUtilisationOmni.Value;
                                if (deleted.HasValue) creditTier.IsDeleted = deleted.Value;
                                creditTier.LineType = CreditTierType.CreditTier;

                                if (modified.HasValue) creditTier.ModifiedOn = modified.Value.ToLocalTime();
                                if (splitGPS.HasValue) creditTier.SplitGps = splitGPS.Value;
                                if (splitOMNI.HasValue) creditTier.SplitOmni = splitOMNI.Value;
                                if (splitOPTrade.HasValue) creditTier.SplitOpTrade = splitOPTrade.Value;
                                if (creditStatus.HasValue) creditTier.Status = EnumHelper.ConvertCreditLineStatus(creditStatus.Value);
                                if (creditTerm.HasValue) creditTier.Term = creditTerm.Value;
                                if (creditLineId.HasValue) creditTier.CreditLineId = creditLineId.Value;
                                if (creditLineGuid.HasValue)
                                {
                                    creditTier.CrmCreditLineId = creditLineGuid.Value;
                                }
                            }
                        }

                        createUpdateCreditTierInfoRequest.CreditTier = creditTier;

                        //if (!isProd)
                        //{
                        //    creditTier.ModifiedBy = FnConstants.StaticUserId;
                        //    creditTier.CreatedBy = FnConstants.StaticUserId;
                        //}
                    }
                    else
                    {
                        errMsg = "Credit Tier cannot be sent to OMNI because this is an update from OMNI";
                    }
                }
                else
                {
                    errMsg = "Credit Tier does not exists for the CreateorUpdateCreditTier";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                CrmException strEx = ExceptionHandler.GetFormattedOrganizationServiceFault(ex);
                errMsg = JsonConvert.SerializeObject(strEx);
            }
            catch (Exception ex)
            {
                CrmException strEx = ExceptionHandler.GetFormattedSystemException(ex, "Failed to GetRequest for CreateOrUpdateCreditTier");
                errMsg = JsonConvert.SerializeObject(strEx);
            }

            return createUpdateCreditTierInfoRequest;
        }

        /// <summary>
        /// Updates the contact
        /// with MCOL permissions
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse UpdateCreditTier(UpdateCreditTierRequest omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            logger.LogInformation("UpdateCreditTier start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new Entity();

            try
            {
                entity = svcClient.Retrieve(CreditTierConstants.Mcorp_credittier, omniRequest.CreditTierId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "UpdateCreditTier-Unable to find credit tier with ID:" + omniRequest.CreditTierId + "-" + EntityCommon.GetErrorMessageFromException(ex));
                response.ErrorMessage = "UpdateCreditTier-Unable to find credit tier with ID:" + omniRequest.CreditTierId + "-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    var creditTier = omniRequest.CreditTier;
                    Entity updatedEntity = new Entity(entity.LogicalName, entity.Id);
                    updatedEntity[CreditTierConstants.Mcorp_deposit] = creditTier.DepositPercentage;
                    updatedEntity[CreditTierConstants.Mcorp_isdeleted] = creditTier.IsDeleted;
                    updatedEntity[CreditTierConstants.Mcorp_credittiertype] = new OptionSetValue((int)creditTier.LineType);
                    updatedEntity[CreditTierConstants.Mcorp_splitgps] = creditTier.SplitGps;
                    updatedEntity[CreditTierConstants.Mcorp_splitomni] = creditTier.SplitOmni;
                    updatedEntity[CreditTierConstants.Mcorp_splitoptrade] = creditTier.SplitOpTrade;
                    int? creditLineStatus = EnumHelper.ConvertOmniCreditLineStatus(creditTier.Status);
                    updatedEntity[CreditTierConstants.Mcorp_term] = CalculateForwardTermMonths((double)creditTier.Term);
                    updatedEntity[CreditTierConstants.Mcorp_excessutilisationomni] = creditTier.ExcessUtilisationOmni;
                    updatedEntity[CreditTierConstants.Mcorp_utilisationgps] = creditTier.UtilisationGps;
                    updatedEntity[CreditTierConstants.Mcorp_utilisationomni] = creditTier.UtilisationOmni;
                    updatedEntity[CreditTierConstants.Mcorp_utilisationoptrade] = creditTier.UtilisationOpTrade;
                    updatedEntity[CreditTierConstants.Mcorp_remainingbalance] = CalculateRemainingBalance(creditTier);
                    updatedEntity[CreditTierConstants.Mcorp_creditlineid] = creditTier.CreditLineId;

                    bool isChanged = false;
                    // Ensure that Total Utilisation is set.

                    // ExcessUtilisationOmni is actually the sum of "ExcessUtilisationOmni + ExcessUtilisationOpTrade" in omni db
                    // The view on OMNI DB has been modified as such to avoid MEX changes

                    if (creditTier.Utilisation != creditTier.UtilisationGps + creditTier.UtilisationOmni + creditTier.UtilisationOpTrade + creditTier.ExcessUtilisationOmni)
                    {
                        var totalUtilisation = creditTier.UtilisationGps + creditTier.UtilisationOmni + creditTier.UtilisationOpTrade + creditTier.ExcessUtilisationOmni;

                        if (totalUtilisation != creditTier.Utilisation)
                        {
                            creditTier.Utilisation = totalUtilisation;
                            updatedEntity[CreditTierConstants.Mcorp_totalutilisation] = creditTier.Utilisation;
                            isChanged = true;
                        }
                    }

                    if (isChanged)
                    {
                        updatedEntity[CreditTierConstants.Mcorp_isupdatefromomni] = true;
                        svcClient.Update(updatedEntity);
                    }

                    response.IsSuccess = true;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    logger.LogError(ex, "UpdateCreditTier-Failed to update credit tier:" + EntityCommon.GetErrorMessageFromException(ex));
                    response.ErrorMessage = "UpdateCreditTier-Failed to update credit tier:" + EntityCommon.GetErrorMessageFromException(ex);
                    response.Exception = ex;
                }
            }

            logger.LogInformation("UpdateCreditTier complete");

            return response;
        }

        /// <summary>
        /// CalculateRemainingBalance.
        /// </summary>
        /// <param name="cInfo">CreditTier Data</param>
        /// <returns>decimal</returns>
        private decimal CalculateRemainingBalance(D365CreditTierInfo cInfo)
        {
            var remaining = cInfo.Amount - cInfo.UtilisationOmni - cInfo.UtilisationGps - cInfo.UtilisationOpTrade;
            return remaining;
        }

        /// <summary>
        /// CalculateForwardTermMonths.
        /// </summary>
        /// <param name="days">days</param>
        /// <returns>int?</returns>
        private int? CalculateForwardTermMonths(double days)
        {
            if (days == default(double))
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(Math.Floor(days / 30.41));
            }
        }

        /// <summary>
        /// Calculates Forward Days.
        /// </summary>
        /// <param name="months">months</param>
        /// <returns>int?</returns>
        private static int? CalculateForwardTermDays(int? months)
        {
            int? retValue = null;

            if (months.HasValue)
            {
                var conv = Math.Ceiling(((double)months) * 30.41);
                int intRet = 0;
                bool isInt = int.TryParse(conv.ToString(), out intRet);

                if (isInt)
                {
                    retValue = intRet;
                }
            }

            return retValue;
        }
    }
}
