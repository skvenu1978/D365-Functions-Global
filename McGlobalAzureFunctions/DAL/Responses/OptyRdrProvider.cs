// <copyright file="OptyRdrProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Const.Enums;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.Responses;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;
    using System.Text;

    /// <summary>
    /// Implementation of IOptyRdrProvider.
    /// </summary>
    public class OptyRdrProvider : IOptyRdrProvider
    {
        /// <summary>
        /// ProcessDealSummary function
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns></returns>
        public DynTaskResponse ProcessDealSummary(DealSummary request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger)
        {
            logger.LogInformation("ProcessDealSummary start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new();

            try
            {
                entity = CrudCommon.GetEntityByCondition(OptyConstants.opportunity, svcClient, OptyConstants.mcorp_clientdealid, request.ClientDealId.ToString(), false);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "ProcessDealSummary-Failed to retrieve opty using Client Deal Id:" + request.ClientDealId.ToString());
                response.ErrorMessage = "ProcessDealSummary-Failed to retrieve opty using Client Deal Id:" + request.ClientDealId.ToString();
                response.Exception = ex;
            }

            try
            {
                //update opty
                CreateUpdateDealOpportunity(request, entity, requestId, svcClient, logger);
                response.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                logger.LogError(ex, "Failed to update opty with omni request id:" + requestId);
                response.ErrorMessage = "Failed to create activity for request:" + requestId.ToString();
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update opty with omni request id:" + requestId);
                response.ErrorMessage = "Failed to create activity for request:" + requestId.ToString();
                response.Exception = ex;
            }

            logger.LogInformation("ProcessDealSummary complete");

            return response;
        }

        /// <summary>
        /// CreateUpdateOpportunity
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="previousOpportunity">previousOpportunity</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        public void CreateUpdateDealOpportunity(DealSummary request, Entity previousOpportunity, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger)
        {
            logger.LogInformation("CreateUpdateOpportunity start for request:" + requestId.ToString());

            CurrencyInfo currencyBuy = new CurrencyInfo();
            CurrencyInfo currencySell = new CurrencyInfo();
            Guid systemuserId = Guid.Empty;
            request.SourceOfFunds = request.SourceOfFunds ?? string.Empty;

            if (!string.IsNullOrEmpty(request.OpportunityBuyCurrencyName))
            {
                currencyBuy = EntityCommon.RetrieveCurrencyIdByCode(request.OpportunityBuyCurrencyName, svcClient);
            }

            if (!string.IsNullOrEmpty(request.OpportunitySellCurrencyName))
            {
                currencySell = EntityCommon.RetrieveCurrencyIdByCode(request.OpportunitySellCurrencyName, svcClient);
            }

            Entity opportunity = new Entity(OptyConstants.opportunity);
            opportunity[OptyConstants.customerid] = new EntityReference(OptyConstants.account, request.AccountId);
            Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, request.AccountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Mcorp_owningbusiness,
                AccountConstants.Mcorp_clienttype));

            opportunity[OptyConstants.mcorp_owningbusiness] = parentAccount[OptyConstants.mcorp_owningbusiness];
            opportunity[OptyConstants.mcorp_clienttype] = parentAccount[OptyConstants.mcorp_clienttype];
            opportunity[OptyConstants.mcorp_opportunitymargin] = request.OpportunityMargin;
            opportunity[OptyConstants.mcorp_clientdealid] = request.ClientDealId.ToString();
            opportunity[OptyConstants.mcorp_stoprate] = request.SpotRate;
            opportunity[OptyConstants.mcorp_reasonforenquiry] = new OptionSetValue((int)request.ReasonsForTrade);
            string optySubject = GetOpportunityDealSubject(request, previousOpportunity);
            opportunity[OptyConstants.name] = optySubject;
            opportunity[OptyConstants.mcorp_sourceoffunds] = request.SourceOfFunds;
            opportunity[OptyConstants.mcorp_summarytype] = new OptionSetValue((int)SummaryType.Deal);
            if (!string.IsNullOrEmpty(request.FurtherDetails))
                opportunity[OptyConstants.mcorp_reasonforenquirydetails] = request.FurtherDetails;

            if (request.IsOnline)
            {
                opportunity[OptyConstants.mcorp_source] = new OptionSetValue((int)Source.Online);
            }
            else
            {
                opportunity[OptyConstants.mcorp_source] = new OptionSetValue((int)Source.Phone);
            }

            bool hasBookedError = false;
            Entity bookedEntity = SetBookedByAndOwner(request.BookedBy, svcClient, logger, out hasBookedError);

            if (!hasBookedError)
            {
                opportunity[OptyConstants.mcorp_bookedbyid] = bookedEntity[OptyConstants.mcorp_bookedbyid];
                opportunity[OptyConstants.ownerid] = bookedEntity[OptyConstants.ownerid];
            }

            if (request.ValueDate != DateTime.MinValue) opportunity[OptyConstants.mcorp_valuedate] = request.ValueDate.ToUniversalTime();
            if (request.DealDate != DateTime.MinValue) opportunity[OptyConstants.estimatedclosedate] = request.DealDate.ToUniversalTime();
            if (request.ModifiedDate != DateTime.MinValue) opportunity[OptyConstants.mcorp_oppmodifiedon] = request.ModifiedDate.ToUniversalTime();

            if (request.ContactId != Guid.Empty)
            {
                opportunity[OptyConstants.mcorp_contactid] = new EntityReference(OptyConstants.contact, request.ContactId);
            }

            // Check request type
            if (request.ContractType == default(int))
                opportunity[OptyConstants.mcorp_contracttype] = new OptionSetValue((int)ContractType.Spot);
            else
                opportunity[OptyConstants.mcorp_contracttype] = new OptionSetValue((int)request.ContractType);

            if (request.IsOnline)
            {
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.OnlineTrade);
                if (request.PlanId != default(int))
                    opportunity[OptyConstants.mcorp_clientplanid] = request.PlanId;
            }
            else if (request.PlanId != default(int))
            {
                opportunity[OptyConstants.mcorp_clientplanid] = request.PlanId;
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.RPPRequirement);
            }
            else
            {
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.CallInTrade);
            }

            // Set currency id 
            if (currencyBuy.CurrencyId.HasValue)
            {
                opportunity[OptyConstants.mcorp_opportunitybuycurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyBuy.CurrencyId.Value);
                opportunity[OptyConstants.mcorp_buyamount] = request.BuyAmount;

                if (currencyBuy.CurrencyShortCode == "GBP")
                {
                    opportunity[OptyConstants.mcorp_gbpequivalent] = request.BuyAmount;
                }
            }

            if (currencySell.CurrencyId.HasValue)
            {
                opportunity[OptyConstants.mcorp_opportunitysellcurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencySell.CurrencyId.Value);
                opportunity[OptyConstants.mcorp_sellamount] = request.SellAmount;

                if (currencySell.CurrencyShortCode == "GBP")
                {
                    opportunity[OptyConstants.mcorp_gbpequivalent] = request.SellAmount;
                }
            }

            Guid? createdOpportunityId = null;

            if (!string.IsNullOrEmpty(previousOpportunity.LogicalName))
            {
                opportunity.Id = previousOpportunity.Id;
                createdOpportunityId = previousOpportunity.Id;
                opportunity[OptyConstants.mcorp_amended] = request.IsAmendment;

                svcClient.Update(opportunity);
            }
            else
            {
                opportunity[OptyConstants.mcorp_expectedtradefrequency] = new OptionSetValue(6); // Other
                opportunity[OptyConstants.mcorp_averagetransaction] = decimal.Parse("1");
                opportunity[OptyConstants.mcorp_cashintransit] = new OptionSetValue(100000003); //self shipping

                createdOpportunityId = svcClient.Create(opportunity);
                opportunity.Id = createdOpportunityId.Value;
            }

            if (createdOpportunityId.HasValue)
            {
                logger.LogInformation("Opportunity created with id:" + createdOpportunityId.Value);

                logger.LogInformation("Changing opportunity status start");
                string statusErrMsg = string.Empty;
                ChangeOpportunityStatusByDealStatus(request, opportunity, optySubject, svcClient, out statusErrMsg);

                if (statusErrMsg != string.Empty)
                {
                    logger.LogError(statusErrMsg);
                }
            }

            logger.LogInformation("CreateUpdateOpportunity complete for request:" + requestId.ToString());
        }

        /// <summary>
        /// Plan Summary Function
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessPlanSummary(PlanSummary request, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger)
        {
            logger.LogInformation("ProcessPlanSummary start");

            DynTaskResponse response = new DynTaskResponse();
            response.IsSuccess = false;
            response.RecordId = null;

            Entity entity = new();

            try
            {
                entity = CrudCommon.GetEntityByCondition(OptyConstants.opportunity, svcClient, OptyConstants.mcorp_clientplanid, request.PlanId.ToString(), false);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ProcessPlanSummary-Failed to retrieve opty using Client Deal Id:" + request.PlanId.ToString();
                response.Exception = ex;
                logger.LogError(ex, response.ErrorMessage);
            }

            try
            {
                // create opty
                CreateUpdatePlanOpportunity(request, entity, requestId, svcClient, logger);
                response.IsSuccess = true;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "Failed to create opty with omni Plan id:" + request.PlanId.ToString() + "-" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
                logger.LogError(ex, response.ErrorMessage);
            }

            logger.LogInformation("ProcessPlanSummary complete");

            return response;
        }

        /// <summary>
        /// CreateUpdatePlanOpportunity.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="previousOpportunity">previousOpportunity</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <param name="logger">logger</param>
        private void CreateUpdatePlanOpportunity(PlanSummary request, Entity previousOpportunity, Guid requestId, IOrganizationServiceAsync2 svcClient, ILogger<FnCorporateDealsReader> logger)
        {
            logger.LogInformation("CreateUpdatePlanOpportunity start for request:" + requestId.ToString());

            CurrencyInfo currencyBuy = new();
            CurrencyInfo currencySell = new();
            Guid systemuserId = Guid.Empty;
            request.SourceOfFunds = request.SourceOfFunds ?? string.Empty;

            if (!string.IsNullOrEmpty(request.OpportunityBuyCurrencyName))
            {
                currencyBuy = EntityCommon.RetrieveCurrencyIdByCode(request.OpportunityBuyCurrencyName, svcClient);
            }

            if (!string.IsNullOrEmpty(request.OpportunitySellCurrencyName))
            {
                currencySell = EntityCommon.RetrieveCurrencyIdByCode(request.OpportunitySellCurrencyName, svcClient);
            }

            Entity opportunity = new(OptyConstants.opportunity);
            opportunity[OptyConstants.customerid] = new EntityReference(OptyConstants.account, request.AccountId);
            Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, request.AccountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Mcorp_owningbusiness,
              AccountConstants.Mcorp_clienttype));

            opportunity[OptyConstants.mcorp_owningbusiness] = parentAccount[OptyConstants.mcorp_owningbusiness];
            opportunity[OptyConstants.mcorp_clienttype] = parentAccount[OptyConstants.mcorp_clienttype];
            opportunity[OptyConstants.mcorp_clientplanid] = request.PlanId.ToString();
            opportunity[OptyConstants.mcorp_rppoption] = new OptionSetValue((int)request.RppOption);
            opportunity[OptyConstants.mcorp_opportunitymargin] = request.OpportunityMargin;
            opportunity[OptyConstants.mcorp_spotrate] = request.SpotRate;
            opportunity[OptyConstants.mcorp_contracttype] = new OptionSetValue((int)ContractType.RPP);
            opportunity[OptyConstants.mcorp_reasonforenquiry] = new OptionSetValue((int)request.ReasonsForTrade);
            opportunity[OptyConstants.mcorp_sourceoffunds] = request.SourceOfFunds;
            opportunity[OptyConstants.mcorp_summarytype] = new OptionSetValue((int)SummaryType.Plan);
            string optySubject = GetOpportunityPlanSubject(request, previousOpportunity);
            opportunity[OptyConstants.name] = optySubject;

            if (request.IsOnline)
            {
                opportunity[OptyConstants.mcorp_source] = new OptionSetValue((int)Source.Online);
            }
            else
            {
                opportunity[OptyConstants.mcorp_source] = new OptionSetValue((int)Source.Phone);
            }

            bool hasBookedError = false;
            Entity bookedEntity = SetBookedByAndOwner(request.BookedBy, svcClient, logger, out hasBookedError);

            if (!hasBookedError)
            {
                opportunity[OptyConstants.mcorp_bookedbyid] = bookedEntity[OptyConstants.mcorp_bookedbyid];
                opportunity[OptyConstants.ownerid] = bookedEntity[OptyConstants.ownerid];
            }

            if (request.IsOnline)
            {
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.OnlineTrade);
            }
            else
            {
                opportunity[OptyConstants.mcorp_opportunitytype] = new OptionSetValue((int)OpportunityType.RPPRequirement);
            }

            // Check Datees
            if (request.ModifiedDate != DateTime.MinValue)
            {
                opportunity[OptyConstants.mcorp_oppmodifiedon] = request.ModifiedDate.ToUniversalTime();
            }

            if (request.PlanExpiryDate != DateTime.MinValue)
            {
                opportunity[OptyConstants.mcorp_lastvaluedate] = request.PlanExpiryDate.ToUniversalTime();
            }

            // Set currency id 
            if (currencyBuy.CurrencyId.HasValue)
            {
                opportunity[OptyConstants.mcorp_opportunitybuycurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencyBuy.CurrencyId.Value);
                opportunity[OptyConstants.mcorp_buyamount] = request.BuyAmount;

                if (currencyBuy.CurrencyShortCode == "GBP")
                {
                    opportunity[OptyConstants.mcorp_gbpequivalent] = request.BuyAmount;
                }
            }

            if (currencySell.CurrencyId.HasValue)
            {
                opportunity[OptyConstants.mcorp_opportunitysellcurrencyid] = new EntityReference(OptyConstants.transactioncurrency, currencySell.CurrencyId.Value);
                opportunity[OptyConstants.mcorp_sellamount] = request.SellAmount;

                if (currencySell.CurrencyShortCode == "GBP")
                {
                    opportunity[OptyConstants.mcorp_gbpequivalent] = request.SellAmount;
                }
            }

            Guid? createdOpportunityId = null;

            if (!string.IsNullOrEmpty(previousOpportunity.LogicalName))
            {
                opportunity.Id = previousOpportunity.Id;
                createdOpportunityId = previousOpportunity.Id;
                svcClient.Update(opportunity);
            }
            else
            {
                opportunity[OptyConstants.mcorp_expectedtradefrequency] = new OptionSetValue(6); // Other
                opportunity[OptyConstants.mcorp_averagetransaction] = decimal.Parse("1");
                opportunity[OptyConstants.mcorp_cashintransit] = new OptionSetValue(100000003); //self shipping

                createdOpportunityId = svcClient.Create(opportunity);
                opportunity.Id = createdOpportunityId.Value;
            }

            if (createdOpportunityId.HasValue)
            {
                logger.LogInformation("Opportunity created with id:" + createdOpportunityId.Value);
                logger.LogInformation("Changing opportunity status start");
                string statusErrMsg = string.Empty;
                ChangeOpportunityStatusByPlanStatus(request, opportunity, svcClient, optySubject, out statusErrMsg);

                if (statusErrMsg != string.Empty)
                {
                    logger.LogError(statusErrMsg);
                }

                logger.LogInformation("Changing opportunity status end");
            }

            logger.LogInformation("CreateUpdatePlanOpportunity end");
        }

        private string GetOpportunityDealSubject(DealSummary contract, Entity previousOpportunity)
        {
            int? clientPlanId = null;

            if (!string.IsNullOrEmpty(previousOpportunity.LogicalName))
            {
                clientPlanId = AttributeHelper.GetStringAttributeAsIntegerValue(previousOpportunity, OptyConstants.mcorp_clientdealid);
            }

            StringBuilder subject = new StringBuilder();

            // Add in subject if the opportuntiy is Plan / deal together or just deal
            if (contract.ClientDealId != default(int) && contract.PlanId != default(int))
                subject.Append("Plan Deal: ");
            else if (!string.IsNullOrEmpty(previousOpportunity.LogicalName) && contract.ClientDealId != default(int) && clientPlanId != null)
                subject.Append("Plan Deal: ");
            else
                subject.Append("Deal: ");

            ///Always present buy and sell
            subject.Append("Buy ");
            subject.Append(contract.OpportunityBuyCurrencyName + " " + contract.BuyAmount.ToString() + " ");
            subject.Append("Sell ");
            subject.Append(contract.OpportunitySellCurrencyName + " " + contract.SellAmount.ToString() + " ");

            subject.Append("@ ");
            subject.Append(contract.SpotRate.ToString() + " ");
            subject.Append(string.Format("({0}%)", contract.OpportunityMargin.ToString()));

            if (contract.IsOnline)
                subject.Append(" - Online");

            return subject.ToString();
        }

        private static Entity SetBookedByAndOwner(Guid bookedBy, IOrganizationServiceAsync2 svcClient, ILogger logger, out bool hasError)
        {
            hasError = false;
            bool contactExists = false;
            bool systemUserExists = false;

            Entity contact = new Entity();
            Entity retOportunity = new Entity();

            try
            {
                //bookedby is crm2011 user id
                Guid? d365UserId = EntityCommon.GetD365UserUsingCRM2011Guid(bookedBy, svcClient);

                if (!d365UserId.HasValue)
                {
                    contact = CrudCommon.CheckEntityExistsById(svcClient, OptyConstants.contact, bookedBy, out contactExists);
                }
                else
                {
                    systemUserExists = true;
                }

                Guid executingUser = CrudCommon.GetExecutingUser(svcClient);
                EntityReference executingUserRef = new EntityReference(OptyConstants.systemuser, executingUser);

                if (systemUserExists == false && contactExists == false)
                {
                    retOportunity = new Entity(OptyConstants.opportunity);
                    retOportunity[OptyConstants.mcorp_bookedbyid] = executingUserRef;
                    retOportunity[OptyConstants.ownerid] = executingUserRef;
                }
                else if (systemUserExists)
                {
                    if (d365UserId.HasValue)
                    {
                        retOportunity = new Entity(OptyConstants.opportunity);
                        EntityReference userRef = new EntityReference(OptyConstants.systemuser, d365UserId.Value);
                        retOportunity[OptyConstants.mcorp_bookedbyid] = userRef;
                        retOportunity[OptyConstants.ownerid] = userRef;
                    }
                }
                else if (contact != null)
                {
                    retOportunity = new Entity(OptyConstants.opportunity);
                    // Add contact as lookup
                    retOportunity[OptyConstants.mcorp_bookedbyid] = executingUserRef;
                    retOportunity[OptyConstants.ownerid] = contact[OptyConstants.ownerid];
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                logger.LogError(ex, "Failed to process Set Booked by User and Owner");
            }

            return retOportunity;
        }

        private static string GetOpportunityPlanSubject(PlanSummary contract, Entity previousOpportunity)
        {
            int? clientPlanId = null;

            if (!string.IsNullOrEmpty(previousOpportunity.LogicalName))
            {
                clientPlanId = AttributeHelper.GetStringAttributeAsIntegerValue(previousOpportunity, OptyConstants.mcorp_clientplanid);
            }

            StringBuilder subject = new StringBuilder();

            if (!string.IsNullOrEmpty(previousOpportunity.LogicalName) && contract.PlanId != default(int) && clientPlanId.HasValue)
            {
                subject.Append("Plan Deal: ");
            }
            else
            {
                subject.Append("Plan: ");
            }

            if (!string.IsNullOrEmpty(contract.OpportunityBuyCurrencyName) && !string.IsNullOrEmpty(contract.BuyAmount.ToString()))
            {
                subject.Append("Buy ");
                subject.Append(contract.OpportunityBuyCurrencyName + " " + contract.BuyAmount.ToString() + " ");
            }

            if (!string.IsNullOrEmpty(contract.OpportunitySellCurrencyName) && !string.IsNullOrEmpty(contract.SellAmount.ToString()))
            {
                subject.Append("Sell ");
                subject.Append(contract.OpportunitySellCurrencyName + " " + contract.SellAmount.ToString() + " ");
            }

            subject.Append("@ ");
            subject.Append(contract.SpotRate.ToString() + " ");
            subject.Append(string.Format("({0}%)", contract.OpportunityMargin.ToString()));

            if (contract.IsOnline)
            {
                subject.Append(" - Online");
            }

            return subject.ToString();
        }

        private void ChangeOpportunityStatusByDealStatus(DealSummary contract, Entity opportunity, string subject, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            errMsg = string.Empty;

            try
            {
                int optyState = AttributeHelper.GetStateStatusOptionSetValue(opportunity, OptyConstants.statecode);
                int optyStatus = AttributeHelper.GetStateStatusOptionSetValue(opportunity, OptyConstants.statuscode);

                switch (contract.DealStatus)
                {
                    case DealStatus.Completed:
                    case DealStatus.Confirmed:
                        if (optyState == (int)OpportunityState.Open)
                        {
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Traded, subject, default(decimal), svcClient);
                        }
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Won_Traded)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Traded, subject, default(decimal), svcClient);
                            }
                        }
                        break;
                    case DealStatus.Cancelled:
                    case DealStatus.ClosedOut:
                        ///if opp. is not still closed like lose so i close it
                        if (optyState == (int)OpportunityState.Open)
                        {
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Lost, OpportunityStatus.Lost_CancelledRequirement, subject, default(decimal), svcClient);
                        }
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Lost_CancelledRequirement)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Lost, OpportunityStatus.Lost_CancelledRequirement, subject, default(decimal), svcClient);
                            }
                        }
                        break;
                    default:
                        break;
                    case DealStatus.None:
                    case DealStatus.Created:
                        break;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to change opportunity status:" + EntityCommon.GetErrorMessageFromException(ex);
            }
        }

        private void ChangeOpportunityStatusByPlanStatus(PlanSummary contract, Entity opportunity, IOrganizationServiceAsync2 svcClient, string optySubject, out string errMsg)
        {
            errMsg = string.Empty;

            try
            {
                int optyState = AttributeHelper.GetStateStatusOptionSetValue(opportunity, "statecode");
                int optyStatus = AttributeHelper.GetStateStatusOptionSetValue(opportunity, "statuscode");

                switch (contract.PlanStatus)
                {
                    case PlanStatus.Active:
                        if (optyState == (int)OpportunityState.Open)
                        {
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Active, optySubject, default(decimal), svcClient);
                        }
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Won_Active)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Active, optySubject, default(decimal), svcClient);
                            }
                        }
                        break;
                    case PlanStatus.Inactive:
                        if (optyState == (int)OpportunityState.Open)
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Inactive, optySubject, default(decimal), svcClient);
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Won_Inactive)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Inactive, optySubject, default(decimal), svcClient);
                            }
                        }
                        break;
                    case PlanStatus.Completed:
                        if (optyState == (int)OpportunityState.Open)
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Completed, optySubject, default(decimal), svcClient);
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Won_Completed)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Won, OpportunityStatus.Won_Completed, optySubject, default(decimal), svcClient);
                            }
                        }
                        break;
                    case PlanStatus.Cancelled:
                        ///if opp. is not still closed like lose so i close it
                        if (optyState == (int)OpportunityState.Open)
                            CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Lost, OpportunityStatus.Lost_CancelledRequirement, optySubject, default(decimal), svcClient);
                        else
                        {
                            if (optyStatus != (int)OpportunityStatus.Lost_CancelledRequirement)
                            {
                                CrudCommon.SetState(opportunity.Id, OptyConstants.opportunity, (int)OpportunityState.Open, (int)OpportunityStatus.Open_InProgress, svcClient);
                                CrudCommon.CloseOpportunity(opportunity.Id, opportunity, OpportunityState.Lost, OpportunityStatus.Lost_CancelledRequirement, optySubject, default(decimal), svcClient);
                            }
                        }
                        break;
                    case PlanStatus.Reversed:
                    case PlanStatus.Negotiating:
                    case PlanStatus.Created:
                    case PlanStatus.None:
                        break;
                    default:
                        break;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to change opportunity status:" + EntityCommon.GetErrorMessageFromException(ex);
            }
        }
    }
}