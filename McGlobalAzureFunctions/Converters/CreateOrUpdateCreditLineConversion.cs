// <copyright file="CreateOrUpdateCreditLineConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Converts credit line message to object for processing.
    /// </summary>
    public class CreateOrUpdateCreditLineConversion
    {
        /// <summary>
        /// Builds the request object using data from D365.
        /// </summary>
        /// <param name="creditLineEntity">Entity</param>
        /// <param name="svcClient">Service Client</param>
        /// <param name="errMsg">Out Err Mesage</param>
        /// <returns>D365CreateOrUpdateCreditLineRequest and bool isMessageValid</returns>
        public static (D365CreateOrUpdateCreditLineRequest,bool isMessageValid) GetCreateOrUpdateCreditLineRequest(Entity creditLineEntity, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            bool isMessageValid = false;
            D365CreateOrUpdateCreditLineRequest createUpdateCreditLineInfoRequest = new ();
            D365CreditLineInfo creditLine = new();

            errMsg = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(creditLineEntity.LogicalName) && creditLineEntity.LogicalName == CreditLineConstants.Mcorp_creditlines)
                {
                    Entity contextEntity = svcClient.Retrieve(CreditLineConstants.Mcorp_creditlines, creditLineEntity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                    int statusCode = AttributeHelper.GetStateStatusOptionSetValue(contextEntity, CreditLineConstants.Statuscode);

                    if (statusCode != (int)CreditLineStatus.Inactive)
                    {
                        isMessageValid = true;

                        bool isUpdateFromOMNI = AttributeHelper.GetBooleanValueDefaultFalse(contextEntity, CreditLineConstants.Mcorp_isupdatefromomni);

                        if (!isUpdateFromOMNI)
                        {
                            Guid? accountId = AttributeHelper.GetGuidFromEntityReference(contextEntity, CreditLineConstants.Mcorp_accountid);
                            creditLine.CrmCreditLineId = creditLineEntity.Id;

                            if (accountId.HasValue)
                            {
                                creditLine.CrmAccountId = accountId.Value;
                            }

                            //creditLine.CrmCreditDetailId = contextEntity.Id;
                            decimal? earlySettlement = AttributeHelper.GetMoneyAttributeAsDecimal(contextEntity, CreditLineConstants.Mcorp_earlysettlementamount);

                            if (earlySettlement.HasValue)
                            {
                                creditLine.DailySettlementLimit = earlySettlement.Value;
                            }

                            int? directDebitValueDate = AttributeHelper.GetOptionSetValue(contextEntity, CreditLineConstants.Mcorp_directdebitvaluedate);

                            if (directDebitValueDate.HasValue)
                            {
                                int clearanceDays = EnumHelper.ConvertDDClearance(directDebitValueDate.Value);
                                if (clearanceDays != 0) creditLine.DirectDebitClearanceDays = clearanceDays;
                            }

                            bool? directDebit = AttributeHelper.GetBooleanValue(contextEntity, CreditLineConstants.Mcorp_directdebit);

                            if (directDebit.HasValue)
                            {
                                creditLine.IsDirectDebit = directDebit.Value;
                            }

                            creditLine.MaxDirectDebitAmount = AttributeHelper.GetMoneyAttributeAsDecimal(contextEntity, CreditLineConstants.Mcorp_maximumdirectdebitamount);
                            int? creditStatus = AttributeHelper.GetOptionSetValue(contextEntity, CreditLineConstants.Statuscode);

                            if (creditStatus.HasValue)
                            {
                                creditLine.Status = EnumHelper.ConvertCreditLineStatus(creditStatus.Value);
                            }

                            int? tolerance = AttributeHelper.GetMoneyAttributeAsInteger(contextEntity, CreditLineConstants.Mcorp_tolerancelevel);

                            if (tolerance.HasValue)
                            {
                                creditLine.ToleranceLevel = tolerance.Value;
                            }

                            createUpdateCreditLineInfoRequest.CreditLine = creditLine;
                        }
                        else
                        {
                            errMsg = "Credit Line cannot be sent to OMNI because this is an update from OMNI";
                        }
                    }
                    else {
                        errMsg = "Credit Line cannot be sent to OMNI because this record is inactive";
                    }
                }
                else
                {
                    errMsg = "Credit Line does not exists for the CreateorUpdateCreditLine";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to GetRequest for CreateOrUpdateCreditLine:" + creditLineEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }

            return (createUpdateCreditLineInfoRequest,isMessageValid);
        }
    }
}