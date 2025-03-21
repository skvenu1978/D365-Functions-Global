// <copyright file="SpCredentials.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Object to hold
    /// Sp Credentials
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SpCredentials
    {
        /// <summary>
        /// Tenant Id for ORG
        /// </summary>
        public string? TenantId { get; set; }

        /// <summary>
        /// Azure App Client ID
        /// </summary>
        public string? ClientId { get; set; }

        /// <summary>
        /// Azure App ClientSecret
        /// </summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Sp Site Name
        /// </summary>
        public string? SpSiteName { get; set; }
    }
}