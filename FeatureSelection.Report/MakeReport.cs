using OxyPlot;
using OxyPlot.Pdf;
using OxyPlot.Reporting;
using OxyPlot.Series;
using System.Collections.Generic;

namespace FeatureSelection.Reporting
{
    public class MakeReport
    {
        private List<PlotModel> plotList = new List<PlotModel>();

        public void CreatePDF(string fileName)
        {
            using (var w = new PdfReportWriter(fileName))
            {
                w.WriteReport(CreateReport(), new ReportStyle());
            }
        }

        private Report CreateReport()
        {
            var r = new Report();
            plotList.ForEach(p => r.AddPlot(p, p.Title, 400, 250));
            return r;
        }

        public void AddStepPlot(string title, List<Point> points)
        {
            var plot = new PlotModel() { Title = title };
            StairStepSeries series = new StairStepSeries();
            points.ForEach(p => series.Points.Add(new DataPoint(p.X, p.Y)));
            plot.Series.Add(series);
            plotList.Add(plot);
        }

        public void AddLinePlot(string title, List<Point> points)
        {
            var plot = new PlotModel() { Title = title };
            LineSeries series = new LineSeries();
            points.ForEach(p => series.Points.Add(new DataPoint(p.X, p.Y)));
            plot.Series.Add(series);
            plotList.Add(plot);
        }

        public void AddScatterPlot(string title, List<Point> points)
        {
            var plot = new PlotModel() { Title = title };
            ScatterSeries series = new ScatterSeries();
            points.ForEach(p => series.Points.Add(new ScatterPoint(p.X, p.Y)));
            plot.Series.Add(series);
            plotList.Add(plot);
        }
    }
}