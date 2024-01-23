using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimsyDev.QAScenario.Generator.Models
{
    public class ExampleRequestItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public List<string> Colors { get; set; }
        public bool IsRequested { get; set; }
        public ExampleRequestInnerItem InnerItem { get; set; }

    }

    public class ExampleRequestInnerItem
    {
        public string Location { get; set; }
        public long InnerValue { get; set; }
        public List<string> InnerColors { get; set; }
    }
}
