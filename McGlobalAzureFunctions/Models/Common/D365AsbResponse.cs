// <copyright file="D365AsbResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// D365AsbResponse.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class D365AsbResponse
    {
        /// <summary>
        /// Gets or Sets RequestName.
        /// </summary>
        public string RequestName { get; set; }

        /// <summary>
        /// Gets or Sets RequestId.
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or Sets IsSuccess.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or Sets ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or Sets ExceptionData.
        /// </summary>
        public Exception ExceptionData { get; set; }
    }
}
