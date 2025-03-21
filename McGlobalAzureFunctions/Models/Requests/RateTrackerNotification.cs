// <copyright file="RateTrackerNotification.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// RateTrackerNotification object.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RateTrackerNotification
    {
        /// <summary>
        /// Gets or Sets OpportunityId
        /// </summary>
        public Guid OpportunityId { get; set; }
    }
}