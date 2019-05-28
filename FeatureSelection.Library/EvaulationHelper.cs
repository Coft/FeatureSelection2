using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public static class EvaulationHelper
    {
        public static double RMSELog(double[] observed, double[] predicted)
        {
            return Math.Sqrt(observed.Select((o, i) => new { o, i }).Join(predicted.Select((p, i) => new { p, i }), o => o.i, p => p.i, (o, p) => new { o.o, p.p }).Select(c => Math.Pow(Math.Log(c.o) - Math.Log(c.p), 2)).Average());
        }

        public static double RMSE(double[] observed, double[] predicted)
        {
            return Math.Sqrt(observed.Select((o, i) => new { o, i }).Join(predicted.Select((p, i) => new { p, i }), o => o.i, p => p.i, (o, p) => new { o.o, p.p }).Select(c => Math.Pow(c.o - c.p, 2)).Average());
        }
    }
}