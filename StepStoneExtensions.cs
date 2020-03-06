namespace Microsoft.Extensions.DependencyInjection
{
    using EvoApi.Services.StepStone;
    using EvoApi.Services.StepStone.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Linq;

    /// <summary>Extensions to inject various stores for the core service.</summary>
    static class StepStoneExtensions
    {
        /// <summary>Register the StepStone API service for dependency injection.
        /// <example>Add a config section for each provider to appsettings.json:
        /// <code>
        /// "StepStone": [
        ///     {
        ///         "Name":              "Optional, fallback to Url",
        ///         "Url":               "https://recruiter.{StepStone brand site}",
        ///         "ClientID":          "████████-████-████-████-████████████",
        ///         "ClientSecret":      "████████-████-████-████-████████████",
        ///         "RecruiterUsername": "████████",
        ///         "RecruiterPassword": "████████"
        ///     },
        ///     ...
        /// ]</code>
        /// <para>The "Name" properties are used in <see cref="IStepStoneService"/> to distinguish between providers.</para>
        /// Pass the whole section:
        /// <code>
        /// public class Startup { ...
        ///     ConfigureServices(IServiceCollection services) { ...
        ///         services.AddStepStoneService("App Name", this.Configuration.GetSection("StepStone"));
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="services">The service collection to extend.</param>
        /// <param name="application">An application name to pass with requests.</param>
        /// <param name="config">The config section for the StepStone brands available.</param>
        /// <returns>The expanded service</returns>
        public static IServiceCollection AddStepStoneService(this IServiceCollection services, string application, IConfigurationSection config)
        {
            if (config == null)
                return services;

            // Parse config services and exclude any incomplete
            var settings =
                from c in config.GetChildren()
                let s = new ServiceSettings
                {
                    Name = c["Name"] ?? c["Url"],
                    Url = c["Url"],
                    ClientID = c["ClientID"],
                    ClientSecret = c["ClientSecret"],
                    RecruiterUsername = c["RecruiterUsername"],
                    RecruiterPassword = c["RecruiterPassword"]
                }
                where
                    !string.IsNullOrEmpty(s.Url) &&
                    !string.IsNullOrEmpty(s.ClientID) &&
                    !string.IsNullOrEmpty(s.ClientSecret) &&
                    !string.IsNullOrEmpty(s.RecruiterUsername) &&
                    !string.IsNullOrEmpty(s.RecruiterPassword)
                select s;

            // Add StepStone.co.uk API service
            return services.AddSingleton<IStepStoneService>(
                sp => new StepStoneService(sp.GetService<ILoggerFactory>(), application ?? "Evolution", settings.ToArray()));
        }
    }
}