using System;
using System.Collections.Generic;
using System.Linq;

namespace FeatureSelection.Library
{
    public class DataProcessing
    {
        public IList<RowModel> TrainRows { get; }
        public IList<RowModel> TestRows { get; }
        public IList<FeatureModel> Features { get; }
        public int TrainRowAmount { get; set; }
        public int TestRowAmount { get; set; }

        public DataProcessing(IList<IList<string>> train, IList<IList<string>> test, string className = null, string IdName = null)
        {
            if (train == null)
            {
                throw new ArgumentNullException(nameof(train));
            }

            Features = train.First().Select((name, index) => new FeatureModel(index, name)).ToList();

            if (!string.IsNullOrEmpty(className))
            {
                Features.Single(f => f.Name == className).IsClass = true;
            }

            if (!string.IsNullOrEmpty(IdName))
            {
                Features.Single(f => f.Name == IdName).IsId = true;
            }

            TrainRowAmount = train.Count() - 1;
            TestRowAmount = test.Count() - 1;

            var IdFeature = Features.Single(f => f.IsId);

            TrainRows = train.Skip(1)
            .Select(
                (r, index) =>
                {
                    var id = r.ElementAt(IdFeature.Index);
                    return new RowModel(index, id);
                }
            )
            .ToList();

            TestRows = test.Skip(1)
            .Select(
                (r, index) =>
                {
                    var id = r.ElementAt(IdFeature.Index);
                    return new RowModel(index, id);
                }
            )
            .ToList();

            foreach (var feature in Features)
            {
                foreach (var row in TrainRows)
                {
                    var value = new ValueModel(row.Id, feature, train.ElementAt(row.Index + 1).ElementAt(feature.Index), false);
                    feature.Values.Add(value);
                    row.Values.Add(value);
                }

                if (!feature.IsClass)
                {
                    foreach (var row in TestRows)
                    {
                        var value = new ValueModel(row.Id, feature, test.ElementAt(row.Index + 1).ElementAt(feature.Index), true);
                        feature.Values.Add(value);
                        row.Values.Add(value);
                    }
                }
            }

            //foreach (var feature in Features)
            //{
            //    feature.Values = train.Skip(1).Select(t => new ValueModel(t.ElementAt(feature.Index), isTest: false)).ToList();

            //    if (!feature.IsClass)
            //    {
            //        var testValues = test.Skip(1).Select(t => new ValueModel(t.ElementAt(feature.Index), isTest: true)).ToList();
            //        feature.Values = feature.Values.Concat(testValues).ToList();
            //    }
            //}
        }

        public DataModel GetDataModel()
        {
            return new DataModel()
            {
                HeadersWithClass = GetHeaders(withClass: true),
                HeadersWithoutClass = GetHeaders(withClass: false),
                OutputTrain = GetTransfomedTrain(),
                OutputTest = GetTransfomedTest(),
                TestIds = GetTestIds()
            };
        }

        public void SetInactive(IEnumerable<string> featureNames)
        {
            featureNames.ToList()
                .ForEach(n => Features.FirstOrDefault(f => f.Name == n).IsActive = false);
        }

        public void SetTrainRowsInactive(IEnumerable<int> rowIndexes)
        {
            rowIndexes.ToList()
                .ForEach(i => TrainRows.FirstOrDefault(r => r.Index == i).IsActive = false);
        }

        public void SetTrainRowsInactiveByIds(IEnumerable<string> rowIds)
        {
            rowIds.ToList()
                .ForEach(i => TrainRows.FirstOrDefault(r => r.Id == i).IsActive = false);
        }

        public void PrintOutliersAmount()
        {
            TrainRows.Where(r => r.Values.Sum(v => v.IsOutlier ? 1 : 0) > 0)
                .OrderByDescending(r => r.Values.Sum(v => v.IsOutlier ? 1 : 0))
                .Select(r => $"{r.Id} => {string.Join(", ", r.Values.Where(v => v.IsOutlier).Select(v => v.Feature.Name))} {r.Values.Sum(v => v.IsOutlier ? 1 : 0)}")
                .ToList()
                .ForEach(r => Console.WriteLine(r));
        }

        public string GetClassForTrainId(string id)
        {
            return TrainRows.FirstOrDefault(r => r.Id == id)?.Values.FirstOrDefault(v => v.Feature.IsClass)?.NewValue;
        }

        public FeatureModel GetFeature(string name)
        {
            return Features.First(f => f.Name == name);
        }

        //public IEnumerable<string> GetRow(int index, bool takeTest, bool withClass)
        //{
        //    foreach (var feature in Features.Where(f => f.IsActive && (!f.IsClass || withClass)))
        //    {
        //        yield return feature.Values.Where(v => v.IsTest == takeTest).ElementAt(index).NewValue;
        //    }
        //}

        public IEnumerable<IEnumerable<string>> GetTransfomedTest()
        {
            return TestRows.Where(r => r.IsActive).Select(r => r.Values.Where(v => v.Feature.IsActive && !v.Feature.IsClass).Select(v => v.NewValue));
            //return Enumerable.Range(0, TestRowAmount).Select(index => GetRow(index, takeTest: true, withClass: false));
        }

        public IEnumerable<string> GetTestIds()
        {
            return Features.Where(f => f.IsId).FirstOrDefault()?.Values?.Where(v => v.IsTest).Select(v => v.NewValue);
        }

        public IEnumerable<string> GetTrainIds()
        {
            return Features.Where(f => f.IsId).FirstOrDefault()?.Values?.Where(v => !v.IsTest).Select(v => v.NewValue);
        }

        public IEnumerable<IEnumerable<string>> GetTransfomedTrain()
        {
            return TrainRows.Where(r => r.IsActive).Select(r => r.Values.Where(v => v.Feature.IsActive).Select(v => v.NewValue));
            //return Enumerable.Range(0, TrainRowAmount).Select(index => GetRow(index, takeTest: false, withClass: true));
        }

        public IEnumerable<string> GetHeaders(bool withClass)
        {
            return Features.Where(f => f.IsActive && (f.IsClass == false || withClass)).Select(f => f.Name);
        }

        public IEnumerable<string> GetFeatureDetails()
        {
            foreach (var feature in Features)
            {
                yield return ($"{feature.Name}, C:{feature.IsClass}, A:{feature.IsActive}, U:{feature.GetUniqueAmount()}, N:{feature.IsNumeric()}, I:{feature.IsInteger()}");
            }
        }
    }
}