// <copyright file="LexisNexisUSFunction.cs" company="moneycorp">
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
    /// LexisNexisUSFunction.
    /// </summary>
    public class LexisNexisUSFunction
    {
        private readonly ILogger<LexisNexisUSFunction> _logger;
        private readonly IOrganizationServiceAsync2 _crmServiceClient;
        private readonly ILexisNexisUSProvider _lxProvider;

        /// <summary>
        /// LexisNexisUSFunction Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="crmServiceClient"></param>
        /// <param name="lxProvider"></param>
        public LexisNexisUSFunction(ILogger<LexisNexisUSFunction> logger, IOrganizationServiceAsync2 crmServiceClient, ILexisNexisUSProvider lxProvider)
        {
            Protect.ForNull(logger, nameof(logger));
            Protect.ForNull(crmServiceClient, nameof(crmServiceClient));
            Protect.ForNull(lxProvider, nameof(lxProvider));
            _logger = logger;
            _crmServiceClient = crmServiceClient;
            _lxProvider = lxProvider;
        }

        /// <summary>
        /// Function Main Run.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="contactId"></param>
        /// <param name="executionContext"></param>
        /// <returns></returns>
        [Function("ekycbylexisnexisus")]
        public async Task<ContentResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ekycbylexisnexisus")] HttpRequestData req, [FromQuery] string contactId, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Screening US contact by Lexis Nexis");
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
            var instantIDApi = Environment.GetEnvironmentVariable("InstantIDURL", EnvironmentVariableTarget.Process);
            var userName = Environment.GetEnvironmentVariable("InstantID_UserName", EnvironmentVariableTarget.Process);
            var password = Environment.GetEnvironmentVariable("InstantID_Password", EnvironmentVariableTarget.Process);

            if (contactId == null)
            {
                return new ContentResult()
                {
                    Content = "Please provide the contact ID",
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            if (instantIDApi == null || userName == null || password == null)
            {
                return new ContentResult()
                {
                    Content = "Lexis Nexis InstantID API or its authentication detail are  not provided",
                    ContentType = "application/json; charset=UTF-8",
                    StatusCode = 400
                };
            }

            var token = string.Empty;
            var authenticationString = $"{userName}:{password}";
            var base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
            var response = await _lxProvider.DoeKYC(base64String, instantIDApi, contactId, _crmServiceClient);

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
