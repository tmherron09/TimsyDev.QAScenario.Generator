using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TimsyDev.QAScenario.Generator.Config;
using TimsyDev.QAScenario.Generator.Extensions;
using TimsyDev.QAScenario.Generator.FunctionServices;
using TimsyDev.QAScenario.Generator.Models;
using TimsyDev.QAScenario.Generator.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace TimsyDev.QAScenario.Generator;

public class Function
{
    private IConfigurationService _configService;
    private IConfiguration _configuration;
    private IAppConfig _appConfig;
    private AWSOptions _awsOptions;

    // Set Function Level Logging Service.
    // Use ILogger<T> In DI Services
    private readonly ILogService _logService;
    private readonly ILogger<Function> _logger;

    /*
     * Uncomment if using XRay Configuration
     * private IXRayConfig _xrayConfig; 
     * private readonly IAWSXRayRecorder _recorder;
     */

    private readonly IGeneratorS3Service _generatorS3Service;

    // Example Service used to Test Implementation of Project Boilerplate
    private readonly IExampleService _exampleService;

    public Function()
    {
        SetupConfiguration();

        try
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            // _recorder = serviceProvider.GetService<IAWSXRayRecorder>();
            _logService = serviceProvider.GetService<ILogService>() ?? throw new InvalidOperationException($"Error: Unable to GetService: {nameof(ILogService)} from ServiceProvider.");
            _logger = serviceProvider.GetService<ILogger<Function>>() ?? throw new InvalidOperationException($"Error: Unable to GetService: {nameof(ILogger<Function>)} from ServiceProvider.");
            _exampleService = serviceProvider.GetService<IExampleService>() ?? throw new InvalidOperationException($"Error: Unable to GetService: {nameof(IExampleService)} from ServiceProvider.");
            _generatorS3Service = serviceProvider.GetService<IGeneratorS3Service>() ?? throw new InvalidOperationException($"Error: Unable to GetService: {nameof(IGeneratorS3Service)} from ServiceProvider.");
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, $"Error Starting FunctionHandler: {typeof(Function).Namespace}");
            throw;
        }
    }

    public Function(IReadOnlyCollection<object> mockServices = null)
    {
        SetupConfiguration();

        _appConfig = (IAppConfig)mockServices.First(x => x is IAppConfig);

        _configService = (IConfigurationService)mockServices.First(x => x is IConfigurationService);
        // _recorder = (IAWSXRayRecorder)mockServices.First(x => x is IAWSXRayRecorder);
        _logger = (ILogger<Function>)mockServices.First(x => x is ILogger<Function>);
        _logService = (ILogService)mockServices.First(x => x is ILogService);
        _exampleService = (IExampleService)mockServices.First(x => x is IExampleService);
    }

    private void SetupConfiguration()
    {
        //  bootstrap configuration
        var envService = new EnvironmentService();
        _configService = new ConfigurationService(envService);

        _configuration = _configService.GetConfiguration();

        SetFunctionHandlerConfigs();

        var logConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", _appConfig.ApplicationName)
            .Enrich.WithProperty("Machine", Environment.MachineName)
            .WriteTo.Console(new RenderedCompactJsonFormatter());

        if (envService.EnvironmentName != "Deployed")
        {
            logConfig.WriteTo.File(new RenderedCompactJsonFormatter(), "App_Data/log.txt", rollingInterval: RollingInterval.Day).WriteTo.File(new RenderedCompactJsonFormatter(), "App_Data/formatted_log.json", rollingInterval: RollingInterval.Hour);
        }

        Log.Logger = logConfig.CreateLogger();
    }

    private void SetFunctionHandlerConfigs()
    {
        _appConfig = _configService.GetAppConfig();
        _awsOptions = _configuration.GetAWSOptions();
    }

    private void ConfigureServices(ServiceCollection serviceCollection)
    {
        // Add the Singleton instance of XRayRecorder if using XRay.
        // IAWSXRayRecorder recorder = AWSXRayRecorder.Instance;
        // serviceCollection.AddSingleton<IAWSXRayRecorder>(recorder);

        serviceCollection.AddTransient<IEnvironmentService, EnvironmentService>();
        serviceCollection.AddTransient<IConfigurationService, ConfigurationService>();
        serviceCollection.AddTransient<ILogService, LogService>();
        serviceCollection.AddLogging(x =>
        {
            x.ClearProviders();
            x.AddSerilog(dispose: true);
        });

        serviceCollection.AddDefaultAWSOptions(_awsOptions);

        serviceCollection.AddSingleton<IAppConfig>(_appConfig);

        // Example of DI - Remove Prior to Production
        serviceCollection.AddSingleton<IExampleService, ExampleService>();
        serviceCollection.AddAWSService<IAmazonS3>();
        serviceCollection.AddSingleton<IGeneratorS3Service, GeneratorS3Service>();

    }

    /// <summary>
    /// Blank (Async) FunctionHandler for Boilerplate.
    /// Starting point for new TimsyDev Lambda Function.
    /// </summary>
    /// <param name="request">Request from API Gateway Proxy.</param>
    /// <param name="context"></param>
    /// <returns>Proxy Response Item</returns>
    public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        LogFunctionEntry(request, context);

        DocumentRequest documentRequest;

        try
        {
            documentRequest = JsonSerializer.Deserialize<DocumentRequest>(request.Body);
        }
        catch (Exception ex)
        {
            documentRequest = new DocumentRequest();
        }

        var bucketName = "qa-scenario.generator-files-by-extension";
        var objectName = "PDF.pdf";

        Log.Logger.Information("Beginning S3 File Download Test.");

        var responseFile = await _generatorS3Service.ReturnS3FileEncoded(bucketName, objectName);

        var response = new APIGatewayProxyResponse
        {
            Body = responseFile,
            
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/pdf" }, { "Content-Disposition", "filename=\"QA-PDF.pdf\"" } },
            MultiValueHeaders = new Dictionary<string, IList<string>>{ { "Content-Disposition", new List<string>{ "attachment", $"filename=\"{documentRequest.FullReturnFileName}\"" } } },
            IsBase64Encoded = true
        };

        return response;
    }

    /// <summary>
    /// Simple Function for Boilerplate Testing. Update your aws-lambda-tools-defaults.json
    /// function-handler to point to ExampleFunctionHandlerAsync
    /// Example (If Namespace has not been updated):
    ///     "function-handler": "TimsyDev.QAScenario.Generator::TimsyDev.QAScenario.Generator.Function::ExampleFunctionHandlerAsync"
    /// </summary>
    /// <param name="request">No Example Request will be consumed. WIP </param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> ExampleFunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            _logger.LogTrace(LocalLogEvents.FunctionStart, "Function Start: {functionHandler}\tNamespace: {namespace}", nameof(ExampleFunctionHandlerAsync), typeof(Function).Namespace);

            // _recorder.AddAnnotation($"typeof(Function).Namespace", "FunctionHandlerAsync");
            // _recorder.AddMetadata("author", "<insert dev name>");

            var contextDisplay = JsonSerializer.Serialize(context);
            Log.Logger.Information("Logging ExampleFunctionHandlerAsync @LambdaContext: {@lambdaContext}", context);
            Log.Logger.Information("Logging ExampleFunctionHandlerAsync LambdaContext: {lambdaContext}",
                contextDisplay);

            _exampleService.ExampleAddAndLog(1986, 2023);

            _exampleService.ExampleLogException();
            _exampleService.ExampleAppConfigDI();

            var response = APIGatewayProxyResponseWrapper.Build(200, "TimsyDev Boilerplate Example FunctionHandlerAsync Invocation.");

            await Task.Run(() =>
            {
                // Example difference of each Logging mechanism.
                var logObject = new { name = "Log Object", numbericValue = 1986 };
                Log.Logger.Information("Placeholder awaiter Log.");
                _logService.Info($"ILogService Log Info - Log Object: {logObject}");
                _logger.LogInformation("ILogger<Function> Log Info - Log Object: {@logObject}", logObject);
            });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(LocalLogEvents.Return500Response, ex, "Failure Alert: {namespace} {functionHandler} has Failed.", typeof(Function).Namespace, nameof(ExampleFunctionHandlerAsync));
            throw;
        }
        finally
        {
            _logger.LogTrace("Trace");
        }
    }

    public async Task<APIGatewayProxyResponse> ExampleRequestFunctionHandlerAsync(APIGatewayProxyRequest request,
        ILambdaContext context)
    {
        LogFunctionEntry(request, context);

        request.QueryStringParameters.TryGetValue("QueryName", out string queryParamName);
        request.QueryStringParameters.TryGetValue("IntValue", out string intValueString);
        request.MultiValueQueryStringParameters.TryGetValue("QueryFood", out IList<string> queryFood);

        var exampleBody = JsonSerializer.Deserialize<ExampleRequestItem>(request.Body);

        var intValue = int.Parse(intValueString);

        _logger.LogInformation("QueryName: {queryName}", queryParamName);
        _logger.LogInformation("IntValue: {intValue}", intValue);
        _logger.LogInformation("QueryFood: {@queryFood}", queryFood);
        _logger.LogInformation("ExampleRequestItem: {exampleRequestItem}", exampleBody);
        _logger.LogInformation("ExampleRequestItem Expanded: {@exampleRequestItem}", exampleBody);
        _logger.LogInformation("ExampleRequestInnerItem: {exampleRequestInnerItem}", exampleBody.InnerItem);
        _logger.LogInformation("ExampleRequestInnerItem Expanded: {@exampleRequestInnerItem}", exampleBody.InnerItem);

        await LogAsyncPlaceholder();

        return APIGatewayProxyResponseWrapper.Build(200, exampleBody);
    }

    private void LogFunctionEntry(APIGatewayProxyRequest request, ILambdaContext context)
    {
        _logger.LogInformation(LocalLogEvents.FunctionStart, "Function Start: {functionHandler}\tNamespace: {namespace}", nameof(FunctionHandlerAsync), typeof(Function).Namespace);
        _logger.LogInformation(LocalLogEvents.LogRequestItem, "Invocation Request: {@request}", request);
    }

    private async Task LogAsyncPlaceholder()
    {
        await Task.Run(() =>
        {
            Log.Logger.Information("Placeholder awaiter Log.");
        });
    }

}