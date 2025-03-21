// <copyright file="ContactInfoDetails.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// ContactInfoDetails object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContactInfoDetails
    {
        /// <summary>
        /// Gets or Sets AuthorisationStatus.
        /// </summary>
        public AuthorisationStatus AuthorisationStatus { get; set; }

        /// <summary>
        /// Gets or Sets AuthorisedForTrade.
        /// </summary>
        public bool AuthorisedForTrade { get; set; }

        /// <summary>
        /// Gets or Sets BusinessEmail.
        /// </summary>
        public string? BusinessEmail { get; set; }

        /// <summary>
        /// Gets or Sets CountryCodeBusinessPhone.
        /// </summary>
        public string? CountryCodeBusinessPhone { get; set; }

        /// <summary>
        /// Gets or Sets BusinessPhone.
        /// </summary>
        public string? BusinessPhone { get; set; }

        /// <summary>
        /// Gets or Sets CRMContactGuid.
        /// </summary>
        public Guid CRMContactGuid { get; set; }

        /// <summary>
        /// Gets or Sets CRMId.
        /// </summary>
        public int CRMId { get; set; }

        /// <summary>
        /// Gets or Sets FirstName.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or Sets HomeEmail.
        /// </summary>
        public string? HomeEmail { get; set; }

        /// <summary>
        /// Gets or Sets CountryCodeHomePhone.
        /// </summary>
        public string? CountryCodeHomePhone { get; set; }

        /// <summary>
        /// Gets or Sets HomePhone.
        /// </summary>
        public string? HomePhone { get; set; }

        /// <summary>
        /// Gets or Sets IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or Sets JobTitle.
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// Gets or Sets LastName.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or Sets MayArrangeTransfer.
        /// </summary>
        public bool MayArrangeTransfer { get; set; }

        /// <summary>
        /// Gets or Sets MayEnquireAboutAccount.
        /// </summary>
        public bool MayEnquireAboutAccount { get; set; }

        /// <summary>
        /// Gets or Sets CountryCodeMobilePhone.
        /// </summary>
        public string? CountryCodeMobilePhone { get; set; }

        /// <summary>
        /// Gets or Sets MobilePhone.
        /// </summary>
        public string? MobilePhone { get; set; }

        /// <summary>
        /// Gets or Sets CountryOfResidence.
        /// </summary>
        public string? CountryOfResidence { get; set; }

        /// <summary>
        /// Gets or Sets ModifiedOn.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or Sets Salutation.
        /// </summary>
        public Salutation Salutation { get; set; }

        /// <summary>
        /// Gets or Sets SendEmail.
        /// </summary>
        public bool SendEmail { get; set; }

        /// <summary>
        /// Gets or Sets SendMail.
        /// </summary>
        public bool SendMail { get; set; }

        /// <summary>
        /// Gets or Sets SendPopEmail.
        /// </summary>
        public bool SendPopEmail { get; set; }

        /// <summary>
        /// Gets or Sets SendSMS.
        /// </summary>
        public bool SendSMS { get; set; }

        /// <summary>
        /// Gets or Sets WebAccessStatus.
        /// </summary>
        public ContactWebAccessStatus WebAccessStatus { get; set; }

        /// <summary>
        /// Gets or Sets CountryName.
        /// </summary>
        public string? CountryName { get; set; }

        /// <summary>
        /// Gets or Sets IsAuthorisePayment.
        /// </summary>
        public bool IsAuthorisePayment { get; set; }

        /// <summary>
        /// Gets or Sets IsMlpaEnabled.
        /// </summary>
        public bool IsMlpaEnabled { get; set; }

        /// <summary>
        /// Gets or Sets IsSuperUser.
        /// </summary>
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// Gets or Sets CanCreateUnlimited.
        /// </summary>
        public bool CanCreateUnlimited { get; set; }

        /// <summary>
        /// Gets or Sets CreateLimit.
        /// </summary>
        public decimal CreateLimit { get; set; }

        /// <summary>
        /// Gets or Sets NumberOfApproversRequired.
        /// </summary>
        public int NumberOfApproversRequired { get; set; }

        /// <summary>
        /// Gets or Sets CanApproveUnlimited.
        /// </summary>
        public bool CanApproveUnlimited { get; set; }

        /// <summary>
        /// Gets or Sets ApproveLimit.
        /// </summary>
        public decimal ApproveLimit { get; set; }

        /// <summary>
        /// Gets or Sets IsSendContractNote.
        /// </summary>
        public bool IsSendContractNote { get; set; }

        /// <summary>
        /// Gets or Sets IsSendInvoice.
        /// </summary>
        public bool IsSendInvoice { get; set; }

        /// <summary>
        /// Gets or Sets AccountOpeningWebUserId.
        /// </summary>
        public Guid? AccountOpeningWebUserId { get; set; }

        /// <summary>
        /// Gets or Sets AccountOpeningStatusReason.
        /// </summary>
        public AccountOpeningStatusReason? AccountOpeningStatusReason { get; set; }

        /// <summary>
        /// Gets or Sets NewRecipientNotification.
        /// </summary>
        public NotificationMethod NewRecipientNotification { get; set; }

        /// <summary>
        /// Gets or Sets IncomingFundsNotification.
        /// </summary>
        public NotificationMethod IncomingFundsNotification { get; set; }

        /// <summary>
        /// Gets or Sets ApplicationSource.
        /// </summary>
        public ApplicationSource ApplicationSource { get; set; }

        /// <summary>
        /// Gets or Sets Postcode.
        /// </summary>
        public string? Postcode { get; set; }

        /// <summary>
        /// Gets or Sets DateOfBirth.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or Sets WebUserPrivilegesGranted.
        /// </summary>
        public List<string>? WebUserPrivilegesGranted { get; set; }

        /// <summary>
        /// Gets or Sets WebUserPrivilegesDenied.
        /// </summary>
        public List<string>? WebUserPrivilegesDenied { get; set; }

        /// <summary>
        /// Gets or Sets HasLoyaltyCard.
        /// </summary>
        public bool HasLoyaltyCard { get; set; }

        /// <summary>
        /// Gets or Sets LoyaltyCardType.
        /// </summary>
        public LoyaltyCardType LoyaltyCardType { get; set; }

        /// <summary>
        /// Gets or Sets ReceivePaymentApprovalEmails.
        /// </summary>
        public bool? ReceivePaymentApprovalEmails { get; set; }

        /// <summary>
        /// Gets or Sets SubscribeToFeaturesTips.
        /// </summary>
        public bool? SubscribeToFeaturesTips { get; set; }

        /// <summary>
        /// Gets or Sets SubscribeToNewsAlerts.
        /// </summary>
        public bool? SubscribeToNewsAlerts { get; set; }

        /// <summary>
        /// Gets or Sets SubscribeToPromotions.
        /// </summary>
        public bool? SubscribeToPromotions { get; set; }

        /// <summary>
        /// Gets or Sets ContactViaEmail.
        /// </summary>
        public bool? ContactViaEmail { get; set; }

        /// <summary>
        /// Gets or Sets ContactViaMail.
        /// </summary>
        public bool? ContactViaMail { get; set; }

        /// <summary>
        /// Gets or Sets ContactViaSms.
        /// </summary>
        public bool? ContactViaSms { get; set; }

        /// <summary>
        /// Gets or Sets ContactViaPhone.
        /// </summary>
        public bool? ContactViaPhone { get; set; }

        /// <summary>
        /// Gets or Sets ContactViaDisplayNotifications.
        /// </summary>
        public bool? ContactViaDisplayNotifications { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck1_Type.
        /// </summary>
        public IdentityType IdentityCheck1_Type { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck2_Type.
        /// </summary>
        public IdentityType IdentityCheck2_Type { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck1_Number.
        /// </summary>
        public string? IdentityCheck1_Number { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck2_Number.
        /// </summary>
        public string? IdentityCheck2_Number { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck1_Status.
        /// </summary>
        public IdentityStatus IdentityCheck1_Status { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck2_Status.
        /// </summary>
        public IdentityStatus IdentityCheck2_Status { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck1_Origin.
        /// </summary>
        public string? IdentityCheck1_Origin { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck2_Origin.
        /// </summary>
        public string? IdentityCheck2_Origin { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck1_ExpiryDate.
        /// </summary>
        public DateTime? IdentityCheck1_ExpiryDate { get; set; }

        /// <summary>
        /// Gets or Sets IdentityCheck2_ExpiryDate.
        /// </summary>
        public DateTime? IdentityCheck2_ExpiryDate { get; set; }

        /// <summary>
        /// Gets or Sets ThirdParty.
        /// </summary>
        public bool? ThirdParty { get; set; }

        /// <summary>
        /// Gets or Sets ThirdpartyEmail.
        /// </summary>
        public bool? ThirdpartyEmail { get; set; }

        /// <summary>
        /// Gets or Sets ThirdpartySms.
        /// </summary>
        public bool? ThirdpartySms { get; set; }

        /// <summary>
        /// Gets or Sets ThirdpartyTelephone.
        /// </summary>
        public bool? ThirdpartyTelephone { get; set; }

        /// <summary>
        /// Gets or Sets ThirdpartyPost.
        /// </summary>
        public bool? ThirdpartyPost { get; set; }
    }
}