// <copyright file="OptyConstants.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// OptyConstants.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OptyConstants
    {
        public static readonly string rateTrackerSubject = "New Rate Tracker from MCOL";
        public static readonly string opportunity = "opportunity";
        public static readonly string mcorp_countrycodealpha2 = "mcorp_countrycodealpha2";
        public static readonly string mcorp_countrycode = "mcorp_countrycode";
        public static readonly string contact = "contact";
        public static readonly string account = "account";
        public static readonly string isocurrencycode = "isocurrencycode";
        public static readonly string mcorp_owningbusiness = "mcorp_owningbusiness";
        public static readonly string mcorp_clienttype = "mcorp_clienttype";
        public static readonly string mcorp_fedsid = "mcorp_fedsid";
        public static readonly string statecode = "statecode";
        public static readonly string statuscode = "statuscode";
        public static readonly string systemuser = "systemuser";
        public static readonly string transactioncurrency = "transactioncurrency";
        public static readonly string mcorp_checkalerts = "mcorp_checkalerts";
        public static readonly string mcorp_limitratealertsubscriptionid = "mcorp_limitratealertsubscriptionid";
        public static readonly string mcorp_stopratealertsubscriptionid = "mcorp_stopratealertsubscriptionid";
        public static readonly string mcorp_omniclientdealid = "mcorp_omniclientdealid";
        public static readonly string closeprobability = "closeprobability";
        public static readonly string customerid = "customerid";
        public static readonly string mcorp_opportunitymargin = "mcorp_opportunitymargin";
        public static readonly string mcorp_stoprate = "mcorp_stoprate";
        public static readonly string mcorp_reasonforenquiry = "mcorp_reasonforenquiry";
        public static readonly string mcorp_sourceoffunds = "mcorp_sourceoffunds";
        public static readonly string mcorp_summarytype = "mcorp_summarytype";
        public static readonly string mcorp_reasonforenquirydetails = "mcorp_reasonforenquirydetails";
        public static readonly string mcorp_source = "mcorp_source";
        public static readonly string mcorp_bookedbyid = "mcorp_bookedbyid";
        public static readonly string mcorp_valuedate = "mcorp_valuedate";
        public static readonly string estimatedclosedate = "estimatedclosedate";
        public static readonly string mcorp_oppmodifiedon = "mcorp_oppmodifiedon";
        public static readonly string mcorp_contactid = "mcorp_contactid";
        public static readonly string parentcontactid = "parentcontactid";
        public static readonly string parentaccountid = "parentaccountid";
        public static readonly string mcorp_contracttype = "mcorp_contracttype";
        public static readonly string mcorp_opportunitytype = "mcorp_opportunitytype";
        public static readonly string mcorp_clientplanid = "mcorp_clientplanid";
        public static readonly string mcorp_opportunitybuycurrencyid = "mcorp_opportunitybuycurrencyid";
        public static readonly string mcorp_opportunitysellcurrencyid = "mcorp_opportunitysellcurrencyid";
        public static readonly string mcorp_buyamount = "mcorp_buyamount";
        public static readonly string mcorp_sellamount = "mcorp_sellamount";
        public static readonly string mcorp_gbpequivalent = "mcorp_gbpequivalent";
        public static readonly string mcorp_amended = "mcorp_amended";
        public static readonly string mcorp_rppoption = "mcorp_rppoption";
        public static readonly string mcorp_spotrate = "mcorp_spotrate";
        public static readonly string mcorp_lastvaluedate = "mcorp_lastvaluedate";

        public static readonly string name = "name";//Text, 300 max, Topic display name
        public static readonly string mcorp_raisealert = "mcorp_raisealert";//bool, default false
        public static readonly string mcorp_interestedinemigration = "mcorp_interestedinemigration";//bool, default false
        public static readonly string mcorp_interestedinother = "mcorp_interestedinother";//bool, default false
        public static readonly string mcorp_interestedinpropertypurchaseorsale = "mcorp_interestedinpropertypurchaseorsale"; //bool, default false
        public static readonly string mcorp_interestedinregularpayments = "mcorp_interestedinregularpayments"; //bool, default false
        public static readonly string mcorp_clientcategory = "mcorp_clientcategory"; //choice, default none
        public static readonly string mcorp_buyingwhereid = "mcorp_buyingwhereid";//lookup, Country
        public static readonly string mcorp_buyingregion = "mcorp_buyingregion";//Text, Max 100 char
        public static readonly string mcorp_ttfeestatus = "mcorp_ttfeestatus";//choice, default none
        public static readonly string description = "description";//Multiline, max 2000 char
        public static readonly string mcorp_bankrate = "mcorp_bankrate";//Float Bank Side Limit Rate
        public static readonly string mcorp_banksidestoprate = "mcorp_banksidestoprate";//Float Bank Side Stop Rate
        public static readonly string mcorp_stoporlimit = "mcorp_stoporlimit";//Choice
        public static readonly string mcorp_dealtisbase = "mcorp_dealtisbase";//bool default false
        public static readonly string mcorp_opportunitysource = "mcorp_opportunitysource";//choice, default CRM
        public static readonly string mcorp_emailnotificationcontact = "mcorp_emailnotificationcontact";//Text, Max 100 char
        public static readonly string mcorp_mobilephone = "mcorp_mobilephone";//Text, Max 100 char
        public static readonly string mcorp_countrycodemobid = "mcorp_countrycodemobid";//Lookup, CountryCode
        public static readonly string mcorp_notificationcontactid = "mcorp_notificationcontactid";//Lookup, Contact
        public static readonly string mcorp_marketwatchnotificationmethod = "mcorp_marketwatchnotificationmethod";//Choice, default None
        public static readonly string mcorp_basecurrency = "mcorp_basecurrency";//Choice, default Sell
        public static readonly string mcorp_clientdealid = "mcorp_clientdealid";//Text, Max 100 char
        public static readonly string mcorp_limitrate = "mcorp_limitrate";//Float, precission: 2 decimal places
        public static readonly string mcorp_locktime = "mcorp_locktime";//DateTime, Format: Date and Time
        public static readonly string ownerid = "ownerid";//Lookup, Systemuser

        public static readonly string mcorp_expectedtradefrequency = "mcorp_expectedtradefrequency";
        public static readonly string mcorp_averagetransaction = "mcorp_averagetransaction";
        public static readonly string mcorp_cashintransit = "mcorp_cashintransit";
    }
}