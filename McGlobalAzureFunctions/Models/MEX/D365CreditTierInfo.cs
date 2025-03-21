// <copyright file="D365CreditTierInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object to hold Credit Tier Data
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365CreditTierInfo
    {
        /// <summary>
        /// D365 record Id
        /// </summary>
        public Guid CrmCreditTierId { get; set; }

        /// <summary>
        /// Credit Tier Amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Credit Tier CreatedBy
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Credit Tier CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Credit Tier CreditLineId
        /// </summary>
        public int CreditLineId { get; set; }

        /// <summary>
        /// Credit Tier CreditLine Guid
        /// </summary>
        public Guid CrmCreditLineId { get; set; }

        /// <summary>
        /// Credit Tier DepositPercentage
        /// </summary>
        public decimal DepositPercentage { get; set; }

        /// <summary>
        /// Credit Tier ExcessUtilisationOmni
        /// </summary>
        public decimal ExcessUtilisationOmni { get; set; }

        /// <summary>
        /// Credit Tier IsDeleted
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// Credit Tier LineType
        /// </summary>
        public CreditTierType LineType { get; set; }

        /// <summary>
        /// Credit Tier ModifiedBy
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// Credit Tier ModifiedOn
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Credit Tier SplitGps
        /// </summary>
        public decimal SplitGps { get; set; }

        /// <summary>
        /// Credit Tier SplitOmni
        /// </summary>
        public decimal SplitOmni { get; set; }

        /// <summary>
        /// Credit Tier SplitOpTrade
        /// </summary>
        public decimal SplitOpTrade { get; set; }

        /// <summary>
        /// Credit Tier Status
        /// </summary>
        public CreditLineStatus Status { get; set; }

        /// <summary>
        /// Credit Tier Term
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Credit Tier Utilisation
        /// </summary>
        public decimal Utilisation { get; set; }

        /// <summary>
        /// Credit Tier UtilisationGps
        /// </summary>
        public decimal UtilisationGps { get; set; }

        /// <summary>
        /// Credit Tier UtilisationOmni
        /// </summary>
        public decimal UtilisationOmni { get; set; }

        /// <summary>
        /// Credit Tier UtilisationOpTrade
        /// </summary>
        public decimal UtilisationOpTrade { get; set; }
    }
}