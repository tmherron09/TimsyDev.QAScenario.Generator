using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TimsyDev.QAScenario.Generator.Config;
using TimsyDev.QAScenario.Generator.FunctionServices;
using TimsyDev.QAScenario.Generator.Models;

namespace TimsyDev.QAScenario.Generator.Tests;

[TestFixture]
public class FunctionTest
{
    private readonly Faker _faker = new Faker();
    private List<object> _mockServices;
    private Mock<IExampleService> _exampleService;
    private Mock<ILogService> _logService;
    private Mock<ILogger<Function>> _logger;
    private Mock<IConfigurationService> _configurationService;
    private Mock<IAppConfig> _appConfig;

    [SetUp]
    public void SetUp()
    {
        _exampleService = new Mock<IExampleService>();
        _logService = new Mock<ILogService>();
        _logger = new Mock<ILogger<Function>>();
        _configurationService = new Mock<IConfigurationService>();
        _appConfig = new Mock<IAppConfig>();

        _mockServices = new List<object>()
        {
            _exampleService.Object,
            _logService.Object,
            _logger.Object,
            _configurationService.Object,
            _appConfig.Object
        };
    }

    [Test]
    public async Task TestFunctionInvocation()
    {

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function(_mockServices);
        var context = new TestLambdaContext();
        // var result = function.FunctionHandlerAsync(new APIGatewayProxyRequest(), context);

        Assert.DoesNotThrowAsync(async () => await function.FunctionHandlerAsync(new APIGatewayProxyRequest(), context));
        var result = await function.FunctionHandlerAsync(new APIGatewayProxyRequest(), context);
        Assert.IsNotNull(result);

    }

    [Test]
    public async Task TestExampleFunctionInvocation()
    {
        var expectedResponseBody = "TimsyDev Boilerplate Example FunctionHandlerAsync Invocation.";
        var expectedStatusCode = 200;
        var expectedAllowedHeaders = AccessControlAllowHeaders.ApplicationJson;
        var expectedAllowedOrigins = "*";
        var expectedAllowedMethods = "OPTIONS, GET";

        // Invoke the lambda function and confirm the string was upper cased.
        var function = new Function(_mockServices);
        var context = new TestLambdaContext();
        // var result = function.FunctionHandlerAsync(new APIGatewayProxyRequest(), context);
        APIGatewayProxyResponse result = null;
        Assert.DoesNotThrowAsync(async () => result = await function.ExampleFunctionHandlerAsync(new APIGatewayProxyRequest(), context));
        //var result = await function.FunctionHandlerAsync(new APIGatewayProxyRequest(), context);
        Assert.IsNotNull(result);
        Assert.That(result.Body == expectedResponseBody);
        Assert.That(result.StatusCode == expectedStatusCode);
        Assert.That(result.Headers["Access-Control-Allow-Headers"] == expectedAllowedHeaders);
        Assert.That(result.Headers["Access-Control-Allow-Origin"] == expectedAllowedOrigins);
        Assert.That(result.Headers["Access-Control-Allow-Methods"] == expectedAllowedMethods);
    }

    [Test]
    public void GenerateExampleRequestItem()
    {
        var exampleRequestItem = new ExampleRequestItem()
        {
            Name = "Example Request Item",
            IsRequested = false,
            Colors = new List<string>()
            {
                "Orange", "Yellow", "Blue", "Green"
            },
            Value = 2_023,
            InnerItem = new ExampleRequestInnerItem()
            {
                InnerColors = new List<string>()
                {
                    "Red", "Purple", "Pink", "Teal"
                },
                InnerValue = long.MaxValue,
                Location = "I'm The Inside Example."
            }
        };

        var request = new APIGatewayProxyRequest()
        {
            QueryStringParameters = new Dictionary<string, string>()
            {
                {"QueryName", "QueryParam Name"},
                {"IntValue", "500"}
            },
            MultiValueQueryStringParameters = new Dictionary<string, IList<string>>()
            {
                {"QueryFood", new List<string>()
                    {
                        "Cheese", "Pizza", "Chocolate", "Bacon"
                    }
                }
            },
            Body = JsonSerializer.Serialize(exampleRequestItem)
        };

        var testValue = JsonSerializer.Serialize(request);

        Assert.That(!string.IsNullOrEmpty(testValue));
    }
}
