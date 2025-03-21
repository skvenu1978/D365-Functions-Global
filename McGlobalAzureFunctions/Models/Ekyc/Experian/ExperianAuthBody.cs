using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc.Experian
{
    /// <summary>
    /// ExperianAuthBody.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExperianAuthBody
    {
        /// <summary>
        /// Gets or Sets UserName.
        /// </summary>
        [JsonPropertyName("username")]
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or Sets Password.
        /// </summary>
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or Sets ClientId.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        /// <summary>
        /// Gets or Sets ClientSecret.
        /// </summary>
        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; }
    }
}
