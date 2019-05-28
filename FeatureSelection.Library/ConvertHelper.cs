using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public static class ConvertHelper
    {
        public static IEnumerable<string> DataSetToCSV(IEnumerable<string> headerRow, IEnumerable<IEnumerable<string>> dataSet, string joinSeparator)
        {
            return (new List<IEnumerable<string>>() { headerRow }).Concat(dataSet).Select(r => string.Join(joinSeparator, r.ToArray()));
        }

        public static IList<IList<string>> CSVToDataConstrutor(string[] csvRows, char splitSeparator)
        {
            return csvRows.Select(l => (IList<string>)l.Split(splitSeparator).ToList()).ToList();
        }

        public static IList<string> ResultToCSV(string header, double[] testResults, IEnumerable<string> testIds)
        {
            return new string[] { header }.Concat(testResults.Select((r, i) => testIds.ElementAt(i) + ',' + Convert.ToString(r, NumberFormatInfo.InvariantInfo))).ToList();
        }
    }
}