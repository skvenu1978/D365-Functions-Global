// <copyright file="Response.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Response
    {
        public Guid RequestId { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}