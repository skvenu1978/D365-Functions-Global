// <copyright file="CrmActivity.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Requests
{
    using McGlobalAzureFunctions.Const.Enums;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Crm activity Object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CrmActivity
    {
        /// <summary>
        /// Activity Subject
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Activity Description
        /// </summary>
        public string? BodyDetails { get; set; }

        /// <summary>
        /// Activity Source
        /// </summary>
        public CrmEntitySource Source { get; set; }

        /// <summary>
        /// Activity Type
        /// </summary>
        public CrmActivityType ActivityType { get; set; }

        /// <summary>
        /// Activity State
        /// </summary>
        public CrmActivityState ActivityState { get; set; }

        /// <summary>
        /// Activity ObjectId
        /// </summary>
        public Guid? ObjectId { get; set; }

        /// <summary>
        /// Activity DueDate
        /// </summary>
        public DateTime DueDate { get; set; }
    }
}