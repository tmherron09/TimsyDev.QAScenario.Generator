namespace TimsyDev.QAScenario.Generator.Config
{
    public interface IAppConfig : IConfigSettings
    {
        string ApplicationName { get; }
        string ExampleServiceMessage { get; }
    }
    public class AppConfig : IAppConfig
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string ExampleServiceMessage { get; set; } = string.Empty;
    }
}
