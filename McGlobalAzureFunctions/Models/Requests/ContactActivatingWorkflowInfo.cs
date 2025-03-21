// <copyright file="ContactActivatingWorkflowInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ContactActivatingWorkflowInfo
    {
        public int ContactId { get; set; }

        public Guid CRMContactGuid { get; set; }

        public ContactWebAccessStatus ContactWebAccessStatus { get; set; }

        public DateTime? DateActivated { get; set; }

        public DateTime? ActivationEmailSentDate { get; set; }

        public Guid? ActivationEmailFromId { get; set; }
    }
}
