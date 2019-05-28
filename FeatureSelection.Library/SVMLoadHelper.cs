using LibSVMsharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public static class SVMLoadHelper
    {
        public static SVMProblem Load(IEnumerable<IEnumerable<string>> data, bool isWithClass)
        {
            NumberFormatInfo provider = new NumberFormatInfo();

            SVMProblem problem = new SVMProblem();

            foreach (var row in data)
            {
                var doubleRows = row
                    .Select(v => Convert.ToDouble(v))
                    .Select((d, i) => new { d, i })
                    .Reverse();

                SVMNode[] nodes = doubleRows
                    .Skip(isWithClass ? 1 : 0)
                    .Select(d => new SVMNode() { Index = d.i + 1, Value = d.d })
                    .ToArray();

                double y = isWithClass ? doubleRows.First().d : 0;
                problem.Add(nodes, y);
            }

            return problem;
        }
    }
}