using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public class ValueModel
    {
        public string RowId { get; set; }
        public FeatureModel Feature { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public bool IsTest { get; set; }
        public bool IsOutlier { get; set; }

        public ValueModel(string rowId, FeatureModel feature, string value, bool isTest)
        {
            RowId = rowId;
            Feature = feature;
            OldValue = value;
            NewValue = value;
            IsTest = isTest;
            IsOutlier = false;
        }
    }
}