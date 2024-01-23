using Microsoft.Extensions.Logging;

namespace TimsyDev.QAScenario.Generator.Models
{
    public static class LocalLogEvents
    {
        public static readonly EventId FunctionStart = new(100, "Function Start");
        public static readonly EventId LogRequestItem = new(101, "Log Request Item");
        public static readonly EventId GetItem = new(301, "Get Item");
        public static readonly EventId GetItems = new(302, "Get Items");
        public static readonly EventId Return500Response = new(500, "Return 500 Response");
    }
}
