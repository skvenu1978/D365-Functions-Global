using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc.Experian
{
    /// <summary>
    /// ExperianCCResponse.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExperianCCResponse
    {
        /// <summary>
        /// Gets or Sets responseHeader.
        /// </summary>
        public ResponseHeader? responseHeader { get; set; }

        /// <summary>
        /// Gets or Sets clientResponsePayload.
        /// </summary>
        public ClientResponsePayload? clientResponsePayload { get; set; }

        /// <summary>
        /// Gets or Sets originalRequestData.
        /// </summary>
        public OriginalRequestData? originalRequestData { get; set; }
    }

    /// <summary>
    /// ClientResponsePayload.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ClientResponsePayload
    {
        /// <summary>
        /// Gets or Sets orchestrationDecisions.
        /// </summary>
        public List<OrchestrationDecision>? orchestrationDecisions { get; set; }

        /// <summary>
        /// Gets or Sets decisionElements.
        /// </summary>
        public List<DecisionElement>? decisionElements { get; set; }
    }

    /// <summary>
    /// DecisionElement.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DecisionElement
    {
        /// <summary>
        /// Gets or Sets applicantId.
        /// </summary>
        public string? applicantId { get; set; }

        /// <summary>
        /// Gets or Sets serviceName.
        /// </summary>
        public string? serviceName { get; set; }

        /// <summary>
        /// Gets or Sets decision.
        /// </summary>
        public string? decision { get; set; }

        /// <summary>
        /// Gets or Sets score.
        /// </summary>
        public int score { get; set; }

        /// <summary>
        /// Gets or Sets decisionText.
        /// </summary>
        public string? decisionText { get; set; }

        /// <summary>
        /// Gets or Sets decisionReason.
        /// </summary>
        public string? decisionReason { get; set; }

        /// <summary>
        /// Gets or Sets appReference.
        /// </summary>
        public string? appReference { get; set; }

        /// <summary>
        /// Gets or Sets matches.
        /// </summary>
        public List<Match>? matches { get; set; }

        /// <summary>
        /// Gets or Sets decisionReason.
        /// </summary>
        public List<Score>? scores { get; set; }

        /// <summary>
        /// Gets or Sets warningsErrors.
        /// </summary>
        public List<object>? warningsErrors { get; set; }

        /// <summary>
        /// Gets or Sets dataCounts.
        /// </summary>
        public List<DataCount>? dataCounts { get; set; }

        /// <summary>
        /// Gets or Sets rules.
        /// </summary>
        public List<Rule>? rules { get; set; }

        /// <summary>
        /// Gets or Sets otherData.
        /// </summary>
        public OtherData? otherData { get; set; }
    }

    /// <summary>
    /// DataCount.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class DataCount
    {
        /// <summary>
        /// Gets or Sets name.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// Gets or Sets value.
        /// </summary>
        public long? value { get; set; }
    }

    /// <summary>
    /// Match.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Match
    {
        /// <summary>
        /// Gets or Sets name.
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// Gets or Sets value.
        /// </summary>
        public string? value { get; set; }
    }

    /// <summary>
    /// OtherData.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OtherData
    {

        public string response { get; set; }
    }

    /// <summary>
    /// Rule.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Rule
    {
        public string ruleName { get; set; }

        public string ruleId { get; set; }

        public string ruleText { get; set; }

        public int ruleScore { get; set; }
    }

    /// <summary>
    /// Score.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class Score
    {
        public string? Name { get; set; }

        public string? Type { get; set; }

        public int? score { get; set; }
    }

    /// <summary>
    /// OriginalRequestData.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OriginalRequestData
    {

        public Application? application { get; set; }


        public List<Contact>? contacts { get; set; }
    }

    /// <summary>
    /// OrchestrationDecision.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class OrchestrationDecision
    {

        public string? sequenceId { get; set; }


        public string? decisionSource { get; set; }


        public string? decision { get; set; }


        public List<string>? DdcisionReasons { get; set; }


        public int score { get; set; }


        public string? decisionText { get; set; }


        public string? nextAction { get; set; }


        public string? decisionTime { get; set; }
    }

    /// <summary>
    /// ResponseHeader.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class ResponseHeader
    {

        public string? tenantId { get; set; }


        public string? requestType { get; set; }

        public string? clientReferenceId { get; set; }


        public string? expRequestId { get; set; }


        public string? messageTime { get; set; }

        public OverallResponse? overallResponse { get; set; }

        public string? responseCode { get; set; }

        public string? responseType { get; set; }

        public string? responseMessage { get; set; }
    }

    /// <summary>
    /// OverallResponse.
    /// </summary>
    public partial class OverallResponse
    {
        public string? decision { get; set; }

        public string? decisionText { get; set; }

        public List<string>? decisionReasons { get; set; }
    }
}

