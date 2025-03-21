// <copyright file="AttributeHelper.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// AttributeHelper.
    /// </summary>
    public class AttributeHelper
    {
        /// <summary>
        /// GetOptionSetTextValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string GetOptionSetTextValue(Entity entity, string attributeName)
        {
            string retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                if (entity.FormattedValues != null)
                {
                    retValue = entity.FormattedValues[attributeName];
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetAliasedStringValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string? GetAliasedStringValue(Entity entity, string attributeName)
        {
            string? retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                var objRetValue = entity.GetAttributeValue<AliasedValue>(attributeName).Value;

                if (objRetValue != null)
                {
                    retValue = objRetValue.ToString();
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetAliaseAttributeStringValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string? GetAliaseAttributeStringValue(Entity entity, string attributeName)
        {
            string? retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                var objRetValue = entity.GetAttributeValue<AliasedValue>(attributeName).Value;

                if (objRetValue != null)
                {
                    retValue = objRetValue.ToString();
                    //string? strRetValue = objRetValue.ToString();

                    //if (strRetValue != null)
                    //{
                    //    retValue = strRetValue;
                    //}
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetAliasedOptionSetValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>int</returns>
        public static int? GetAliasedOptionSetValue(Entity entity, string attributeName)
        {
            int? retValue = null;

            if (entity.Contains(attributeName))
            {
                OptionSetValue objRetValue = (OptionSetValue)entity.GetAttributeValue<AliasedValue>(attributeName).Value;

                if (objRetValue != null)
                {
                    retValue = objRetValue.Value;
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetAliasedOptionSetTextValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string? GetAliasedOptionSetTextValue(Entity entity, string attributeName)
        {
            string? retValue = null;

            if (entity.Contains(attributeName))
            {
                object objRetValue = entity.FormattedValues[attributeName];

                if (objRetValue != null)
                {
                    retValue = objRetValue.ToString();
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetGuidValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>Guid?</returns>
        public static Guid? GetGuidValue(Entity entity, string attributeName)
        {
            Guid? retValue = null;

            if (entity.Contains(attributeName))
            {
                if (entity.GetAttributeValue<EntityReference>(attributeName) != null)
                {
                    retValue = entity.GetAttributeValue<EntityReference>(attributeName).Id;
                }
            }

            return retValue;
        }

        /// <summary>
        /// GetStringAttributeValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>string</returns>
        public static string GetStringAttributeValue(Entity entity, string attributeName)
        {
            string retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<string>(attributeName);
            }

            return retValue;
        }

        /// <summary>
        /// GetStringAttributeAsIntegerValue.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>int?</returns>
        public static int? GetStringAttributeAsIntegerValue(Entity entity, string attributeName)
        {
            string retValue = string.Empty;
            int? retValueAsInt = null;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<string>(attributeName);
            }

            bool isInt = int.TryParse(retValue, out int strInt);

            if (isInt)
            {
                retValueAsInt = strInt;
            }

            return retValueAsInt;
        }

        /// <summary>
        /// GetStringAttributeAsGuid.
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="attributeName">attributeName</param>
        /// <returns>Guid?</returns>
        public static Guid? GetStringAttributeAsGuid(Entity entity, string attributeName)
        {
            string retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<string>(attributeName);
            }

            Guid? retValueAsGuid = null;
            bool isGuid = Guid.TryParse(retValue, out Guid gdValue);

            if (isGuid)
            {
                retValueAsGuid = gdValue;
            }

            return retValueAsGuid;
        }

        public static Guid GetStringAttributeAsGuidDefault(Entity entity, string attributeName, Guid defaultGuid)
        {
            Guid retGuid = defaultGuid;

            string retValue = string.Empty;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<string>(attributeName);
            }

            bool isGuid = Guid.TryParse(retValue, out Guid gdValue);

            if (isGuid)
            {
                retGuid = gdValue;
            }

            return retGuid;
        }

        public static bool? GetBooleanValue(Entity entity, string attributeName)
        {
            bool? retValue = null;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = entity.GetAttributeValue<bool>(attributeName);
            }

            return retValue;
        }

        public static DateTime? GetDateTimeAttributeValue(Entity entity, string attributeName)
        {
            DateTime? retValue = null;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<DateTime>(attributeName);
            }

            return retValue;
        }

        public static decimal GetMoneyAttributeDefaultZero(Entity entity, string attributeName)
        {
            decimal retValue = new decimal(0);

            if (entity.Contains(attributeName))
            {
                decimal tempRetValue = entity.GetAttributeValue<Money>(attributeName).Value;
                retValue = decimal.Round(tempRetValue, 2, MidpointRounding.AwayFromZero);
            }

            return retValue;
        }

        public static decimal? GetMoneyAttributeAsDecimal(Entity entity, string attributeName)
        {
            decimal? retValue = null;

            if (!entity.Contains(attributeName))
            {
                return retValue;
            }

            decimal tempRetValue = entity.GetAttributeValue<Money>(attributeName).Value;
            retValue = decimal.Round(tempRetValue, 2, MidpointRounding.AwayFromZero);

            return retValue;
        }

        /// <summary>
        /// Gets the Money Attribute
        /// Value in the form of Integer
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static int? GetMoneyAttributeAsInteger(Entity entity, string attributeName)
        {
            int? retValue = null;

            if (!entity.Contains(attributeName))
            {
                return retValue;
            }

            decimal tempRetValue = entity.GetAttributeValue<Money>(attributeName).Value;
            decimal rounded = decimal.Round(tempRetValue, 0, MidpointRounding.AwayFromZero);
            retValue = int.Parse(rounded.ToString("0"), System.Globalization.NumberStyles.Number);

            return retValue;
        }

        public static DateTime? GetFormattedDateTimeAttributeValue(Entity entity, string attributeName)
        {
            DateTime? retValue = null;

            if (!entity.Contains(attributeName.ToLower()))
            {
                return retValue;
            }

            DateTime crmdt = DateTime.SpecifyKind(entity.GetAttributeValue<DateTime>(attributeName.ToLower()), DateTimeKind.Local);
            retValue = TimeZoneInfo.ConvertTime(crmdt, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")).Date;

            return retValue;
        }

        public static decimal? GetDecimalAttributeValue(Entity entity, string attributeName)
        {
            decimal? retValue = null;

            if (!entity.Contains(attributeName))
            {
                return retValue;
            }

            var objRetValue = entity[attributeName];

            if (objRetValue == null)
            {
                return retValue;
            }

            bool isDecimal = decimal.TryParse(objRetValue.ToString(), out decimal dcRetValue);

            if (isDecimal)
            {
                retValue = dcRetValue;
            }

            return retValue;
        }

        public static decimal GetDecimalAttributeValueDefaultZero(Entity entity, string attributeName)
        {
            decimal retValue = decimal.Zero;

            if (entity.Contains(attributeName))
            {
                decimal tempRetValue = entity.GetAttributeValue<decimal>(attributeName);
                retValue = decimal.Round(tempRetValue, 2, MidpointRounding.AwayFromZero);
            }

            return retValue;
        }

        public static int? GetWholeNumberAttributeValue(Entity entity, string attributeName)
        {
            int? retValue = null;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<int>(attributeName);
            }

            return retValue;
        }

        public static decimal? GetFloatAttributeAsDecimal(Entity entity, string attributeName)
        {
            decimal? retValue = null;

            if (!entity.Contains(attributeName))
            {
                return retValue;
            }

            bool isDouble = entity.TryGetAttributeValue<double>(attributeName, out double defValue);

            if (!isDouble)
            {
                return retValue;
            }

            bool isDecimal = decimal.TryParse(defValue.ToString("F4"), out decimal defDec);

            if (isDecimal)
            {
                retValue = defDec;
            }

            return retValue;
        }

        public static int GetWholeNumberAttributeValueDefaultZero(Entity entity, string attributeName)
        {
            int retValue = 0;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<int>(attributeName);
            }

            return retValue;
        }


        public static Guid? GetGuidFromEntityReference(Entity entity, string attributeName)
        {
            Guid? retValue = null;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<EntityReference>(attributeName).Id;
            }

            return retValue;
        }

        public static EntityReference? GetEntityReference(Entity entity, string attributeName)
        {
            EntityReference? retValue = null;

            if (entity.Contains(attributeName))
            {
                retValue = entity.GetAttributeValue<EntityReference>(attributeName);
            }

            return retValue;
        }

        public static string GetLookupStringFromEntityReference(Entity entity, string attributeName)
        {
            string retValue = string.Empty;

            if (!entity.Contains(attributeName))
            {
                return retValue;
            }

            retValue = entity.GetAttributeValue<EntityReference>(attributeName).Name;

            if (!string.IsNullOrEmpty(retValue))
            {
                return retValue;
            }

            if (entity.FormattedValues.Contains(attributeName))
            {
                retValue = entity.FormattedValues[attributeName];
            }

            return retValue;
        }

        public static string GetStringWholeNumberAttributeValue(Entity entity, string attributeName)
        {
            string strRetValue = string.Empty;

            if (!entity.Contains(attributeName))
            {
                return strRetValue;
            }

            int? retValue = (int?)entity.Attributes[attributeName];
            strRetValue = retValue.Value.ToString();

            return strRetValue;
        }

        public static int? GetOptionSetValue(Entity entity, string attributeName)
        {
            int? retValue = null;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = entity.GetAttributeValue<OptionSetValue>(attributeName).Value;
            }

            return retValue;
        }

        public static int? GetOptionSetValueFromCollection(Entity entity, string attributeName)
        {
            int? retValue = null;

            if (!entity.Contains(attributeName) || entity[attributeName] == null)
            {
                return retValue;
            }

            var coll = entity.GetAttributeValue<OptionSetValueCollection>(attributeName);

            foreach (OptionSetValue item in coll)
            {
                retValue = item.Value;
                break;
            }

            return retValue;
        }

        public static int GetStateStatusOptionSetValue(Entity entity, string attributeName)
        {
            int retValue = 0;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = entity.GetAttributeValue<OptionSetValue>(attributeName).Value;
            }

            return retValue;
        }

        public static bool GetBooleanValueDefaultFalse(Entity entity, string attributeName)
        {
            bool retValue = false;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = entity.GetAttributeValue<bool>(attributeName);
            }

            return retValue;
        }

        public static bool GetInverseBooleanValueDefaultFalse(Entity entity, string attributeName)
        {
            bool retValue = false;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = !entity.GetAttributeValue<bool>(attributeName);
            }

            return retValue;
        }


        public static bool GetBooleanValueDefaultTrue(Entity entity, string attributeName)
        {
            bool retValue = true;

            if (entity.Contains(attributeName) && entity[attributeName] != null)
            {
                retValue = entity.GetAttributeValue<bool>(attributeName);
            }

            return retValue;
        }

        private static string FormatEsbDateTimeValue(DateTime dateIn)
        {
            var dateOut = FormatEsbDateValue(dateIn);
            dateOut += $" {dateIn.Hour:00}:{dateIn.Minute:00}:{dateIn.Second.ToString("00")}";
            return dateOut;
        }

        private static string FormatEsbDateValue(DateTime dateIn)
        {
            var dayOut = dateIn.Day.ToString("00");
            string monthOut = "JANFEBMARAPRMAYJUNJULAUGSEPOCTNOVDEC".Substring((dateIn.Month - 1) * 3, 3);
            string yearOut = dateIn.Year.ToString();
            return $"{dayOut}-{monthOut}-{yearOut}";
        }
    }
}