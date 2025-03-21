// <copyright file="AlertInfoRequest.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Models.MEX;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// AlertInfoRequest.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AlertInfoRequest
    {
        /// <summary>
        /// Gets or Sets AlertsInfoDetails.
        /// </summary>
        public List<AlertInfoDetails>? AlertsInfoDetails { get; set; }
    }
}