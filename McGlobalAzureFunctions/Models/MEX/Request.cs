// <copyright file="Request.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Request
    {
        public Guid RequestId { get; set; }
    }
}