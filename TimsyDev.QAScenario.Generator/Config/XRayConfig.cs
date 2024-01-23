namespace TimsyDev.QAScenario.Generator.Config
{
    public interface IXRayConfig : IConfigSettings
    {
        bool IsEnabled { get; }
    }
    public class XRayConfig : IXRayConfig
    {
        public bool IsEnabled { get; set; }
    }
}
