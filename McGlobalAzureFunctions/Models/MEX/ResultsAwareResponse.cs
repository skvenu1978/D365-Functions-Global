// <copyright file="ResultsAwareResponse.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ResultsAwareResponse : Response
    {
        public List<CrmCreateOrUpdateExchangeResults>? Results { get; set; }
    }
}