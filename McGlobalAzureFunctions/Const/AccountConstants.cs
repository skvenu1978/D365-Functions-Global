// <copyright file="AccountConstants.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Contants store 
    /// for Account table
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class AccountConstants
    {
        /// <summary>
        /// Account entity Name.
        /// </summary>
        public static readonly string Account = "account";

        /// <summary>
        /// Account table field NAME.
        /// </summary>
        public static readonly string Name = "name";

        /// <summary>
        /// Account table field accountid.
        /// </summary>
        public static readonly string Accountid = "accountid";

        /// <summary>
        /// Account table field accountnumber.
        /// </summary>
        public static readonly string Accountnumber = "accountnumber";

        /// <summary>
        /// Account table field statuscode.
        /// </summary>
        public static readonly string Statuscode = "statuscode";

        /// <summary>
        /// Account table field statecode.
        /// </summary>
        public static readonly string Statecode = "statecode";

        /// <summary>
        /// Account table field mcorp_fedsid.
        /// </summary>
        public static readonly string Mcorp_fedsid = "mcorp_fedsid";

        /// <summary>
        /// Account table field mcorp_clienttype.
        /// </summary>
        public static readonly string Mcorp_clienttype = "mcorp_clienttype";

        /// <summary>
        /// Account table field parentaccountid.
        /// </summary>
        public static readonly string Parentaccountid = "parentaccountid";

        /// <summary>
        /// Account table field mcorp_dealerid.
        /// </summary>
        public static readonly string Mcorp_dealerid = "mcorp_dealerid";

        /// <summary>
        /// Account table field Mcorp_accountexecutiveid.
        /// </summary>
        public static readonly string Mcorp_accountexecutiveid = "mcorp_accountexecutiveid";

        /// <summary>
        /// Account table related LOOKUP AE.
        /// </summary>
        public static readonly string Aefullname = "ae.fullname";

        /// <summary>
        /// Account table related field de.fullname.
        /// </summary>
        public static readonly string Defullname = "de.fullname";

        /// <summary>
        /// Account table field mcorp_mifidcategory.
        /// </summary>
        public static readonly string Mcorp_mifidcategory = "mcorp_mifidcategory";

        /// <summary>
        /// Account table field mcorp_owningbusiness.
        /// </summary>
        public static readonly string Mcorp_owningbusiness = "mcorp_owningbusiness";

        /// <summary>
        /// Account table field fax.
        /// </summary>
        public static readonly string Fax = "fax";

        /// <summary>
        /// Account table field emailaddress1.
        /// </summary>
        public static readonly string Emailaddress1 = "emailaddress1";

        /// <summary>
        /// Account table field emailaddress2.
        /// </summary>
        public static readonly string Emailaddress2 = "emailaddress2";

        /// <summary>
        /// Account table field telephone1.
        /// </summary>
        public static readonly string Telephone1 = "telephone1";

        /// <summary>
        /// Account table field telephone2.
        /// </summary>
        public static readonly string Telephone2 = "telephone2";

        /// <summary>
        /// Account table field telephone3.
        /// </summary>
        public static readonly string Telephone3 = "telephone3";

        /// <summary>
        /// Account table field mcorp_country.
        /// </summary>
        public static readonly string Mcorp_country = "mcorp_country";

        /// <summary>
        /// Account table field primarycontactid.
        /// </summary>
        public static readonly string Primarycontactid = "primarycontactid";

        /// <summary>
        /// Account table related.
        /// field pcfullname
        /// </summary>
        public static readonly string Pcfullname = "pc.fullname";

        /// <summary>
        /// Account table field modifiedon.
        /// </summary>
        public static readonly string Modifiedon = "modifiedon";

        /// <summary>
        /// Account table field mcorp_accountopeninguserid.
        /// </summary>
        public static readonly string Mcorp_accountopeninguserid = "mcorp_accountopeninguserid";

        /// <summary>
        /// Account table field preferredcontactmethodcode.
        /// </summary>
        public static readonly string Preferredcontactmethodcode = "preferredcontactmethodcode";

        /// <summary>
        /// Account table field mcorp_omnipaymentreference.
        /// </summary>
        public static readonly string Mcorp_omnipaymentreference = "mcorp_omnipaymentreference";

        /// <summary>
        /// Account table field mcorp_duns.
        /// </summary>
        public static readonly string Mcorp_duns = "mcorp_duns";

        /// <summary>
        /// Account table field mcorp_gpsaccountsetup.
        /// </summary>
        public static readonly string Mcorp_gpsaccountsetup = "mcorp_gpsaccountsetup";

        /// <summary>
        /// Account table field mcorp_gpsclientsignedtcs.
        /// </summary>
        public static readonly string Mcorp_gpsclientsignedtcs = "mcorp_gpsclientsignedtcs";

        /// <summary>
        /// Account table field mcorp_gpsprospect.
        /// </summary>
        public static readonly string Mcorp_gpsprospect = "mcorp_gpsprospect";

        /// <summary>
        /// Account table field accountdonotemail.
        /// </summary>
        public static readonly string Accountdonotemail = "donotemail";

        /// <summary>
        /// Account table field accountdonotpostalmail.
        /// </summary>
        public static readonly string Accountdonotpostalmail = "donotpostalmail";

        /// <summary>
        /// Account table field mcorp_spot.
        /// </summary>
        public static readonly string Mcorp_spot = "mcorp_spot";

        /// <summary>
        /// Account table field mcorp_forward.
        /// </summary>
        public static readonly string Mcorp_forward = "mcorp_forward";

        /// <summary>
        /// Account table field mcorp_debitcard.
        /// </summary>
        public static readonly string Mcorp_debitcard = "mcorp_debitcard";

        /// <summary>
        /// Account table field mcorp_countryofresidenceid.
        /// </summary>
        public static readonly string Mcorp_countryofresidenceid = "mcorp_countryofresidenceid";

        /// <summary>
        /// Account table field mcorp_enableddirectdebit.
        /// </summary>
        public static readonly string Mcorp_enableddirectdebit = "mcorp_enableddirectdebit";

        /// <summary>
        /// Account table field mcorp_creditcard.
        /// </summary>
        public static readonly string Mcorp_creditcard = "mcorp_creditcard";

        /// <summary>
        /// Account table field mcorp_enabledcreditcard.
        /// </summary>
        public static readonly string Mcorp_enabledcreditcard = "mcorp_enabledcreditcard";

        /// <summary>
        /// Account table field mcorp_directdebit.
        /// </summary>
        public static readonly string Mcorp_directdebit = "mcorp_directdebit";

        /// <summary>
        /// Account table field mcorp_enableddebitcard.
        /// </summary>
        public static readonly string Mcorp_enableddebitcard = "mcorp_enableddebitcard";

        /// <summary>
        /// Account table field mcorp_tradelimit.
        /// </summary>
        public static readonly string Mcorp_tradelimit = "mcorp_tradelimit";

        /// <summary>
        /// Account table field mcorp_aggregatelimit.
        /// </summary>
        public static readonly string Mcorp_aggregatelimit = "mcorp_aggregatelimit";

        /// <summary>
        /// Account table field mcorp_spread.
        /// </summary>
        public static readonly string Mcorp_spread = "mcorp_spread";

        /// <summary>
        /// Account table field mcorp_system.
        /// </summary>
        public static readonly string Mcorp_system = "mcorp_system";

        /// <summary>
        /// Account table field mcorp_campaignid.
        /// </summary>
        public static readonly string Mcorp_campaignid = "mcorp_campaignid";

        /// <summary>
        /// Account table field mcorp_primarypartnerreferralid.
        /// </summary>
        public static readonly string Mcorp_primarypartnerreferralid = "mcorp_primarypartnerreferralid";

        /// <summary>
        /// Account table lookup field ownerid.
        /// </summary>
        public static readonly string Ownerid = "ownerid";

        /// <summary>
        /// Account table currency field mcorp_directdebitaggregatelimit.
        /// </summary>
        public static readonly string Mcorp_directdebitaggregatelimit = "mcorp_directdebitaggregatelimit";

        /// <summary>
        /// Account table field mcorp_directdebittype.
        /// </summary>
        public static readonly string Mcorp_directdebittype = "mcorp_directdebittype";

        /// <summary>
        /// Account table field mcorp_expectedpaymentinamountpertransaction.
        /// </summary>
        public static readonly string Mcorp_expectedpaymentinamountpertransaction = "mcorp_expectedpaymentinamountpertransaction";

        /// <summary>
        /// Account table field mcorp_expectedamountpertransaction.
        /// </summary>
        public static readonly string Mcorp_expectedamountpertransaction = "mcorp_expectedamountpertransaction";

        /// <summary>
        /// Account table choice field Mcorp_clientmodel.
        /// </summary>
        public static readonly string Mcorp_clientmodel = "mcorp_clientmodel";

        /// <summary>
        /// Account table choice field Mcorp_kpsendnotifications.
        /// </summary>
        public static readonly string Mcorp_kpsendnotifications = "mcorp_kpsendnotifications";

        /// <summary>
        /// Account table choice field Mcorp_kpnotificationcontactemail.
        /// </summary>
        public static readonly string Mcorp_kpnotificationcontactemail = "mcorp_kpnotificationcontactemail";

        /// <summary>
        /// Account table choice field Mcorp_moneyservicebusiness.
        /// </summary>
        public static readonly string Mcorp_moneyservicebusiness = "mcorp_moneyservicebusiness";

        /// <summary>
        /// Account table choice field Mcorp_financialinstitution.
        /// </summary>
        public static readonly string Mcorp_financialinstitution = "mcorp_financialinstitution";

        /// <summary>
        /// Account table bool field mcorp_highvaluedealer.
        /// </summary>
        public static readonly string Mcorp_highvaluedealer = "mcorp_highvaluedealer";

        /// <summary>
        /// Account table choice field mcorp_clientlanguage
        /// </summary>
        public static readonly string Mcorp_clientlanguage = "mcorp_clientlanguage";

        /// <summary>
        /// Account table bool field mcorp_hasloyaltycard.
        /// </summary>
        public static readonly string Mcorp_hasloyaltycard = "mcorp_hasloyaltycard";

        /// <summary>
        /// Account table choice field mcorp_loyaltycardtype.
        /// </summary>
        public static readonly string Mcorp_loyaltycardtype = "mcorp_loyaltycardtype";

        /// <summary>
        /// Account table string field mcorp_legalentityidentifier.
        /// </summary>
        public static readonly string Mcorp_legalentityidentifier = "mcorp_legalentityidentifier";

        /// <summary>
        /// Account table lookup field mcorp_mfrmaccountid.
        /// </summary>
        public static readonly string Mcorp_mfrmaccountid = "mcorp_mfrmaccountid";

        /// <summary>
        /// Account table lookup field mcorp_tttaccountlink.
        /// </summary>
        public static readonly string Mcorp_tttaccountlink = "mcorp_tttaccountlink";

        /// <summary>
        /// Account table string field mcorp_legacysystemname.
        /// </summary>
        public static readonly string Mcorp_legacysystemname = "mcorp_legacysystemname";

        /// <summary>
        /// Account table string field mcorp_legacysystemid.
        /// </summary>
        public static readonly string Mcorp_legacysystemid = "mcorp_legacysystemid";

        /// <summary>
        /// Account table choice field mcorp_paymentclassification.
        /// </summary>
        public static readonly string Mcorp_paymentclassification = "mcorp_paymentclassification";

        /// <summary>
        /// Account table choice field customertypecode.
        /// </summary>
        public static readonly string Customertypecode = "customertypecode";

        /// <summary>
        /// Account table choice field mcorp_tradingstatus.
        /// </summary>
        public static readonly string Mcorp_tradingstatus = "mcorp_tradingstatus";

        /// <summary>
        /// Account table bool field mcorp_forwardquestionnaire.
        /// </summary>
        public static readonly string Mcorp_forwardquestionnaire = "mcorp_forwardquestionnaire";

        /// <summary>
        /// Account table bool field mcorp_enabledbulkpayments.
        /// </summary>
        public static readonly string Mcorp_enabledbulkpayments = "mcorp_enabledbulkpayments";

        /// <summary>
        /// Account table bool field mcorp_reversewireenabled.
        /// </summary>
        public static readonly string Mcorp_reversewireenabled = "mcorp_reversewireenabled";

        /// <summary>
        /// Account table bool field Mcorp_cloudelementsenabled.
        /// </summary>
        public static readonly string Mcorp_cloudelementsenabled = "mcorp_cloudelementsenabled";

        /// <summary>
        /// Account table bool field mcorp_dealerincomingfund.
        /// </summary>
        public static readonly string Mcorp_dealerincomingfund = "mcorp_dealerincomingfund";

        /// <summary>
        /// Account table bool field Mcorp_prioritycustomer.
        /// </summary>
        public static readonly string Mcorp_prioritycustomer = "mcorp_prioritycustomer";

        /// <summary>
        /// Account table bool field Mcorp_editpaymentdate.
        /// </summary>
        public static readonly string Mcorp_editpaymentdate = "mcorp_editpaymentdate";
    }
}