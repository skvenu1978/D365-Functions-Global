// <copyright file="LookupReference.cs" company ="TTT Moneycorp Ltd">
// Copyright 2025 moneycorp
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Lookup Details
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LookupReference
    {
        /// <summary>
        /// Lookup Entity Name
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Lookup Entity Id
        /// </summary>
        public Guid? EntityId { get; set; }
    }
}
