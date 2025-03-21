// <copyright file="DynTaskResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Xrm.Sdk;

    [ExcludeFromCodeCoverage]
    public class DynTaskResponse
    {
        public Guid? RecordId { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
        public string? LogMessage { get; set; }

        public TemplateItem? TemplateData { get; set; }

        public Entity? Entity { get; set; }

        public string? SellCurrency { get; set; }
        public string? BuyCurrency { get; set; }
        public string? TargetRate { get; set; }
    }
}