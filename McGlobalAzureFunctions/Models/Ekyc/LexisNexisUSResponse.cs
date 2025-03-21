using Microsoft.Crm.Sdk.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc
{
    public class LexisNexisUSResponse
    {
        public InstantIDResponseEx? InstantIDResponseEx { get; set; }
    }
    public class ComprehensiveVerification
    {
        public int ComprehensiveVerificationIndex { get; set; }
        public RiskIndicators? RiskIndicators { get; set; }
        public PotentialFollowupActions? PotentialFollowupActions { get; set; }
    }

    public class FollowupAction
    {
        public string? RiskCode { get; set; }
        public string? Description { get; set; }
    }
    public class Header
    {
        public int Status { get; set; }
        public string? TransactionId { get; set; }
    }
    public class InstantIDResponseEx
    {

        public Response? response { get; set; }
    }
    public class PotentialFollowupActions
    {
        public List<FollowupAction>? FollowupAction { get; set; }
    }

    public class Response
    {
        public Header? Header { get; set; }
        public Result? Result { get; set; }
    }

    public class Result
    {
        public string? UniqueId { get; set; }
        public bool DOBVerified { get; set; }
        public int NameAddressSSNSummary { get; set; }
        public ComprehensiveVerification? ComprehensiveVerification { get; set; }
        public string? PhoneOfNameAddress { get; set; }
        public string? AdditionalScore1 { get; set; }
        public string? AdditionalScore2 { get; set; }
        public bool PassportValidated { get; set; }
        public int DOBMatchLevel { get; set; }
        public bool SSNFoundForLexID { get; set; }
        public bool AddressPOBox { get; set; }
        public bool AddressCMRA { get; set; }
        public string? InstantIDVersion { get; set; }
        public bool EmergingId { get; set; }
        public bool AddressStandardized { get; set; }
        public WatchLists? WatchLists { get; set; }
        public bool BureauDeleted { get; set; }
        public bool ITINExpired { get; set; }
        public bool IsPhoneCurrent { get; set; }
    }

    public class WatchList
    {
        public string? Table { get; set; }
        public string? RecordNumber { get; set; }
        public string? Country { get; set; }
        public string? EntityName { get; set; }
    }

    public class WatchLists
    {
        public List<WatchList>? WatchList { get; set; }
    }
    public class RiskIndicator
    {
        public string? RiskCode { get; set; }
        public string? Description { get; set; }
        public int Sequence { get; set; }
    }

    public class RiskIndicators
    {
        public List<RiskIndicator>? RiskIndicator { get; set; }
    }
}
