using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public class DataModel
    {
        public IEnumerable<string> HeadersWithClass { get; set; }
        public IEnumerable<string> HeadersWithoutClass { get; set; }
        public IEnumerable<IEnumerable<string>> OutputTrain { get; set; }
        public IEnumerable<IEnumerable<string>> OutputTest { get; set; }
        public IEnumerable<string> TestIds { get; set; }
    }
}