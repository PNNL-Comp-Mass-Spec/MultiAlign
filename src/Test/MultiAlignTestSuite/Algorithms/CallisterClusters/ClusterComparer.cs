﻿#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MultiAlignCore.Algorithms.SpectralProcessing;
using MultiAlignCore.Data;
using MultiAlignTestSuite.Papers.Alignment.SSM;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using LinearAxis = OxyPlot.Axes.LinearAxis;

#endregion

namespace MultiAlignTestSuite.Algorithms.CallisterClusters
{
    [TestFixture]
    public class ClusterComparer : BaseSpectralAlignmentTest
    {
        private MSSpectra ReadSpectrum(string path)
        {
            var spectrum = new MSSpectra();
            var lines = File.ReadAllLines(path);

            spectrum.Peaks = new List<XYData>();
            foreach (var line in lines)
            {
                var data = line.Split('\t');
                if (data.Length > 1)
                {
                    spectrum.Peaks.Add(new XYData(Convert.ToDouble(data[0]),
                        Convert.ToDouble(data[1])));
                }
            }
            spectrum.Peaks = XYData.Bin(spectrum.Peaks, 0, 2000, .15);
            return spectrum;
        }

        private PlotModel CreatePlot(List<XYData> peaksX,
            List<XYData> peaksY,
            double tolerance)
        {
            var plotModel1 = new PlotModel();
            plotModel1.LegendBorderThickness = 0;
            plotModel1.LegendOrientation = LegendOrientation.Horizontal;
            plotModel1.LegendPlacement = LegendPlacement.Outside;
            plotModel1.LegendPosition = LegendPosition.BottomCenter;
            plotModel1.Title = "MS/MS Spectra";

            var categoryAxis1 = new LinearAxis();
            categoryAxis1.MinorStep = tolerance;
            plotModel1.Axes.Add(categoryAxis1);

            var linearAxis1 = new LinearAxis();
            linearAxis1.MaximumPadding = 0.06;
            linearAxis1.MinimumPadding = 0.06;
            plotModel1.Axes.Add(linearAxis1);

            var xseries = new StemSeries();
            for (var j = 0; j < peaksY.Count; j++)
            {
                var peakX = peaksX[j];
                var peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (var datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (var datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y/max));
            }
            plotModel1.Series.Add(series);

            foreach (var datum in peaksY)
            {
                max = Math.Max(max, datum.Y);
            }
            var series2 = new StemSeries();
            series2.Title = "Spectra Y";
            foreach (var datum in peaksY)
            {
                series2.Points.Add(new DataPoint(datum.X, (datum.Y*-1)/max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance,
            string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            var model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            var plot = new PlotView();
            plot.Model = model;
            var form = new Form();
            form.Size = Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = DockStyle.Fill;
            form.Controls.Add(plot);
            form.Show();

            IO.Utilities.SleepNow(3);

            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.DisplayRectangle);
                bitmap.Save(path);
            }
        }


        private PlotModel CreatePlot(List<XYData> peaksX,
            double tolerance)
        {
            var plotModel1 = new PlotModel();
            plotModel1.LegendBorderThickness = 0;
            plotModel1.LegendOrientation = LegendOrientation.Horizontal;
            plotModel1.LegendPlacement = LegendPlacement.Outside;
            plotModel1.LegendPosition = LegendPosition.BottomCenter;
            plotModel1.Title = "MS/MS Spectra";

            var categoryAxis1 = new LinearAxis();
            categoryAxis1.MinorStep = tolerance;
            plotModel1.Axes.Add(categoryAxis1);

            var linearAxis1 = new LinearAxis();
            linearAxis1.MaximumPadding = 0.06;
            linearAxis1.MinimumPadding = 0.06;
            plotModel1.Axes.Add(linearAxis1);

            var xseries = new StemSeries();
            for (var j = 0; j < peaksX.Count; j++)
            {
                var peakX = peaksX[j];

                double value = 0;
                if (peakX.Y > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (var datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (var datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y/max));
            }
            plotModel1.Series.Add(series);


            return plotModel1;
        }

        private void DisplaySpectra(MSSpectra spectrumX, double mzTolerance, string path = "comparison.png",
            string newTitle = "MS/MS Spectra")
        {
            var model = CreatePlot(spectrumX.Peaks, mzTolerance);
            model.Title = newTitle;

            var plot = new PlotView();
            plot.Model = model;
            var form = new Form();
            form.Size = Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = DockStyle.Fill;
            form.Controls.Add(plot);
            form.Show();

            IO.Utilities.SleepNow(3);

            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.DisplayRectangle);
                bitmap.Save(path);
            }
        }

        [Test]
        [TestCase(@"m:\clusters\99665", SpectralComparison.CosineDotProduct, .8, Ignore=true)]
        public void Create(string directory, SpectralComparison comparerType, double percent)
        {
            var comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent);

            var spectra = new List<MSSpectra>();
            var files = Directory.GetFiles(directory, "*.txt");

            foreach (var file in files)
            {
                spectra.Add(ReadSpectrum(file));
            }

            var values = new List<double>();
            for (var i = 0; i < spectra.Count; i++)
            {
                //for (int j = i + 1; j < spectra.Count; j++)
                {
                    //double value = comparer.CompareSpectra(spectra[i], spectra[j]);
                    //values.Add(value);

                    Console.WriteLine("{0}", Path.GetFileNameWithoutExtension(files[i]));

                    //                             Path.GetFileNameWithoutExtension(files[j]));
                    DisplaySpectra(spectra[i], .15, newTitle: "MS/MS Spectra - ");
                }
            }

            var baseName = Path.GetFileName(directory);
            // Console.WriteLine("{0} - {1}", baseName, values.Average());
            // values.ForEach(x => Console.WriteLine("\t{0}", x));
        }
    }
}