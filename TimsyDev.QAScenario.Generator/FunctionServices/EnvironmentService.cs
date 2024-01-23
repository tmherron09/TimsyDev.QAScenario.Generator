using System;

namespace TimsyDev.QAScenario.Generator.FunctionServices
{
    public interface IEnvironmentService
    {
        string EnvironmentName { get; set; }
    }
    public class EnvironmentService : IEnvironmentService
    {
        public string EnvironmentName { get; set; }

        public EnvironmentService()
        {
            EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }
    }
}
