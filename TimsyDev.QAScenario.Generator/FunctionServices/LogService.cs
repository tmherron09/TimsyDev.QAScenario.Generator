using Amazon.Lambda.Core;
using System;

namespace TimsyDev.QAScenario.Generator.FunctionServices
{
    public interface ILogService
    {
        void Info(string infoString);
        void Error(string message, Exception exception);
    }
    public class LogService : ILogService
    {
        public void Info(string infoString)
        {
            LambdaLogger.Log(infoString);
        }

        public void Error(string message, Exception exception)
        {
            LambdaLogger.Log($"Error: {message}, {exception.Message}");
        }
    }
}
