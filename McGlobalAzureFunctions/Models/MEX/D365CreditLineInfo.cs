// <copyright file="D365CreditLineInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Const.Enums;

    /// <summary>
    /// D365CreditLineInfo object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365CreditLineInfo
    {
        /// <summary>
        /// Gets or Set CrmAccountId.
        /// </summary>
        public Guid CrmAccountId { get; set; }

        /// <summary>
        /// Gets or Set CrmCreditLineId.
        /// </summary>
        public Guid CrmCreditLineId { get; set; }

        /// <summary>
        /// Gets or Set DailySettlementLimit.
        /// </summary>
        public decimal? DailySettlementLimit { get; set; }

        /// <summary>
        /// Gets or Set DirectDebitClearanceDays.
        /// </summary>
        public int DirectDebitClearanceDays { get; set; }

        /// <summary>
        /// Gets or Set IsDirectDebit.
        /// </summary>
        public bool IsDirectDebit { get; set; }

        /// <summary>
        /// Gets or Set MaxDirectDebitAmount.
        /// </summary>
        public decimal? MaxDirectDebitAmount { get; set; }

        /// <summary>
        /// Gets or Set Status.
        /// </summary>
        public CreditLineStatus Status { get; set; }

        /// <summary>
        /// Gets or Set ToleranceLevel.
        /// </summary>
        public int? ToleranceLevel { get; set; }
    }
}
