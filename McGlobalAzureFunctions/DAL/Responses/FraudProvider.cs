// <copyright file="FraudProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Extensions.Logging;
    using System.ServiceModel;

    using McGlobalAzureFunctions.Models.Responses;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.Models.Requests;
    using System.Text;

    /// <summary>
    /// Fraud Provider
    /// </summary>
    public class FraudProvider :IFraudProvider
    {
        /// <summary>
        /// reads the request from OMNI and updates D365.
        /// </summary>
        /// <param name="omniRequest">omniRequest</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns></returns>
        public DynTaskResponse ProcessFraudClientAuthorisationStatusChangeRequest(FraudClientAuthorisationStatusChangeRequest omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            DynTaskResponse accResponse = DynUpdateAccount(omniRequest, omniRequest.RequestId, svcClient, logger);
            response = accResponse;
            return response;
        }

        /// <summary>
        /// DynUpdateAccount.
        /// </summary>
        /// <param name="omniData">omniData</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        private DynTaskResponse DynUpdateAccount(FraudClientAuthorisationStatusChangeRequest omniData, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            DynTaskResponse response = new();
            response.IsSuccess = false;
            response.RecordId = null;
            logger.LogInformation("Inside the DynUpdateAccount method");

            try
            {
                logger.LogInformation("FraudClientAuthorisationStatusChangeRequest Account FedsId: " + omniData.FedsId);
                Entity d365Account = CrudCommon.GetEntityByCondition(AccountConstants.Account, svcClient, AccountConstants.Mcorp_fedsid, omniData.FedsId.ToString(), false);

                if (!string.IsNullOrEmpty(d365Account.LogicalName))
                {
                    Entity d365AccountToUpdate = new Entity(AccountConstants.Account, d365Account.Id);

                    int? accountStatus = AttributeHelper.GetOptionSetValue(d365Account, AccountConstants.Statuscode);

                    if (accountStatus.HasValue)
                    {
                        D365AccountStatus acntStatus = EnumHelper.ConvertAccountStatus((int)omniData.AccountStatus);

                        if (accountStatus.Value != (int)acntStatus)
                        {
                            CrudCommon.SetState(d365AccountToUpdate.Id, AccountConstants.Account, 0, (int)acntStatus, svcClient);
                            ChangeContactsAuthStatus(d365AccountToUpdate.Id, omniData.AccountStatus, svcClient, logger);
                        }

                        response.IsSuccess = true;
                        response.RecordId = d365AccountToUpdate.Id;
                    }
                }
                else
                {
                    logger.LogInformation("FraudClientAuthorisationStatusChangeRequest- No Account found  with FedsId: " + omniData.FedsId + " - for RequestId: " + requestId.ToString());
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "FraudClientAuthorisationStatusChange-Failed to update account FedsId:" + omniData.FedsId.ToString() + " - for RequestId: " + requestId.ToString() + " - " + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// ChangeContactsAuthStatus
        /// </summary>
        /// <param name="accountId">accountId</param>
        /// <param name="accountStatus">accountStatus</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        private void ChangeContactsAuthStatus(Guid accountId, McAccountStatus accountStatus, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            logger.LogInformation("changeContactsAuthStatus Method Begin in FraudClientAuthorisationStatusChangeTask");
            EntityCollection contacts = CrudCommon.GetEntitiesByCondition(ContactConstants.Contact, svcClient, ContactConstants.Parentcustomerid, accountId, true);

            if (contacts != null && contacts.Entities.Count > 0)
            {
                foreach (Entity contact in contacts.Entities)
                {
                    int? contactAuthStatus = AttributeHelper.GetOptionSetValue(contact, ContactConstants.Mcorp_authorisationstatus);
                    if (contactAuthStatus.HasValue)
                    {
                        if ((accountStatus == McAccountStatus.Authorised) && (contactAuthStatus.Value != (int)AuthorisationStatus.Authorised))
                            SetContactAuthStatus(contact.Id, AuthorisationStatus.Authorised, svcClient, logger);

                        if ((accountStatus != McAccountStatus.Authorised) && (contactAuthStatus.Value == (int)AuthorisationStatus.Authorised))
                            SetContactAuthStatus(contact.Id, AuthorisationStatus.Unauthorised, svcClient, logger);
                    }
                }
            }

            logger.LogInformation("changeContactsAuthStatus Method End in FraudClientAuthorisationStatusChangeTask");
        }

        /// <summary>
        /// SetContactAuthStatus.
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <param name="authStatus">authStatus</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        private void SetContactAuthStatus(Guid contactId, AuthorisationStatus authStatus, IOrganizationServiceAsync2 svcClient, ILogger logger)
        {
            logger.LogInformation("setContactAuthStatus Method Begin in FraudClientAuthorisationStatusChangeTask for Contact: " + contactId.ToString());
            Entity toUpdate = new Entity(ContactConstants.Contact);
            toUpdate.Id = contactId;
            toUpdate.Attributes[ContactConstants.Mcorp_authorisationstatus] = new OptionSetValue((int)authStatus);
            svcClient.Update(toUpdate);
            logger.LogInformation("setContactAuthStatus Method End in FraudClientAuthorisationStatusChangeTask for Contact: " + contactId.ToString());
        }
    }
}
