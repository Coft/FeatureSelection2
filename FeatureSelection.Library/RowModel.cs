using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureSelection.Library
{
    public class RowModel
    {
        public int Index { get; set; }
        public string Id { get; set; }
        public IList<ValueModel> Values { get; set; }
        public bool IsActive { get; set; }

        public RowModel(int index, string id)
        {
            Index = index;
            Id = id;
            Values = new List<ValueModel>();
            IsActive = true;
        }
    }
}