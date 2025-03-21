// <copyright file="EmailActivity.cs" company ="TTT Moneycorp Ltd">
// Copyright 2025 moneycorp
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Models.Responses;

    /// <summary>
    /// Object class to hold Email Activity from DocGen.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailActivity
    {
        /// <summary>
        /// Gets or Sets Email Subject.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or Sets Email Body.
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// Gets or Sets Email Attachments.
        /// </summary>
        public List<AttachmentNotification>? Attachments { get; set; }

        /// <summary>
        /// Gets or Sets Email Sender.
        /// </summary>
        public Guid Sender { get; set; }

        /// <summary>
        /// Gets or Sets Email Recipients.
        /// </summary>
        public List<RecipientInfo>? Recipients { get; set; }

        /// <summary>
        /// Gets or Sets Email SendEmail.
        /// </summary>
        public bool SendEmail { get; set; }

        /// <summary>
        /// Gets or Sets Email RequestName.
        /// </summary>
        public string? RequestName { get; set; }

        /// <summary>
        /// Gets or Sets Email Regarding Table Name.
        /// </summary>
        public string? RegardingEntityName { get; set; }

        /// <summary>
        /// Gets or Sets Email Regarding Entity Id.
        /// </summary>
        public Guid? RegardingEntityId { get; set; }

        /// <summary>
        /// Gets or Sets Email DocumentId.
        /// </summary>
        public string? DocumentId { get; set; }

        /// <summary>
        /// Gets or Sets Email Sender Address.
        /// </summary>
        public string? SenderEmail { get; set; }

        /// <summary>
        /// Gets or Sets Email RequestId.
        /// </summary>
        public Guid? RequestId { get; set; }
    }
}
