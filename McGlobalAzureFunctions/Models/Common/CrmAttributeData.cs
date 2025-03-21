// <copyright file="AttributeData.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    public class CrmAttributeData
    {
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeText { get; set; }
        public string AttributeLogicalName { get; set; }
        public string HtmlKeyWord { get; set; }
        public string HtmlKeyWordContent { get; set; }
        public bool IsConditional { get; set; }
    }
}
