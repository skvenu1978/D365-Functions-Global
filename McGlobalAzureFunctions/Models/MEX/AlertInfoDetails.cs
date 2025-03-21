// <copyright file="AlertInfoDetails.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AlertInfoDetails.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AlertInfoDetails
    {
        /// <summary>
        /// Gets or sets AlertText.
        /// </summary>
        public string? AlertText { get; set; }

        /// <summary>
        /// Gets or sets CRMAlertGuid.
        /// </summary>
        public Guid CRMAlertGuid { get; set; }

        /// <summary>
        /// Gets or sets CRMId.
        /// </summary>
        public int CRMId { get; set; }

        /// <summary>
        /// Gets or sets ClientAlertType.
        /// </summary>
        public ClientAlertType ClientAlertType { get; set; }

        /// <summary>
        /// Gets or sets CreatedBy.
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets CreatedOn.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets ExpiryDate.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets ModifiedBy.
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets ModifiedOn.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets ClientAlertSubjectType.
        /// </summary>
        public ClientAlertSubjectType ClientAlertSubjectType { get; set; }

        /// <summary>
        /// Gets or sets ClientOmniAlertId.
        /// </summary>
        public int? ClientOmniAlertId { get; set; }
    }
}