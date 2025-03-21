using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc.Experian
{
    public class CrmEKYCResult
    {
        public string? Decision { get; set; }
        public int DecisionScore { get; set; }
        public string? DecisionText { get; set; }
        public string? DecisionReason {  get; set; }
        public string? AppReference {  get; set; }
        public string? DecisionCode {  get; set; }
        public string? ExpRequestId {  get; set; }
    }
}
