// <copyright file="LexisNexisFunction.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using System;
    using System.Threading;
    using static System.Net.Mime.MediaTypeNames;
    using static System.Runtime.InteropServices.JavaScript.JSType;
    using McGlobalAzureFunctions.Abstractions.Ekyc;
    using McGlobalAzureFunctions.Gate;

    /// <summary>
    /// LexisNexisFunction.
    /// </summary>
    public class LexisNexisFunction
    {
        private readonly ILogger<LexisNexisFunction> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly ILexisNexisProvider _lxProvider;

        /// <summary>
        /// LexisNexisFunction Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="crmServiceClient"></param>
        /// <param name="lxProvider"></param>
        public LexisNexisFunction(ILogger<LexisNexisFunction> logger, IOrganizationServiceAsync2 crmServiceClient, ILexisNexisProvider lxProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(lxProvider, nameof(lxProvider));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
            _lxProvider = lxProvider;
        }

        /// <summary>
        /// Main Run Function.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="contactId"></param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        [Function("ekycbylexisnexis")]
        public async Task<ContentResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ekycbylexisnexis")] HttpRequestData req, [FromQuery] string contactId, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Screening contact by Lexis Nexis");
            logger.LogDebug("ScreeningAndImport function triggered");
            var result = await GetContentResult(contactId);

            return result;
        }

        /// <summary>
        /// GetContentResult.
        /// </summary>
        /// <param name="contactId">contactId</param>
        /// <returns>ContentResult task</returns>
        public async Task<ContentResult> GetContentResult(string? contactId)
        {
            var screeningURL = Environment.GetEnvironmentVariable("LexisNexisScreeningURL", EnvironmentVariableTarget.Process);
            var clientId = Environment.GetEnvironmentVariable("LexisNexisClientId", EnvironmentVariableTarget.Process);
            var clientSecret = Environment.GetEnvironmentVariable("LexisNexisClientSecret", EnvironmentVariableTarget.Process);

            if (contactId == null)
            {
                return new ContentResult()
                {
                    Content = "Please provide the contact ID",
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            if (clientId == null || clientSecret == null || screeningURL == null)
            {
                return new ContentResult()
                {
                    Content = "Lexis Nexis API or authentication detail is not provided",
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            var authenticationString = $"{clientId}:{clientSecret}";
            var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
            var response = await _lxProvider.GetScreeningRecord(base64String, screeningURL, contactId, _crmServiceClient);

            if (response.IsSuccessful == false)
            {
                return new ContentResult()
                {
                    Content = response.Message,
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            var result = new ContentResult()
            {
                Content = response.Message,
                ContentType = "application/json; charset=UTF-8",
                StatusCode = 200
            };

            return result;
        }
    }
}

