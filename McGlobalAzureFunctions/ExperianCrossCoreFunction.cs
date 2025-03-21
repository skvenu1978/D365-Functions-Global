using McGlobalAzureFunctions.Abstractions.Ekyc;
using McGlobalAzureFunctions.Models.Ekyc.Experian;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.Text.Json;
using McGlobalAzureFunctions.Gate;

namespace McGlobalAzureFunctions
{
    /// <summary>
    /// ExperianCrossCoreFunction.
    /// </summary>
    public class ExperianCrossCoreFunction
    {
        private readonly ILogger<ExperianCrossCoreFunction> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly IExperianProvider _expProvider;

        /// <summary>
        /// ExperianCrossCoreFunction Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="crmServiceClient"></param>
        /// <param name="expProvider"></param>
        public ExperianCrossCoreFunction(ILogger<ExperianCrossCoreFunction> logger,IOrganizationServiceAsync2 crmServiceClient, IExperianProvider expProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(expProvider, nameof(expProvider));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
            _expProvider = expProvider;
        }

        /// <summary>
        /// Main Run Function
        /// </summary>
        /// <param name="req"></param>
        /// <param name="contactId"></param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        [Function("ekycbycrosscore")]
        public async Task<ContentResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ekycbycrosscore")] HttpRequestData req, [FromQuery] string contactId, FunctionContext executionContext)
        {
            _logger.LogDebug("ScreeningAndImport function triggered");
            var result = await GetContentResult(contactId);
            return result;
        }

        /// <summary>
        /// GetContentResult
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public async Task<ContentResult> GetContentResult(string contactId)
        {
            string? authUrl = Environment.GetEnvironmentVariable("ExperianCCAuthUrl", EnvironmentVariableTarget.Process);
            string? clientId = Environment.GetEnvironmentVariable("ExperianCCClientId", EnvironmentVariableTarget.Process);
            string? clientSecret = Environment.GetEnvironmentVariable("ExperianCCClientSecret", EnvironmentVariableTarget.Process);
            string? userName = Environment.GetEnvironmentVariable("ExperianCCUserName", EnvironmentVariableTarget.Process);
            string? password = Environment.GetEnvironmentVariable("ExperianCCPassword", EnvironmentVariableTarget.Process);
            string? screeningApi = Environment.GetEnvironmentVariable("ExperianScreeningApi", EnvironmentVariableTarget.Process);

            Protect.ForNullOrWhiteSpace(authUrl, nameof(authUrl));
            Protect.ForNullOrWhiteSpace(clientId, nameof(clientId));
            Protect.ForNullOrWhiteSpace(clientSecret, nameof(clientSecret));

            ExperianAuthBody body = new()
            {
                UserName = userName,
                Password = password,
                ClientId = clientId,
                ClientSecret = clientSecret,
            };

            string authContent = JsonSerializer.Serialize(body);

            string? token = await _expProvider.GetToken(authUrl, authContent);

            if (!string.IsNullOrEmpty(token))
            {
                return new ContentResult()
                {
                    Content = "Unable to authenticate with Experian Cross Core",
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            bool response = await _expProvider.DoeKYCCheck(token, contactId, _crmServiceClient, screeningApi);

            var result = new ContentResult()
            {
                Content = "Request is processed",
                ContentType = "application/json; charset=UTF-8",
                StatusCode = 200
            };

            return result;
        }
    }
}
