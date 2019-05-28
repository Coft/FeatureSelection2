using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FeatureSelection.Library
{
    public class FeatureModel
    {
        public string Name { get; set; }
        public FeatureType Type { get; set; }
        public bool IsClass { get; set; }
        public bool IsId { get; set; }
        public bool IsActive { get; set; }
        public IList<ValueModel> Values { get; set; }
        public int Index { get; set; }

        public void ReplaceValues(string match, string target)
        {
            foreach (var value in Values.Where(v => v.NewValue == match))
            {
                value.NewValue = target;
            }
        }

        public FeatureModel(int index, string name)
        {
            Index = index;
            Name = name;
            IsActive = true;
            Type = FeatureType.String;
            Values = new List<ValueModel>();
        }

        public void FillGaps()
        {
        }

        public int GetUniqueAmount()
        {
            return Values.Distinct().Count();
        }

        public bool IsNumeric()
        {
            double myDouble;
            return Values.All(v => double.TryParse(v.NewValue, out myDouble));
        }

        public bool IsInteger()
        {
            int myInt;
            return Values.All(v => int.TryParse(v.NewValue, out myInt));
        }

        public void TransformEnumToInt()
        {
            foreach (var singleEnums in Values.Select(v => v.NewValue).Distinct().Select((name, index) => new { name, index }).ToList())
            {
                ReplaceValues(singleEnums.name, (singleEnums.index + 1).ToString());
            }
        }

        public void Transform(Func<string, string> func)
        {
            Values.ToList().ForEach(v => v.NewValue = func(v.NewValue));
        }

        public void Transform(Func<double, double> func)
        {
            Values.ToList().ForEach(v => v.NewValue = Convert.ToString(func(Convert.ToDouble(v.NewValue))));
        }

        public void Discretize(int levelAmount)
        {
            IList<double> steps = new List<double>();
            var valuesOrderd = Values.Select(n => double.Parse(n.NewValue)).OrderBy(n => n);
            var min = valuesOrderd.First();
            var max = valuesOrderd.Last();

            var step = Math.Floor((double)valuesOrderd.Count() / (levelAmount));

            for (int i = 0; i < levelAmount; i++)
            {
                steps.Add(valuesOrderd.ElementAt((int)Math.Floor((double)i * step)));
            }

            steps.Add(max + 1);

            foreach (var value in Values.Select((value, index) => new { value, index }))
            {
                var oldValue = double.Parse(value.value.NewValue);

                for (int j = 0; j < steps.Count() - 1; j++)
                {
                    if (oldValue >= steps[j] && oldValue < steps[j + 1])
                    {
                        value.value.NewValue = j.ToString();
                    }
                }
            }
        }

        public Tuple<string, double, double> IQR(double iqrMultiplier)
        {
            var sortedValues = Values.Select(v => double.Parse(v.NewValue)).OrderBy(v => v).ToList();
            var count = sortedValues.Count;
            double mediana = 0;
            double q1 = 0;
            double q3 = 0;
            double half = count / 2;
            double quater = count / 4;

            if (count % 2 == 0)
            {
                mediana = (sortedValues.ElementAt((int)half - 1) + sortedValues.ElementAt((int)half)) / 2;
            }
            else
            {
                mediana = sortedValues.ElementAt((int)Math.Floor(half));
            }

            if (quater % 2 == 0)
            {
                q1 = (sortedValues.ElementAt((int)quater - 1) + sortedValues.ElementAt((int)quater)) / 2;
                q3 = (sortedValues.ElementAt((int)quater + (int)Math.Floor(half) - 1) + sortedValues.ElementAt((int)quater + (int)Math.Floor(half))) / 2;
            }
            else
            {
                q1 = sortedValues.ElementAt((int)Math.Floor(quater));
                q3 = sortedValues.ElementAt((int)Math.Floor(quater) + (int)Math.Floor(half));
            }

            if (q1 == mediana)
            {
                q1--;
            }

            if (q3 == mediana)
            {
                q3++;
            }
            double a = q1 - iqrMultiplier * (q3 - q1);
            double b = q3 + iqrMultiplier * (q3 - q1);

            //if (sortedValues.Count(v => v < a || v > b) > 0)
            //{
            //    Console.WriteLine($"{Name} => [OC:{sortedValues.Count(v => v < a || v > b)}] F:{sortedValues.First()} L:{sortedValues.Last()} M:{mediana} Q1:{q1} Q3:{q3} A:{a} B:{b}");
            //}
            return Tuple.Create(Name, a, b);
        }

        public IList<string> GetOutliers()
        {
            var iqr = IQR(2);
            return Values.Where(v => double.Parse(v.NewValue) < iqr.Item2 || double.Parse(v.NewValue) > iqr.Item3)
                .Select(v => v.RowId).ToList();
        }

        public void MarkOutliers()
        {
            var iqr = IQR(2);
            var values = Values.Where(v => double.Parse(v.NewValue) < iqr.Item2 || double.Parse(v.NewValue) > iqr.Item3);
            //if (values.Count() > 0)
            //{
            //    Console.WriteLine($"{iqr.Item1} x=> {values.Count()}");
            //}
            foreach (var value in values)
            {
                value.IsOutlier = true;
            }
        }
    }
}