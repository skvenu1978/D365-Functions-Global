// <copyright file="CrmCreateOrUpdateExchangeResults.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Models.MEX
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// CrmCreateOrUpdateExchangeResults
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CrmCreateOrUpdateExchangeResults
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ExceptionMessages
        /// </summary>
        public List<string> ExceptionMessages { get; set; }

        /// <summary>
        /// CRMGuid
        /// </summary>
        public Guid? CRMGuid { get; set; }
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="exceptionMessages"></param>
        /// <param name="crmGuid"></param>
        public CrmCreateOrUpdateExchangeResults(int id, List<string> exceptionMessages, Guid? crmGuid)
        {
            Id = id;
            ExceptionMessages = exceptionMessages;
            CRMGuid = crmGuid;
        }
    }
}
