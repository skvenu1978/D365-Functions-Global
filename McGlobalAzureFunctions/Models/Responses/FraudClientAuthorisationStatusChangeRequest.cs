// <copyright file="FraudClientAuthorisationStatusChangeRequest.cs" company ="TTT Moneycorp Ltd">
// Copyright 2025 moneycorp
// </copyright>

namespace McGlobalAzureFunctions.Models.Responses
{
    using McGlobalAzureFunctions.Const.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// FraudClientAuthorisationStatusChangeRequest object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FraudClientAuthorisationStatusChangeRequest
    {
        /// <summary>
        /// RequestId
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// FedsId
        /// </summary>
        public int FedsId { get; set; }

        /// <summary>
        /// ValidationResults
        /// </summary>
        public List<ValidationResult>? ValidationResults { get; set; }

        /// <summary>
        /// AccountStatus
        /// </summary>
        public McAccountStatus AccountStatus { get; set; }
    }
}
