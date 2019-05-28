using FeatureSelection.Library;
using FeatureSelection.Reporting;
using LibSVMsharp;
using LibSVMsharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FeatureSelection.Console
{
    internal class OutlierLine
    {
        public string FeatureName { get; set; }
        public string Outliers { get; set; }
    }

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var report = new MakeReport();
            List<string> consoleOutput = new List<string>();

            consoleOutput.Add($"{System.DateTime.Now} - Loading");

            var train = ConvertHelper.CSVToDataConstrutor(File.ReadAllLines("train.csv"), ',');
            var test = ConvertHelper.CSVToDataConstrutor(File.ReadAllLines("test.csv"), ',');

            var fs = new DataProcessing(train, test, "SalePrice", "Id");

            fs.GetFeature("SalePrice").Transform((value) => Math.Log(1 + value));

            fs.SetInactive(new List<string> {"Id","PoolArea", "LandContour","PoolQC", "LotConfig","Utilities","Alley",
             "Street",  "BsmtHalfBath", "LowQualFinSF", "3SsnPorch", "LandSlope", "YrSold", "Condition1", "BsmtFinType2", "RoofMatl",
             "MiscVal", "MiscFeature", "BsmtFinSF2", "Condition2",  "BldgType", "ScreenPorch", "MoSold", "Functional" });

            fs.SetInactive(new List<string> { "BsmtCond", "BsmtUnfSF", "GarageCars", "PavedDrive", "SaleType", "SaleCondition",
                "BsmtExposure", "GarageCond", "Fence", "Heating", "BsmtQual",
            });

            //fs.SetInactive(new List<string> { "EnclosedPorch" });

            fs.SetTrainRowsInactiveByIds(new List<string> { "1299", "186", "198", "636", "1032", "1183", "1153", "1174" });
            //fs.SetTrainRowsInactiveByIds(new List<string> { "130", "188", "199", "268", "305", "497", "524", "530", "692", "770", "884", "1025", "1231", "1371", "1387", "1424", "1441" });
            consoleOutput.Add($"{System.DateTime.Now} - Transforming model");

            fs.GetFeature("LotFrontage").ReplaceValues("NA", "0");
            fs.GetFeature("MasVnrArea").ReplaceValues("NA", "-1");
            fs.GetFeature("GarageYrBlt").ReplaceValues("NA", "-1");

            fs.Features.Where(f => !f.IsNumeric() && !f.IsClass).All(n => { n.TransformEnumToInt(); return true; });
            //fs.Features.Select(f => new OutlierLine { FeatureName = f.Name, Outliers = string.Join(", ", f.GetOutliers()) }).ToList().ForEach(o => consoleOutput.Add($"{o.FeatureName} => {o.Outliers}"));
            //TODO: To jakoś nie polepszyło, a nawet pogorszyło, trzeba by dodać taką funkcję zamiast wyliczać to tutaj i ująć zmienną GarageYrBlt
            //var oldestYearBuild = fs.GetFeature("YearBuilt").Values.Min(v => double.Parse(v.NewValue));
            //fs.GetFeature("YearBuilt").Transform((value) => (double.Parse(value) - oldestYearBuild).ToString());

            //fs.Features.ToList().ForEach(f => report.AddScatterPlot(f.Name, f.Values.Where(v => !v.IsTest).Select(v => new Point() { X = double.Parse(v.NewValue), Y = double.Parse(fs.GetClassForTrainId(v.RowId)) }).ToList()));
            //report.CreatePDF("Report.pdf");

            //var oldestYearRemodAdd = fs.GetFeature("YearRemodAdd").Values.Min(v => double.Parse(v.NewValue));
            //fs.GetFeature("YearRemodAdd").Transform((value) => (double.Parse(value) - oldestYearRemodAdd).ToString());

            //OUTLIERS
            //foreach (var feature in fs.Features.Where(f => f.IsActive && !f.IsClass && !f.IsId && new List<string> { "EnclosedPorch", "BsmtFinSF2", "GarageYrBlt", "OpenPorchSF", "ScreenPorch", "MasVnrArea", "LotArea", "Condition1", "MSSubClass", "MiscVal" }.IndexOf(f.Name) < 0))
            //{
            //    feature.MarkOutliers();
            //}
            //fs.PrintOutliersAmount();
            consoleOutput.Add($"{System.DateTime.Now} - Gathering data");

            var dataModel = fs.GetDataModel();

            File.WriteAllLines("output-train.csv", ConvertHelper.DataSetToCSV(dataModel.HeadersWithClass, dataModel.OutputTrain, ","));
            File.WriteAllLines("output-test.csv", ConvertHelper.DataSetToCSV(dataModel.HeadersWithoutClass, dataModel.OutputTest, ","));

            consoleOutput.Add($"{System.DateTime.Now} - Preparing SVM problem");

            //if (false)
            {
                //SVM.SetPrintStringFunction(null);
                SVMProblem testSet = SVMLoadHelper.Load(dataModel.OutputTest, isWithClass: false);
                SVMProblem trainingSet = SVMLoadHelper.Load(dataModel.OutputTrain, isWithClass: true);

                SVMParameter parameter = new SVMParameter()
                {
                    Type = SVMType.EPSILON_SVR,
                    Kernel = SVMKernelType.RBF,
                    //C = 10,
                    Gamma = 0.01,
                    CacheSize = 2000,
                    Eps = 0.1,// * Math.Pow(10, i);
                    //parameter.Probability = true;
                };

                //List<Tuple<string, double>> rmseAfterRemoveFeature = new List<Tuple<string, double>>();
                //var activeFeatures = fs.Features.Where(f => f.IsActive || !f.IsClass);
                //foreach (var feature in activeFeatures)
                //{
                //    feature.IsActive = false;
                //    var outputTrain = fs.GetTransfomedTrain();
                //    SVMProblem trainingSet = SVMLoadHelper.Load(outputTrain, isWithClass: true);

                //    consoleOutput.Add("=====================");
                //    //SVMModel model = trainingSet.Train(parameter);
                //    //double[] trainingResults = trainingSet.Predict(model);
                //    double[] crossvalidationResults;
                //    trainingSet.CrossValidation(parameter, 3, out crossvalidationResults);
                //    //consoleOutput.Add(parameter.GetOutput());
                //    var rmselog = EvaulationHelper.RMSELog(trainingSet.Y.ToArray(), crossvalidationResults);
                //    rmseAfterRemoveFeature.Add(new Tuple<string, double>(feature.Name, rmselog));
                //    consoleOutput.Add($"{System.DateTime.Now} - {feature.Name} - {rmselog}");
                //    feature.IsActive = true;
                //}

                //consoleOutput.Add($"{System.DateTime.Now} - {bestToRemove.Item1} - {bestToRemove.Item2}");
                consoleOutput.Add("=====================");
                consoleOutput.Add("Ordered");
                //consoleOutput.AddRange(rmseAfterRemoveFeature.OrderBy(t => t.Item2).Select(t => $"{t.Item1} - {t.Item2}"));
                SVMModel model = trainingSet.Train(parameter);
                double[] trainResults = trainingSet.Predict(model);
                double[] testResults = testSet.Predict(model);

                //double meanSquaredErr = testSet.EvaluateRegressionProblem(testResults, out correlationCoef);

                trainingSet.Y
                    .Select((y, index) => Math.Abs(y - trainResults[index]))
                    .Select((v, index) => new { index, v })
                    .OrderByDescending(v => v.v)
                    .Take(15)
                    .ToList()
                    .ForEach(e => consoleOutput.Add($"{e.index}:{e.v}"));

                var rmselog = EvaulationHelper.RMSELog(trainingSet.Y.ToArray(), trainResults);
                consoleOutput.Add($"{System.DateTime.Now} - {rmselog}");

                File.WriteAllLines("submission_fs.csv", ConvertHelper.ResultToCSV("Id,SalePrice", testResults.Select(v => Math.Exp(v) - 1).ToArray(), dataModel.TestIds));
                consoleOutput.Add($"{System.DateTime.Now} -I had finish");
                SVM.SaveModel(model, "model.txt");
            }

            //report.CreatePDF("Report.pdf");
            consoleOutput.ForEach(s => System.Console.WriteLine(s));
            File.WriteAllLines("consoleOutput.txt", consoleOutput);

            System.Console.ReadLine();
        }
    }
}