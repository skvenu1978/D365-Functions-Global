// <copyright file="AlertTypeResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class AlertTypeResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public int? StopOrLimit { get; set; }
        public int? LimitRateAlertSubscriptionId { get; set; }
        public double? LimitRate { get; set; }
        public double? BankRate { get; set; }
        public int? StopRateAlertSubscriptionId { get; set; }
        public double? StopRate { get; set; }
        public double? BanksideStopRate { get; set; }
    }
}