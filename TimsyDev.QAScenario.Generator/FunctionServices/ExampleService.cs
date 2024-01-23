using Microsoft.Extensions.Logging;
using System;
using TimsyDev.QAScenario.Generator.Config;

namespace TimsyDev.QAScenario.Generator.FunctionServices
{
    public interface IExampleService
    {
        int ExampleAddAndLog(int x, int y);
        string ExampleLogException();
        void ExampleAppConfigDI();
    }
    public class ExampleService : IExampleService
    {
        private readonly ILogger<ExampleService> _logger;
        private readonly IAppConfig _appConfig;

        public ExampleService(ILogger<ExampleService> logger, IAppConfig appConfig)
        {
            _logger = logger;
            _appConfig = appConfig;
        }

        public int ExampleAddAndLog(int x, int y)
        {
            try
            {
                _logger.LogInformation("Begin Example Service Method and Logging example. Now Adding {exampleIntX} and {exampleIntY}.", x, y);

                var exampleResult = x + y;

                _logger.LogInformation("End Example Service Method: Result {exampleResult}.", exampleResult);
                return exampleResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExampleAddAndLog: This Message should not be possible to occur.");
                throw;
            }
        }

        public string ExampleLogException()
        {
            try
            {
                _logger.LogInformation("Begin Example Service Method and Logging Exceptions example. Now Calling first Inner Method.");

                return InnerMethodOne();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Expected Error: Example Exception Thrown from {throwMethodName} => {middleMethodName} => {innerMethodName}", nameof(InnerMethodThree), nameof(InnerMethodTwo), nameof(InnerMethodOne));
                return "Successfully Threw Example Exceptions.";
            }
        }

        public void ExampleAppConfigDI()
        {
            _logger.LogInformation("ExampleAppConfigDI Now Logging AppConfig ExampleServiceMessage: {exampleServiceMessage}", _appConfig.ExampleServiceMessage);
        }

        private string InnerMethodOne()
        {
            return InnerMethodTwo();
        }

        private string InnerMethodTwo()
        {
            return InnerMethodThree();
        }

        private string InnerMethodThree()
        {
            _logger.LogInformation("Logging from Inner Method Three Prior to Exception.");
            throw new Exception("Expected Exception throw to display logging results.");
        }

    }
}
