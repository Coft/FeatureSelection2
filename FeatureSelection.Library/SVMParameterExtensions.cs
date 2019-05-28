using LibSVMsharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public static class SVMParameterExtensions
    {
        public static string GetOutput(this SVMParameter parameter)
        {
            return $"C:{parameter.C}\n" +
                $"CacheSize:{parameter.CacheSize}\n" +
                $"Coef0:{parameter.Coef0}\n" +
                $"Degree:{parameter.Degree}\n" +
                $"Eps:{parameter.Eps}\n" +
                $"Gamma:{parameter.Gamma}\n" +
                $"Kernel:{parameter.Kernel}\n" +
                $"Nu:{parameter.Nu}\n" +
                $"P:{parameter.P}\n" +
                $"Probability:{parameter.Probability}\n" +
                $"Shrinking:{parameter.Shrinking}\n" +
                $"Type:{parameter.Type}\n" +
                $"Wheights:{string.Join(",", parameter.Weights)}\n";
        }
    }
}