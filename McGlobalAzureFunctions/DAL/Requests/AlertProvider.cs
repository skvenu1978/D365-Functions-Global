// <copyright file="AlertProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Requests
{
    using McGlobalAzureFunctions.Abstractions.Requests;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Main class for Alerts
    /// </summary>
    public class AlertProvider : IAlertProvider
    {
        /// <summary>
        /// Gets the alert request data from D365.
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="svcClient"></param>
        /// <param name="_logger"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public AlertInfoRequest GetAlertRequest(Guid accountId, IOrganizationServiceAsync2 svcClient, ILogger _logger, out string errMsg)
        {
            _logger.LogInformation("GetAlertRequest started");

            AlertInfoRequest alertInfo = new AlertInfoRequest();
            errMsg = string.Empty;

            try
            {
                Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, accountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Accountnumber));
                int? accountNumber = AttributeHelper.GetStringAttributeAsIntegerValue(parentAccount, AccountConstants.Accountnumber);
                EntityCollection alerts = new EntityCollection();
                List<AlertInfoDetails> lstAlertsInfoDetails = [];
                List<ContactInfoDetails> lstContactnfo = [];
                alerts = EntityCommon.GetAlertsRelatedToAccount(accountId, svcClient);

                if (alerts.Entities != null && alerts.Entities.Count > 0)
                {
                    foreach (Entity alert in alerts.Entities)
                    {
                        AlertInfoDetails alertInfoDetails = new AlertInfoDetails();
                        string rowErrMsg = string.Empty;

                        alertInfoDetails = AlertDetailToObject(alert, svcClient, accountNumber, out rowErrMsg);

                        if (string.IsNullOrEmpty(rowErrMsg))
                        {
                            lstAlertsInfoDetails.Add(alertInfoDetails);
                        }
                        else
                            errMsg += rowErrMsg;
                    }

                    if (lstAlertsInfoDetails.Count > 0)
                    {
                        alertInfo = new AlertInfoRequest();
                        alertInfo.AlertsInfoDetails = lstAlertsInfoDetails;
                    }
                }
                else
                {
                    errMsg = "This account does not have any alerts";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to GetRequest for Alert:" + accountId.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                _logger.LogError(ex, $"Failed to GetRequest for Alert{0}", accountId);
            }
            catch (Exception ex)
            {
                errMsg = "Failed to GetRequest for Alert:" + accountId.ToString() + " with error:" + EntityCommon.GetErrorMessageFromException(ex);
                _logger.LogError(ex, "Failed to GetRequest for Alert:" + accountId);
            }

            _logger.LogInformation("GetAlertRequest exited");

            return alertInfo;
        }

        /// <summary>
        /// Get data from D365
        /// and prepare object
        /// </summary>
        /// <param name="alert">Entity</param>
        /// <param name="svcClient">service Client</param>
        /// <param name="accountNumber">Crm account number</param>
        /// <param name="errMsg">out error message</param>
        /// <returns></returns>
        private AlertInfoDetails AlertDetailToObject(Entity alert, IOrganizationServiceAsync2 svcClient, int? accountNumber, out string errMsg)
        {
            AlertInfoDetails alertInfo = new AlertInfoDetails();
            errMsg = string.Empty;

            try
            {
                if (accountNumber.HasValue) alertInfo.CRMId = accountNumber.Value;
                else
                {
                    errMsg = "Account CRM ID is null or empty for ALERT - no alerts id present";
                }

                alertInfo.CRMAlertGuid = alert.Id;
                alertInfo.AlertText = AttributeHelper.GetStringAttributeValue(alert, AlertConstants.Mcorp_text);
                Guid? createdAlert = AttributeHelper.GetGuidFromEntityReference(alert, AlertConstants.Createdby);

                if (createdAlert.HasValue)
                {
                    alertInfo.CreatedBy = EntityCommon.GetCRM2011UserIdFromD365(createdAlert.Value, svcClient);
                }

                DateTime? dateCreated = AttributeHelper.GetDateTimeAttributeValue(alert, AlertConstants.Createdon);
                alertInfo.CreatedOn = dateCreated.HasValue ? dateCreated.Value.ToLocalTime() : default(DateTime);

                DateTime? dateModified = AttributeHelper.GetDateTimeAttributeValue(alert, AlertConstants.Modifiedon);
                alertInfo.ModifiedOn = dateModified.HasValue ? dateModified.Value.ToLocalTime() : default(DateTime);

                Guid? modifiedAlert = AttributeHelper.GetGuidFromEntityReference(alert, AlertConstants.Modifiedby);
                if (modifiedAlert != null)
                {
                    alertInfo.ModifiedBy = EntityCommon.GetCRM2011UserIdFromD365(modifiedAlert.Value, svcClient);
                }

                int? alertType = AttributeHelper.GetOptionSetValue(alert, AlertConstants.Mcorp_type);
                if (alertType.HasValue) alertInfo.ClientAlertType = this.SelectClientAlertType(alertType.Value);

                alertInfo.ClientAlertSubjectType = ClientAlertSubjectType.None;
                DateTime? expDate = AttributeHelper.GetDateTimeAttributeValue(alert, AlertConstants.Mcorp_expiry);
                alertInfo.ExpiryDate = expDate.HasValue ? expDate.Value.ToLocalTime() : new Nullable<DateTime>();


                int? alertStatecode = AttributeHelper.GetOptionSetValue(alert, AlertConstants.Statecode);
                if (alertStatecode.HasValue)
                {
                    alertInfo.IsActive = alertStatecode.Value == 0 ? true : false;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed in AlertDetailToObject:" + alert.Id + "with error: " + EntityCommon.GetErrorMessageFromFaultException(ex);
            }
            catch (Exception ex)
            {
                errMsg = "Failed in AlertDetailToObject:" + alert.Id + "with error: " + EntityCommon.GetErrorMessageFromException(ex);
            }

            return alertInfo;
        }

        /// <summary>
        /// Client Alert Type
        /// Setter
        /// </summary>
        /// <param name="alertType"></param>
        /// <returns></returns>
        private ClientAlertType SelectClientAlertType(int alertType)
        {
            ClientAlertType clientalerttype = ClientAlertType.Information;

            switch (alertType)
            {
                case 3:
                    clientalerttype = ClientAlertType.Critical;
                    break;
                case 1:
                    clientalerttype = ClientAlertType.Information;
                    break;
                case 2:
                    clientalerttype = ClientAlertType.Warning;
                    break;
            }
            return clientalerttype;
        }
    }
}