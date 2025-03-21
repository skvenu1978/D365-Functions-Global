// <copyright file="TemplateItem.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using Microsoft.Xrm.Sdk;

    public class TemplateItem
    {
        /// <summary>
        /// Gets or sets OwningBusinessName
        /// </summary>
        public string OwningBusiness { get; set; }

        /// <summary>
        /// Gets or sets value of the TemplateSubject.
        /// </summary>
        public string TemplateSubject { get; set; }

        /// <summary>
        /// Gets or sets value of the FromEmail.
        /// </summary>
        public string FromEmail { get; set; }
        
        /// <summary>
        /// Gets or sets value of the ToEmail.
        /// </summary>
        public string ToEmail { get; set; }

        public EntityReference? ToEntityRef { get; set; }

        public EntityReference? FromEntityRef { get; set; }

        /// <summary>
        /// Gets or sets value of the ContentHtml.
        /// </summary>
        public string ContentHtml { get; set; }

        /// <summary>
        /// Gets or sets value of the TemplateBackground.
        /// </summary>
        public string TemplateBackground { get; set; }
    }
}
