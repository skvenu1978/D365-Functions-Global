// <copyright file="LexNexConstants.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// LexNexConstants.
    /// </summary>
    public class LexNexConstants
    {
        public static readonly string att_firstName = "firstname";
        public static readonly string att_lastname = "lastname";
        public static readonly string att_phone = "mobilephone";
        public static readonly string att_dob = "birthdate";
        public static readonly string att_country = "mcorp_country";
        public static readonly string att_countrycode = "mcorp_iso2code";
        public static readonly string att_buildingNumber = "mcorp_street1";
        public static readonly string att_street = "mcorp_street2";
        public static readonly string att_city = "mcorp_city";
        public static readonly string att_county = "mcorp_stateprovince";
        public static readonly string att_postal = "mcorp_zippostalcode";

        public static readonly string att_nationalId = "mcorp_idnumber1";
        public static readonly string[] referRiskIndicators = { "PO", "11", "12", "14", "40", "50" };
    }
}
