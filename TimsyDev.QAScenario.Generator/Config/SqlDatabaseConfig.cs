namespace TimsyDev.QAScenario.Generator.Config
{
    public interface ISqlDatabaseConfig
    {
        string DatabaseServer { get; }
        string Database { get; }
        string DatabaseUsername { get; }
        string DatabasePassword { get; }
        int DatabaseMaxPoolSize { get; }
        int DatabaseCommandTimeoutSeconds { get; }
    }

    public class SqlDatabaseConfig : ISqlDatabaseConfig
    {
        public string DatabaseServer { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string DatabaseUsername { get; set; } = string.Empty;
        public string DatabasePassword { get; set; } = string.Empty;
        public int DatabaseMaxPoolSize { get; set; }
        public int DatabaseCommandTimeoutSeconds { get; set; }
    }
}
