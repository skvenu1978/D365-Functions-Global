// <copyright file="UserPrivilegesInfo.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class UserPrivilegesInfo
    {
        public bool IsAuthorisePayment { get; set; }
        public bool IsCreateBeneficiary { get; set; }
        public bool IsCreatePayment { get; set; }
        public bool IsCreateDeal { get; set; }
        public int ContactId { get; set; }
        public Guid CRMContactGuid { get; set; }
    }
}