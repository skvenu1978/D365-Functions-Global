// <copyright file="CrossCoreConstants.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// CrossCoreConstants
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CrossCoreConstants
    {
        public static readonly string att_firstName = "firstname";
        public static readonly string att_lastname = "lastname";
        public static readonly string att_buildingNumber = "mcorp_street1";
        public static readonly string att_street = "mcorp_street2";
        public static readonly string att_posttown = "mcorp_city";
        public static readonly string att_county = "mcorp_stateprovince";
        public static readonly string att_postal = "mcorp_zippostalcode";
        public static readonly string tenantId = "4fce8e94953e42ffb2cbe9f040dcdc";
    }
}
