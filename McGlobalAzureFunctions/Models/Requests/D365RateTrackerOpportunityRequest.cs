// <copyright file="D365RateTrackerOpportunityRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// D365RateTrackerOpportunityRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365RateTrackerOpportunityRequest
    {
        /// <summary>
        /// Gets or Sets the Request.
        /// </summary>
        public D365RateTrackerOpportunityInfo? RateTrackerOpportunity { get; set; }
    }
}