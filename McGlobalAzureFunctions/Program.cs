// <copyright file="Program.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using McGlobalAzureFunctions.Abstractions.Fox;
    using McGlobalAzureFunctions.Abstractions.Requests;
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.DAL.Fox;
    using McGlobalAzureFunctions.DAL.Ekyc;
    using McGlobalAzureFunctions.DAL.Requests;
    using McGlobalAzureFunctions.DAL.Responses;
    using McGlobalAzureFunctions.Const;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using System.Diagnostics.CodeAnalysis;
    using McGlobalAzureFunctions.Abstractions.Ekyc;

    /// <summary>
    /// Startup
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// Main function for
        /// setup
        /// </summary>
        public static void Main()
        {
            var d365ConString = Environment.GetEnvironmentVariable(FnConstants.D365ConnectionString, EnvironmentVariableTarget.Process);

            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IOrganizationServiceAsync2>(_ => new ServiceClient(d365ConString));
                    services.AddTransient<IFoxProvider, FoxProvider>();
                    services.AddTransient<IAlertProvider, AlertProvider>();
                    services.AddTransient<ICreditTierProvider, CreditTierProvider>();
                    services.AddTransient<IClientResponseProvider, ClientResponseProvider>();
                    services.AddTransient<IAttachmentProvider, AttachmentProvider>();
                    services.AddTransient<IEmailProvider, EmailProvider>();
                    services.AddTransient<IOptyRdrProvider, OptyRdrProvider>();
                    services.AddTransient<IContactProvider, ContactProvider>();
                    services.AddTransient<IActivityProvider, ActivityProvider>();
                    services.AddTransient<IFraudProvider, FraudProvider>();
                    services.AddTransient<IRateProvider, RateProvider>();
                    services.AddTransient<ILexisNexisProvider, LexisNexisProvider>();
                    services.AddTransient<IExperianProvider, ExperianProvider>();
                    services.AddTransient<ILexisNexisUSProvider, LexisNexisUSProvider>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.Services.Configure<LoggerFilterOptions>(options =>
                    {
                        LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                    == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                        if (defaultRule is not null)
                        {
                            options.Rules.Remove(defaultRule);
                        }
                    });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .Build();

            host.Run();
        }
    }
}