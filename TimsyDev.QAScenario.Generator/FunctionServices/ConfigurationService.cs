using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TimsyDev.QAScenario.Generator.Config;

namespace TimsyDev.QAScenario.Generator.FunctionServices
{
    public interface IConfigurationService
    {
        T? GetConfig<T>(string config) where T : IConfigSettings;
        IConfiguration GetConfiguration();
        AppConfig GetAppConfig();
        XRayConfig GetXRayConfig();
    }
    public class ConfigurationService : IConfigurationService
    {
        public IEnvironmentService EnvService { get; }
        private IConfiguration Configuration { get; }

        public ConfigurationService(IEnvironmentService envService)
        {
            EnvService = envService;
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{EnvService.EnvironmentName}.json")
                .AddEnvironmentVariables()
                .Build();

            if (GetXRayConfig()?.IsEnabled ?? false)
            {
                // TODO Add Xray
                // FlurlHttp.Configure(settings => settings.HttpClientFactory = new XRayHttpClientFactory());
            }

        }

        public T GetConfig<T>(string config) where T : IConfigSettings
        {
            return Configuration
                .GetSection(config)
                .Get<T>() ?? throw new InvalidOperationException($"Error: Appsettings.{EnvService.EnvironmentName}.json does not have requested Section: {config}");
        }

        public IConfiguration GetConfiguration()
        {
            return Configuration;
        }

        public AppConfig GetAppConfig() =>
            GetConfig<AppConfig>(ConfigSection.AppConfig) ?? throw new InvalidOperationException($"Error: Appsettings.{EnvService.EnvironmentName}.json Missing \"AppConfig\" Section");

        public XRayConfig GetXRayConfig() =>
            GetConfig<XRayConfig>(ConfigSection.XRayConfig) ?? throw new InvalidOperationException($"Error: Appsettings.{EnvService.EnvironmentName}.json Missing \"XRayConfig\" Section");

    }
}
