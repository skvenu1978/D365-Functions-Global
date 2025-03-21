// <copyright file="FnConstants.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Const
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// FnConstants.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FnConstants
    {
        /// <summary>
        /// D365ConnectionString.
        /// </summary>
        public static readonly string D365ConnectionString = "D365ConnectionString";

        /// <summary>
        /// Produrl.
        /// </summary>
        public static readonly string Produrl = "moneycorp-global-prod";

        /// <summary>
        /// D365tomexpublishing.
        /// </summary>
        public static readonly string D365tomexpublishing = "d365tomexpublishing";

        /// <summary>
        /// MessageType.
        /// </summary>
        public static readonly string MessageType = "MessageType";

        /// <summary>
        /// RequestId.
        /// </summary>
        public static readonly string RequestId = "RequestId";

        /// <summary>
        /// RecordId.
        /// </summary>
        public static readonly string RecordId = "RecordId";

        /// <summary>
        /// StaticUserId.
        /// </summary>
        public static readonly Guid StaticUserId = Guid.Parse("BBA2EE81-E47E-E411-B0CB-005056803ECA");

        /// <summary>
        /// SharePointSiteName.
        /// </summary>
        public static readonly string SharePointSiteName = "SharePointSiteName";

        /// <summary>
        /// SharePointAppClientId.
        /// </summary>
        public static readonly string SharePointAppClientId = "SharePointAppClientId";

        /// <summary>
        /// SharePointAppSecret.
        /// </summary>
        public static readonly string SharePointAppSecret = "SharePointAppSecret";
    }
}