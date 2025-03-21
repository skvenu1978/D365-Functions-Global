// <copyright file="AsbMessageInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class AsbMessageInfo
    {
        public string? MessageType { get; set; }
        public Guid? RecordId { get; set; }
        public Guid RequestId { get; set; }
        public bool CanProcess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
