// <copyright file="RateProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Converters;
    using McGlobalAzureFunctions.DAL;
    using McGlobalAzureFunctions.FunctionsCommon;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Requests;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Rate Provider
    /// </summary>
    public class RateProvider : IRateProvider
    {
        /// <summary>
        /// GetRateTrackerOpportunityRequest
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="isCreate">isCreate</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="_logger">_logger</param>
        /// <param name="errMsg">errMsg</param>
        /// <returns>RateTrackerOpportunityRequest</returns>
        public D365RateTrackerOpportunityRequest GetRateTrackerOpportunityRequest(Entity entity, bool isCreate, IOrganizationServiceAsync2 svcClient, ILogger<FnD365RateTrackerOpportunityRequestToAsb> _logger, out string errMsg)
        {
            D365RateTrackerOpportunityRequest request = new D365RateTrackerOpportunityRequest();
            request.RateTrackerOpportunity = new D365RateTrackerOpportunityInfo();
            errMsg = string.Empty;

            Entity contextEntity = svcClient.Retrieve(OptyConstants.opportunity, entity.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            if (contextEntity.LogicalName == OptyConstants.opportunity)
            {
                int? optyType = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_opportunitytype);
                int? optySource = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_opportunitysource);

                bool checkAlerts = AttributeHelper.GetBooleanValueDefaultFalse(contextEntity, OptyConstants.mcorp_checkalerts);
                int optyStateCode = AttributeHelper.GetStateStatusOptionSetValue(contextEntity, OptyConstants.statecode);

                if (checkAlerts && optyStateCode == (int)OpportunityState.Open)
                {
                    try
                    {
                        request.RateTrackerOpportunity.OpportunityId = contextEntity.Id;

                        decimal? bankRate = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_bankrate); //float
                        decimal? stopRate = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_stoprate); //float
                        decimal? limitRate = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_limitrate); //float
                        decimal? bankStopRate = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_banksidestoprate); //float
                        decimal? buyAmount = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_buyamount); //float
                        decimal? sellAmount = AttributeHelper.GetFloatAttributeAsDecimal(contextEntity, OptyConstants.mcorp_sellamount); // float
                        int? baseCurrency = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_basecurrency);
                        int? rateLimitAlertSubscriptionId = AttributeHelper.GetWholeNumberAttributeValue(contextEntity, OptyConstants.mcorp_limitratealertsubscriptionid);
                        int? stopRateAlertSubcriptionId = AttributeHelper.GetWholeNumberAttributeValue(contextEntity, OptyConstants.mcorp_stopratealertsubscriptionid);

                        int? stopOrLimit = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_stoporlimit);

                        if (bankRate.HasValue && bankStopRate.HasValue)
                        {
                            _logger.LogInformation("Bank Rate and Stop Rate Has Value");

                            request.RateTrackerOpportunity.AlertType = AlertType.OneCancelsOther;

                            if (baseCurrency.HasValue && baseCurrency.Value == (int)BaseCurrency.Sell)
                            {
                                request.RateTrackerOpportunity.BaseIsSell = true;

                                if (rateLimitAlertSubscriptionId.HasValue) 
                                    request.RateTrackerOpportunity.LimitRateAlertSubscriptionId = rateLimitAlertSubscriptionId.Value;
                                if (stopRateAlertSubcriptionId.HasValue) 
                                    request.RateTrackerOpportunity.StopRateAlertSubscriptionId = stopRateAlertSubcriptionId.Value;
                                if (limitRate.HasValue) 
                                    request.RateTrackerOpportunity.LimitRateLimitedRequested = limitRate.Value;
                                if (bankRate.HasValue) 
                                    request.RateTrackerOpportunity.LimitInterbankRate = bankRate.Value;
                                if (stopRate.HasValue) 
                                    request.RateTrackerOpportunity.StopRateLimitedRequested = stopRate.Value;
                                if (bankStopRate.HasValue) 
                                    request.RateTrackerOpportunity.StopInterbankRate = bankStopRate.Value;
                            }
                            else
                            {
                                //Buy
                                request.RateTrackerOpportunity.BaseIsSell = false;
                                //Limit to Stop
                                if (rateLimitAlertSubscriptionId.HasValue) 
                                    request.RateTrackerOpportunity.StopRateAlertSubscriptionId = rateLimitAlertSubscriptionId.Value;
                                if (stopRateAlertSubcriptionId.HasValue) 
                                    request.RateTrackerOpportunity.LimitRateAlertSubscriptionId = stopRateAlertSubcriptionId.Value;
                                if (limitRate.HasValue) 
                                    request.RateTrackerOpportunity.StopRateLimitedRequested = limitRate.Value;
                                if (bankRate.HasValue) 
                                    request.RateTrackerOpportunity.StopInterbankRate = bankRate.Value;
                                if (stopRate.HasValue) 
                                    request.RateTrackerOpportunity.LimitRateLimitedRequested = stopRate.Value;
                                if (bankStopRate.HasValue) 
                                    request.RateTrackerOpportunity.LimitInterbankRate = bankStopRate.Value;
                            }
                        }
                        else if (stopOrLimit.HasValue)
                        {
                            _logger.LogInformation("Stop or Limit Has Value");

                            if (stopOrLimit.Value == (int)StopOrLimit.Limit)
                            {
                                if (baseCurrency.HasValue && baseCurrency.Value == (int)BaseCurrency.Sell)
                                {
                                    //Sell
                                    request.RateTrackerOpportunity.BaseIsSell = true;
                                    request.RateTrackerOpportunity.AlertType = AlertType.Above;

                                    //Limit to Limit
                                    if (rateLimitAlertSubscriptionId.HasValue) 
                                        request.RateTrackerOpportunity.LimitRateAlertSubscriptionId = rateLimitAlertSubscriptionId.Value;
                                    if (limitRate.HasValue) 
                                        request.RateTrackerOpportunity.LimitRateLimitedRequested = limitRate.Value;
                                    if (bankRate.HasValue) 
                                        request.RateTrackerOpportunity.LimitInterbankRate = bankRate.Value;
                                }
                                else
                                {
                                    //Buy
                                    request.RateTrackerOpportunity.BaseIsSell = false;
                                    request.RateTrackerOpportunity.AlertType = AlertType.Below;
                                    //Limit to Stop
                                    if (rateLimitAlertSubscriptionId.HasValue && rateLimitAlertSubscriptionId.Value != default(int)) 
                                        request.RateTrackerOpportunity.StopRateAlertSubscriptionId = rateLimitAlertSubscriptionId.Value;
                                    if (limitRate.HasValue && limitRate.Value != default(decimal)) 
                                        request.RateTrackerOpportunity.StopRateLimitedRequested = limitRate.Value;
                                    if (bankRate.HasValue && bankRate.Value != default(decimal)) 
                                        request.RateTrackerOpportunity.StopInterbankRate = bankRate.Value;
                                }
                            }

                            if (stopOrLimit.Value == (int)StopOrLimit.Stop)
                            {
                                if (baseCurrency.HasValue && baseCurrency.Value == (int)BaseCurrency.Sell)
                                {
                                    //Sell
                                    request.RateTrackerOpportunity.BaseIsSell = true;
                                    request.RateTrackerOpportunity.AlertType = AlertType.Below;

                                    //stop to stop
                                    if (stopRateAlertSubcriptionId.HasValue && stopRateAlertSubcriptionId.Value != default(int)) 
                                        request.RateTrackerOpportunity.StopRateAlertSubscriptionId = stopRateAlertSubcriptionId.Value;
                                    if (stopRate.HasValue && stopRate.Value != default(decimal))
                                        request.RateTrackerOpportunity.StopRateLimitedRequested = stopRate.Value;
                                    if (bankStopRate.HasValue && bankStopRate.Value != default(decimal)) 
                                        request.RateTrackerOpportunity.StopInterbankRate = bankStopRate.Value;
                                }
                                else
                                {
                                    //Buy
                                    request.RateTrackerOpportunity.BaseIsSell = false;
                                    request.RateTrackerOpportunity.AlertType = AlertType.Above;

                                    //stop to stop
                                    if (stopRateAlertSubcriptionId.HasValue && stopRateAlertSubcriptionId.Value != default(int))
                                    {
                                        request.RateTrackerOpportunity.LimitRateAlertSubscriptionId = stopRateAlertSubcriptionId.Value;
                                    }

                                    if (stopRate.HasValue && stopRate.Value != default(decimal))
                                    {
                                        request.RateTrackerOpportunity.LimitRateLimitedRequested = stopRate.Value;
                                    }

                                    if (bankStopRate.HasValue && bankStopRate.Value != default(decimal))
                                    {
                                        request.RateTrackerOpportunity.LimitInterbankRate = bankStopRate.Value;
                                    }
                                }
                            }
                        }
                        else
                        {
                            errMsg = "Values provided in bankrate / bankside and stopLimit is not correct!";
                        }

                        Guid? buyCurrencyGuid = AttributeHelper.GetGuidFromEntityReference(contextEntity, OptyConstants.mcorp_opportunitybuycurrencyid);
                        Guid? sellCurrencyGuid = AttributeHelper.GetGuidFromEntityReference(contextEntity, OptyConstants.mcorp_opportunitysellcurrencyid);
                        string buyCurrenyName = string.Empty;
                        string sellCurrencyName = string.Empty;
                        
                        if (buyCurrencyGuid.HasValue)
                        {
                            Entity buyCurrency = svcClient.Retrieve(OptyConstants.transactioncurrency, buyCurrencyGuid.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                            if (!string.IsNullOrEmpty(buyCurrency.LogicalName))
                            {
                                buyCurrenyName = AttributeHelper.GetStringAttributeValue(buyCurrency, OptyConstants.isocurrencycode);
                            }
                        }

                        if (sellCurrencyGuid.HasValue)
                        {
                            Entity sellCurrency = svcClient.Retrieve(OptyConstants.transactioncurrency, sellCurrencyGuid.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                            if (!string.IsNullOrEmpty(sellCurrency.LogicalName))
                            {
                                sellCurrencyName = AttributeHelper.GetStringAttributeValue(sellCurrency, OptyConstants.isocurrencycode);
                            }
                        }

                        if (request.RateTrackerOpportunity.BaseIsSell)
                        {
                            if (!string.IsNullOrEmpty(sellCurrencyName)) 
                                request.RateTrackerOpportunity.BaseCCY = sellCurrencyName;
                            if (!string.IsNullOrEmpty(buyCurrenyName)) 
                                request.RateTrackerOpportunity.TermsCCY = buyCurrenyName;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(sellCurrencyName)) 
                                request.RateTrackerOpportunity.TermsCCY = sellCurrencyName;
                            if (!string.IsNullOrEmpty(buyCurrenyName)) 
                                request.RateTrackerOpportunity.BaseCCY = buyCurrenyName;
                        }

                        bool? dealtBase = AttributeHelper.GetBooleanValue(contextEntity, OptyConstants.mcorp_dealtisbase);
                        request.RateTrackerOpportunity.DealtIsBase = dealtBase.HasValue ? dealtBase.Value : false;
                        int? opportSource = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_opportunitysource);
                        
                        if (opportSource.HasValue)
                        {
                            request.RateTrackerOpportunity.RateAlertOriginatingSystem = (opportSource.Value == (int)OpportunitySource.MCOL) ? SourceContact.MoneycorpOnline : SourceContact.Crm;
                        }
                        else
                        {
                            request.RateTrackerOpportunity.RateAlertOriginatingSystem = SourceContact.Crm;
                        }

                        if (request.RateTrackerOpportunity.RateAlertOriginatingSystem == SourceContact.Crm)
                        {
                            if (request.RateTrackerOpportunity.BaseIsSell)
                            {
                                if (buyAmount.HasValue && buyAmount.Value != default(decimal))
                                {
                                    request.RateTrackerOpportunity.Size = buyAmount.Value;
                                    request.RateTrackerOpportunity.DealtIsBase = false;
                                }
                                else request.RateTrackerOpportunity.Size = 0;
                                
                                if (sellAmount.HasValue && sellAmount.Value != default(decimal))
                                {
                                    request.RateTrackerOpportunity.Size = sellAmount.Value;
                                    request.RateTrackerOpportunity.DealtIsBase = true;
                                }
                                else request.RateTrackerOpportunity.Size = 0;
                            }
                            else
                            {
                                if (buyAmount.HasValue && buyAmount.Value != default(decimal))
                                {
                                    request.RateTrackerOpportunity.Size = buyAmount.Value;
                                    request.RateTrackerOpportunity.DealtIsBase = true;
                                }
                                else request.RateTrackerOpportunity.Size = 0;

                                if (sellAmount.HasValue && sellAmount.Value != default(decimal))
                                {
                                    request.RateTrackerOpportunity.Size = sellAmount.Value;
                                    request.RateTrackerOpportunity.DealtIsBase = false;
                                }
                                else request.RateTrackerOpportunity.Size = 0;
                            }
                        }
                        else
                        {
                            if ((request.RateTrackerOpportunity.BaseIsSell && (dealtBase.HasValue && dealtBase.Value == true)) || (!request.RateTrackerOpportunity.BaseIsSell && (dealtBase.HasValue && dealtBase.Value == false)))
                                request.RateTrackerOpportunity.Size = sellAmount.HasValue ? sellAmount.Value : 0;
                            else
                                request.RateTrackerOpportunity.Size = buyAmount.HasValue ? buyAmount.Value : 0;
                        }

                        DateTime? lockTime = AttributeHelper.GetDateTimeAttributeValue(contextEntity, OptyConstants.mcorp_locktime);
                        if (lockTime.HasValue && lockTime != DateTime.MinValue) request.RateTrackerOpportunity.SubscriptionExpiry = lockTime.Value.ToLocalTime();
                        else request.RateTrackerOpportunity.SubscriptionExpiry = null;

                        request.RateTrackerOpportunity.Email = AttributeHelper.GetStringAttributeValue(contextEntity, OptyConstants.mcorp_emailnotificationcontact);
                        request.RateTrackerOpportunity.MobileNumber = AttributeHelper.GetStringAttributeValue(contextEntity, OptyConstants.mcorp_mobilephone);
                        string mobCountryCode = EntityCommon.GetEntityAttributeValue(contextEntity, OptyConstants.mcorp_countrycodemobid, OptyConstants.mcorp_countrycode, svcClient, OptyConstants.mcorp_countrycodealpha2);

                        if (!string.IsNullOrEmpty(mobCountryCode))
                        {
                            request.RateTrackerOpportunity.MobileCountryName = mobCountryCode;
                        }

                        ApplicationSource appSource = ApplicationSource.Moneycorp;

                        Guid? accountId = AttributeHelper.GetGuidFromEntityReference(contextEntity, OptyConstants.customerid);
                        if ((accountId.HasValue))
                        {
                            Entity potentialCustomer = svcClient.Retrieve(AccountConstants.Account, accountId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Accountnumber, AccountConstants.Mcorp_clienttype, AccountConstants.Mcorp_owningbusiness));
                            if (!string.IsNullOrEmpty(potentialCustomer.LogicalName))
                            {
                                int? owningBusiness = AttributeHelper.GetOptionSetValue(potentialCustomer, OptyConstants.mcorp_owningbusiness);
                                int? clientype = AttributeHelper.GetOptionSetValue(potentialCustomer, OptyConstants.mcorp_clienttype);
                                if (clientype.HasValue && clientype.Value != 0) request.RateTrackerOpportunity.ClientType = (ClientType)clientype.Value;
                                request.RateTrackerOpportunity.ClientCRMGuid = potentialCustomer.Id;
                                if (owningBusiness.HasValue)
                                    appSource = EnumHelper.ConvertOwningBusinessToApplicationSourceEnum(owningBusiness.Value);
                            }
                        }
                        request.RateTrackerOpportunity.ApplicationSource = appSource;
                        Guid? notifyContactId = AttributeHelper.GetGuidFromEntityReference(contextEntity, OptyConstants.mcorp_notificationcontactid);

                        if (notifyContactId.HasValue && notifyContactId != default(Guid))
                        {
                            Entity contact = svcClient.Retrieve(OptyConstants.contact, notifyContactId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(ContactConstants.Firstname, ContactConstants.Lastname));
                            if (!string.IsNullOrEmpty(contact.LogicalName))
                            {
                                request.RateTrackerOpportunity.ContactFirstName = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Firstname);
                                request.RateTrackerOpportunity.ContactLastName = AttributeHelper.GetStringAttributeValue(contact, ContactConstants.Lastname);
                            }

                            request.RateTrackerOpportunity.ContactCRMGuid = notifyContactId.Value;
                        }

                        int? marketWatch = AttributeHelper.GetOptionSetValue(contextEntity, OptyConstants.mcorp_marketwatchnotificationmethod);
                        if (marketWatch.HasValue && (marketWatch.Value == (int)MarketWatchNotificationMethod.EmailandSMS || marketWatch.Value == (int)MarketWatchNotificationMethod.SMS))
                            request.RateTrackerOpportunity.IsSendViaSMS = true;
                        else
                            request.RateTrackerOpportunity.IsSendViaSMS = false;

                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        errMsg = "Failed to GetRequest for RateTrackerOpportunity:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromException(ex);
                        _logger.LogError(ex, errMsg);
                    }
                    catch (Exception ex)
                    {
                        errMsg = "Failed to GetRequest for RateTrackerOpportunity:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromException(ex);
                        _logger.LogError(ex, errMsg);
                    }
                }
                else
                {
                    errMsg = "Opportunity is not valid to be sent to OMNI";
                }
            }
            else
            {
                errMsg = "Context entity name does not match.";
            }

            return request;
        }

        /// <summary>
        /// Remove CHECKALERT on 
        /// existing opportunity
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse ProcessRateTrackerDeletion(DeleteRateTrackerOpportunityInfo omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateRateTracker> logger)
        {
            logger.LogInformation("ProcessRateTrackerDeletion start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new Entity();

            try
            {
                entity = svcClient.Retrieve(OptyConstants.opportunity, omniRequest.OpportunityId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "ProcessRateTrackerDeletion-Unable to find oppootunity with ID:" + omniRequest.OpportunityId);
                response.ErrorMessage = "ProcessRateTrackerDeletion-Unable to find oppootunity with ID:" + omniRequest.OpportunityId;
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    bool checkAlert = AttributeHelper.GetBooleanValueDefaultTrue(entity, OptyConstants.mcorp_checkalerts);

                    if (checkAlert)
                    {
                        Entity toUpdate = new Entity(OptyConstants.opportunity, omniRequest.OpportunityId);
                        toUpdate[OptyConstants.mcorp_checkalerts] = false;
                        toUpdate[OptyConstants.mcorp_limitratealertsubscriptionid] = null;
                        toUpdate[OptyConstants.mcorp_stopratealertsubscriptionid] = null;

                        svcClient.Update(toUpdate);
                    }

                    response.IsSuccess = true;
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    logger.LogError(ex, "ProcessRateTrackerDeletion-Failed to update opportunity:" + ex.Message);
                    response.ErrorMessage = "ProcessRateTrackerDeletion-Failed to update opportunity:" + ex.Message;
                    response.Exception = ex;
                }
            }

            logger.LogInformation("ProcessRateTrackerDeletion complete");

            return response;
        }

        /// <summary>
        /// Updates existing opportunity
        /// with Raise Alert to TRUE
        /// </summary>
        /// <param name="omniRequest"></param>
        /// <param name="requestId"></param>
        /// <param name="svcClient"></param>
        /// <param name="isProd"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public DynTaskResponse ProcessRateTrackerNotification(RateTrackerNotification omniRequest, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger)
        {
            logger.LogInformation("ProcessRateTrackerNotification start for requestid:" + requestId);

            DynTaskResponse response = new();
            response.IsSuccess = false;
            response.RecordId = null;
            bool raiseAlert = false;
            Entity entity = new();

            try
            {
                entity = svcClient.Retrieve(OptyConstants.opportunity, omniRequest.OpportunityId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ProcessRateTrackerNotification-Unable to find opportunity with ID:" + omniRequest.OpportunityId + " with error-" + EntityCommon.GetErrorMessageFromFaultException(ex);
                response.Exception = ex;
                logger.LogError(ex, response.ErrorMessage);
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "ProcessRateTrackerNotification-Unable to find opportunity with ID:" + omniRequest.OpportunityId + " with error-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
                logger.LogError(ex, response.ErrorMessage);
            }

            DynTaskResponse emailResp = new();

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                try
                {
                    raiseAlert = AttributeHelper.GetBooleanValueDefaultFalse(entity, OptyConstants.mcorp_raisealert);

                    if (!raiseAlert)
                    {
                        Entity toUpdate = new Entity(OptyConstants.opportunity, omniRequest.OpportunityId);
                        toUpdate[OptyConstants.mcorp_checkalerts] = false;
                        toUpdate[OptyConstants.mcorp_raisealert] = true;
                        toUpdate[OptyConstants.mcorp_limitratealertsubscriptionid] = null;
                        toUpdate[OptyConstants.mcorp_stopratealertsubscriptionid] = null;
                        svcClient.Update(toUpdate);

                        response.IsSuccess = true;
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    response.ErrorMessage = "ProcessRateTrackerNotification-Update opportunity Failed with ID:" + omniRequest.OpportunityId + " with error-" + EntityCommon.GetErrorMessageFromFaultException(ex);
                    response.Exception = ex;
                    logger.LogError(ex, response.ErrorMessage);
                }
                catch (Exception ex)
                {
                    response.ErrorMessage = "ProcessRateTrackerNotification-Update opportunity Failed with ID:" + omniRequest.OpportunityId + " with error-" + EntityCommon.GetErrorMessageFromException(ex);
                    response.Exception = ex;
                    logger.LogError(ex, response.ErrorMessage);
                }

                if (!raiseAlert && response.IsSuccess)
                {
                    // Send Notication Email to Customer
                    response = EmailCommon.ProcessRateTrackerNotificationEmail(svcClient, isProd, logger, entity);

                    // Send notification to AE and DEALER on related account
                    response = EmailCommon.ProcessRateTrackerNotificationAEEmail(svcClient, isProd, logger, entity);
                }
            }

            logger.LogInformation("ProcessRateTrackerNotification complete for requestid:" + requestId);

            return response;
        }

        /// <summary>
        /// Process Rate Tracker Opty from OMNI.
        /// </summary>
        /// <param name="optyId">optyId</param>
        /// <param name="contract">contract</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="isProd">isProd</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessRateTrackerOptyInfo(Guid? optyId, D365RateTrackerOpportunityInfo contract, Guid requestId, IOrganizationServiceAsync2 svcClient, bool isProd, ILogger<FnCorporateRateTracker> logger)
        {
            DynTaskResponse response = CreateUpdateRateTrackerOptyInfo(optyId, contract, requestId, svcClient, logger);

            if (response.IsSuccess && response.Entity != null && response.RecordId.HasValue)
            {
                Entity optyEntity = svcClient.Retrieve(response.Entity.LogicalName, response.RecordId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

                if (contract.ContactCRMGuid.HasValue && response.Entity != null)
                {
                    // this is the notification contact id
                    string templateContent = TemplateHelper.GetRateTrackerSetupTemplate();
                    response = EmailCommon.SendOpportunityEmail(contract.ContactCRMGuid.Value, templateContent, "Rate Tracker Alert", optyEntity, svcClient, isProd, logger);

                    // create phone call activity as well
                    response = CreatePhoneCall(optyEntity, svcClient, response,logger);
                }
            }

            return response;
        }

        /// <summary>
        /// CreatePhoneCall.
        /// </summary>
        /// <param name="regardingEntity"></param>
        /// <param name="svcClient"></param>
        /// <param name="template"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        private static DynTaskResponse CreatePhoneCall(Entity regardingEntity, IOrganizationServiceAsync2 svcClient, DynTaskResponse template, ILogger<FnCorporateRateTracker> logger)
        {
            DynTaskResponse response = new() { IsSuccess = false, RecordId = null };

            try
            {
                Entity activity = new Entity(ActivityConstants.Phonecall);
                activity[ActivityConstants.Description] = "";
                activity[ActivityConstants.Subject] = "New Rate Tracker: " + template.SellCurrency + "-" + template.BuyCurrency + ", Target Rate: " + template.TargetRate;
                activity[ActivityConstants.Regardingobjectid] = new EntityReference(regardingEntity.LogicalName, regardingEntity.Id);
                activity[ActivityConstants.Scheduledend] = regardingEntity.GetAttributeValue<DateTime>("createdon");
                activity[ActivityConstants.Prioritycode] = new OptionSetValue((int)task_prioritycode.Normal);
                activity[ActivityConstants.Ownerid] = regardingEntity[ActivityConstants.Ownerid];
                activity[ActivityConstants.Directioncode] = true;
                EntityReference regOwnerRef = regardingEntity.GetAttributeValue<EntityReference>(ActivityConstants.Ownerid);
                Entity party1 = new(ActivityConstants.Activityparty);
                party1[ActivityConstants.Partyid] = regardingEntity.ToEntityReference();
                EntityCollection to = new EntityCollection();
                to.Entities.Add(party1);
                activity[ActivityConstants.To] = to;
                Entity party2 = new(ActivityConstants.Activityparty);
                party2[ActivityConstants.Partyid] = regOwnerRef;
                EntityCollection from = new();
                from.Entities.Add(party2);
                activity[ActivityConstants.From] = to;

                response.RecordId = svcClient.Create(activity);
                response.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "CreatePhoneCall:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "CreatePhoneCall:" + EntityCommon.GetErrorMessageFromException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// CreateUpdateRateTrackerOptyInfo.
        /// </summary>
        /// <param name="optyId">optyId</param>
        /// <param name="contract">contract</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse CreateUpdateRateTrackerOptyInfo(Guid? optyId, D365RateTrackerOpportunityInfo contract, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateRateTracker> logger)
        {
            logger.LogInformation("ProcessRateTrackerOptyInfo started for request Id:" + requestId.ToString());
            DynTaskResponse response = new() { IsSuccess = false, RecordId = null };

            try
            {
                CurrencyInfo curBase = new CurrencyInfo();
                CurrencyInfo curTerms = new CurrencyInfo();

                Guid? currencyBaseId = null;
                Guid? currencyTermsId = null;
                Guid? countryCodeId = null;

                if (!string.IsNullOrEmpty(contract.BaseCCY))
                {
                    curBase = EntityCommon.RetrieveCurrencyIdByCode(contract.BaseCCY.Trim(), svcClient);
                    currencyBaseId = curBase.CurrencyId;
                }

                if (!string.IsNullOrEmpty(contract.TermsCCY))
                {
                    curTerms = EntityCommon.RetrieveCurrencyIdByCode(contract.TermsCCY.Trim(), svcClient);
                    currencyTermsId = curTerms.CurrencyId;
                }

                if (!string.IsNullOrEmpty(contract.MobileCountryName))
                    countryCodeId = EntityCommon.RetrieveCountryCodeByAlpha2Code(contract.MobileCountryName, svcClient);

                Entity opportunity = new Entity(OptyConstants.opportunity);

                opportunity[OptyConstants.name] = OptyConstants.rateTrackerSubject;
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.RateTracker);
                opportunity[OptyConstants.mcorp_checkalerts] = true;

                if (contract.ClientCRMGuid.HasValue)
                {
                    Entity account = svcClient.Retrieve(OptyConstants.account, contract.ClientCRMGuid.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                    opportunity[OptyConstants.customerid] = account.ToEntityReference();
                    opportunity[OptyConstants.ownerid] = account[OptyConstants.ownerid];
                    opportunity[OptyConstants.mcorp_owningbusiness] = account[OptyConstants.mcorp_owningbusiness];
                    opportunity[OptyConstants.mcorp_clienttype] = account[OptyConstants.mcorp_clienttype];
                }

                if (contract.ContactCRMGuid.HasValue)
                {
                    opportunity[OptyConstants.mcorp_contactid] = new EntityReference(OptyConstants.contact, contract.ContactCRMGuid.Value);
                    opportunity[OptyConstants.mcorp_notificationcontactid] = new EntityReference(OptyConstants.contact, contract.ContactCRMGuid.Value);
                }

                opportunity[OptyConstants.mcorp_dealtisbase] = contract.DealtIsBase;

                if (contract.BaseIsSell)
                {
                    opportunity[OptyConstants.mcorp_basecurrency] = new OptionSetValue((int)BaseCurrency.Sell);
                }
                else
                {
                    opportunity[OptyConstants.mcorp_basecurrency] = new OptionSetValue((int)BaseCurrency.Buy);
                }

                if (contract.AlertType == AlertType.Above)
                {
                    if (contract.BaseIsSell)
                    {
                        opportunity[OptyConstants.mcorp_stoporlimit] = new OptionSetValue((int)StopOrLimit.Limit);

                        if (contract.LimitRateAlertSubscriptionId != default)
                            opportunity[OptyConstants.mcorp_limitratealertsubscriptionid] = contract.LimitRateAlertSubscriptionId;
                        if (contract.LimitRateLimitedRequested != default)
                            opportunity[OptyConstants.mcorp_limitrate] = Convert.ToDouble(contract.LimitRateLimitedRequested);
                        if (contract.LimitInterbankRate != default)
                            opportunity[OptyConstants.mcorp_bankrate] = Convert.ToDouble(contract.LimitInterbankRate);
                    }
                    else
                    {
                        opportunity[OptyConstants.mcorp_stoporlimit] = new OptionSetValue((int)StopOrLimit.Stop);

                        if (contract.LimitRateAlertSubscriptionId != default)
                            opportunity[OptyConstants.mcorp_stopratealertsubscriptionid] = contract.LimitRateAlertSubscriptionId;
                        if (contract.LimitRateLimitedRequested != default)
                            opportunity[OptyConstants.mcorp_stoprate] = Convert.ToDouble(contract.LimitRateLimitedRequested);
                        if (contract.LimitInterbankRate != default)
                            opportunity[OptyConstants.mcorp_banksidestoprate] = Convert.ToDouble(contract.LimitInterbankRate);
                    }
                }
                else
                {
                    if (contract.BaseIsSell)
                    {
                        opportunity[OptyConstants.mcorp_stoporlimit] = new OptionSetValue((int)StopOrLimit.Stop);

                        if(contract.StopRateAlertSubscriptionId != default(int))
                        opportunity[OptyConstants.mcorp_stopratealertsubscriptionid] = contract.StopRateAlertSubscriptionId;

                        if (contract.StopRateLimitedRequested != default(decimal))
                        {
                            opportunity[OptyConstants.mcorp_stoprate] = Convert.ToDouble(contract.StopRateLimitedRequested);
                        }

                        if (contract.StopInterbankRate != default(decimal))
                        {
                            opportunity[OptyConstants.mcorp_banksidestoprate] = Convert.ToDouble(contract.StopInterbankRate);
                        }
                    }
                    else
                    {
                        opportunity[OptyConstants.mcorp_stoporlimit] = new OptionSetValue((int)StopOrLimit.Limit);

                        if(contract.StopRateAlertSubscriptionId != default(int))
                        opportunity[OptyConstants.mcorp_limitratealertsubscriptionid] = contract.StopRateAlertSubscriptionId;

                        if (contract.StopRateAlertSubscriptionId != default(decimal))
                        {
                            opportunity[OptyConstants.mcorp_limitrate] = Convert.ToDouble(contract.StopRateLimitedRequested);
                        }

                        if (contract.StopInterbankRate != default(decimal))
                        {
                            opportunity[OptyConstants.mcorp_bankrate] = Convert.ToDouble(contract.StopInterbankRate);
                        }
                    }
                }

                if (contract.BaseIsSell)
                {
                    if (currencyBaseId.HasValue)
                    {
                        opportunity[OptyConstants.mcorp_opportunitysellcurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyBaseId.Value);
                    }

                    if (currencyTermsId.HasValue)
                    {
                        opportunity[OptyConstants.mcorp_opportunitybuycurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyTermsId.Value);
                    }
                }
                else
                {
                    if (currencyBaseId.HasValue)
                    {
                        opportunity[OptyConstants.mcorp_opportunitybuycurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyBaseId.Value);
                    }

                    if (currencyTermsId.HasValue)
                    {
                        opportunity[OptyConstants.mcorp_opportunitysellcurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyTermsId.Value);
                    }
                }

                if (contract.Size != default)
                {
                    if (contract.BaseIsSell && contract.DealtIsBase || !contract.BaseIsSell && !contract.DealtIsBase)
                        opportunity[OptyConstants.mcorp_sellamount] = Convert.ToDouble(contract.Size);
                    else
                        opportunity[OptyConstants.mcorp_buyamount] = Convert.ToDouble(contract.Size);
                }

                if (contract.SubscriptionExpiry != null && contract.SubscriptionExpiry.HasValue && contract.SubscriptionExpiry != default(DateTime))
                    opportunity[OptyConstants.mcorp_locktime] = contract.SubscriptionExpiry.Value.ToUniversalTime();

                if (contract.RateAlertOriginatingSystem == SourceContact.Crm)
                    opportunity[OptyConstants.mcorp_opportunitysource] = new OptionSetValue((int)OpportunitySource.CRM);
                else
                    opportunity[OptyConstants.mcorp_opportunitysource] = new OptionSetValue((int)OpportunitySource.MCOL);

                opportunity[OptyConstants.mcorp_emailnotificationcontact] = contract.Email;
                opportunity[OptyConstants.mcorp_mobilephone] = contract.MobileNumber;

                if (countryCodeId.HasValue)
                    opportunity[OptyConstants.mcorp_countrycodemobid] = new EntityReference(OptyConstants.mcorp_countrycode, countryCodeId.Value);

                ///Update how contact the customer check if in the contract is present email or SMS
                if (!string.IsNullOrEmpty(contract.Email) && string.IsNullOrEmpty(contract.MobileNumber))
                    opportunity[OptyConstants.mcorp_marketwatchnotificationmethod] = new OptionSetValue((int)MarketWatchNotificationMethod.Email);
                else if (!string.IsNullOrEmpty(contract.Email) && !string.IsNullOrEmpty(contract.MobileNumber))
                    opportunity[OptyConstants.mcorp_marketwatchnotificationmethod] = new OptionSetValue((int)MarketWatchNotificationMethod.EmailandSMS);
                else if (string.IsNullOrEmpty(contract.Email) && !string.IsNullOrEmpty(contract.MobileNumber))
                    opportunity[OptyConstants.mcorp_marketwatchnotificationmethod] = new OptionSetValue((int)MarketWatchNotificationMethod.SMS);

                if (contract.OpportunityId.HasValue)
                {
                    response.RecordId = contract.OpportunityId.Value;
                    opportunity.Id = contract.OpportunityId.Value;
                    svcClient.Update(opportunity);
                }
                else
                {
                    // this is set from Gateway Request for logging. Do not change
                    if (optyId.HasValue)
                    {
                        opportunity.Id = optyId.Value;
                    }

                    // Add D354 Form Defaults
                    opportunity[OptyConstants.mcorp_expectedtradefrequency] = new OptionSetValue(6); // Other
                    opportunity[OptyConstants.mcorp_averagetransaction] = decimal.Parse("1");
                    opportunity[OptyConstants.mcorp_cashintransit] = new OptionSetValue(100000003); //self shipping

                    response.RecordId = svcClient.Create(opportunity);
                }

                response.Entity = opportunity;
                response.Entity.Id = response.RecordId.Value;
                response.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ProcessRateTrackerOptyInfo-Failed to create/update opportunity:" + EntityCommon.GetErrorMessageFromFaultException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "ProcessRateTrackerOptyInfo-Failed to create/update opportunity:" + EntityCommon.GetErrorMessageFromException(ex);
                logger.LogError(ex, response.ErrorMessage);
                response.Exception = ex;
            }

            logger.LogInformation("ProcessRateTrackerOptyInfo completed for request Id:" + requestId.ToString());

            return response;
        }
    }
}
