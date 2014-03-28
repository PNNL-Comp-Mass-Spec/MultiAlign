using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PNNLOmics.Data.Features;
using System.IO;
using PNNLOmics.Algorithms.Distance;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Algorithms.SpectralComparisons;
using PNNLOmics.Algorithms.SpectralProcessing;
using PNNLOmics.Data;
using PNNLOmicsIO.IO;
using MultiAlignCore.IO.Features;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Windows.Forms;
using System.Drawing;
using MultiAlignTestSuite.Papers.Alignment.SSM;


namespace MultiAlignTestSuite.Algorithms
{

    [TestFixture]
    public class ClusterComparer : BaseSpectralAlignmentTest
    {

        private MSSpectra ReadSpectrum(string path)
        {
            MSSpectra spectrum  = new MSSpectra();
            string[] lines      = File.ReadAllLines(path);

            spectrum.Peaks = new List<XYData>();
            foreach (string line in lines)
            {
                string[] data = line.Split('\t');
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
            for (int j = 0; j < peaksY.Count; j++)
            {
                XYData peakX = peaksX[j];
                XYData peakY = peaksY[j];

                double value = 0;
                if (peakX.Y > 0 && peakY.Y > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;
            xseries.Color.ChangeAlpha(100);
            //plotModel1.Series.Add(xseries);

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (XYData datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (XYData datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y / max));
            }
            plotModel1.Series.Add(series);

            foreach (XYData datum in peaksY)
            {
                max = Math.Max(max, datum.Y);
            }
            var series2 = new StemSeries();
            series2.Title = "Spectra Y";
            foreach (XYData datum in peaksY)
            {
                series2.Points.Add(new DataPoint(datum.X, (datum.Y * -1) / max));
            }
            plotModel1.Series.Add(series2);


            return plotModel1;
        }

        private void DisplayComparisonPlot(MSSpectra spectrumX, MSSpectra spectrumY, double mzTolerance, string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            PlotModel model = CreatePlot(spectrumX.Peaks, spectrumY.Peaks, mzTolerance);
            model.Title = newTitle;

            Plot plot = new Plot();
            plot.Model = model;
            Form form = new Form();
            form.Size = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(plot);
            form.ShowDialog();

            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
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
            for (int j = 0; j < peaksX.Count; j++)
            {
                XYData peakX = peaksX[j];

                double value = 0;
                if (peakX.Y  > 0)
                {
                    value = 1;
                }
                xseries.Points.Add(new DataPoint(peakX.X, value));
            }
            xseries.Color = OxyColors.Green;
            xseries.Color.ChangeAlpha(100);
            //plotModel1.Series.Add(xseries);

            var series = new StemSeries();
            series.Title = "Spectra X";
            double max = 0;
            foreach (XYData datum in peaksX)
            {
                max = Math.Max(max, datum.Y);
            }
            foreach (XYData datum in peaksX)
            {
                series.Points.Add(new DataPoint(datum.X, datum.Y / max));
            }
            plotModel1.Series.Add(series);


            return plotModel1;
        }

        private void DisplaySpectra(MSSpectra spectrumX, double mzTolerance, string path = "comparison.png", string newTitle = "MS/MS Spectra")
        {
            PlotModel model = CreatePlot(spectrumX.Peaks, mzTolerance);
            model.Title = newTitle;

            Plot plot = new Plot();
            plot.Model = model;
            Form form = new Form();
            form.Size = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            plot.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(plot);
            form.ShowDialog();

            using (Bitmap bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, form.DisplayRectangle);
                bitmap.Save(path);
            }
        }
       
        [Test]
        [TestCase(@"m:\clusters\99665", SpectralComparison.CosineDotProduct, .8)]
        public void Create(string directory, SpectralComparison comparerType, double percent)
        {
            ISpectralComparer comparer = SpectralComparerFactory.CreateSpectraComparer(comparerType, percent: percent);

            List<MSSpectra> spectra = new List<MSSpectra>();
            string[] files          = Directory.GetFiles(directory, "*.txt");
            
            foreach (string file in files)
            {
                spectra.Add(ReadSpectrum(file));
            }

            List<double> values = new List<double>();
            for (int i = 0; i < spectra.Count; i++)
            {
                //for (int j = i + 1; j < spectra.Count; j++) 
                {
                    //double value = comparer.CompareSpectra(spectra[i], spectra[j]);
                    //values.Add(value);

                    Console.WriteLine("{0}", Path.GetFileNameWithoutExtension(files[i]));
                     
                       //                             Path.GetFileNameWithoutExtension(files[j]));
                    DisplaySpectra(spectra[i], .15, newTitle:"MS/MS Spectra - ");
                }
            }

            string baseName = Path.GetFileName(directory);
           // Console.WriteLine("{0} - {1}", baseName, values.Average());
           // values.ForEach(x => Console.WriteLine("\t{0}", x));
        }
    }
}
