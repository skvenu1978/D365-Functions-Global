// <copyright file="DeleteRateTrackerOpportunityRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class DeleteRateTrackerOpportunityRequest
    {
        public Guid OpportunityId { get; set; }
    }
}