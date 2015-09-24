#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Alignment;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Drawing;
using MultiAlignCore.IO.Features;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Annotations;
using Svg;

#endregion

namespace MultiAlignTestSuite.Drawing
{
    [TestFixture]
    [UsedImplicitly]
    public class ScatterPlotTest: TestBase
    {
        [Test]
        [UsedImplicitly]
        public void TestSimpleCreation()
        {
            var random = new Random();
            var points = new List<int> {100, 1000, 10000, 100000};

            foreach (var totalPoints in points)
            {
                var start = DateTime.Now;
                var plotModel1 = new PlotModel {Subtitle = "No 'binning'", Title = "ScatterSeries (n=32768)"};

                var linearAxis1 = new LinearAxis {Position = AxisPosition.Bottom};
                plotModel1.Axes.Add(linearAxis1);

                var linearAxis2 = new LinearAxis();
                plotModel1.Axes.Add(linearAxis2);

                var scatterSeries1 = new ScatterSeries
                {
                    MarkerSize = 1,
                    MarkerStrokeThickness = 0,
                    MarkerType = MarkerType.Diamond,
                    Title = "Series 1" + totalPoints
                };

                var pointList = new List<ScatterPoint>();
                for (var i = 0; i < totalPoints; i++)
                {
                    var point = new ScatterPoint
                    {
                        X = random.NextDouble(),
                        Y = random.NextDouble()
                    };
                    pointList.Add(point);
                }
                Console.WriteLine();

                var end = DateTime.Now;
                Console.WriteLine("Creation Part of Test Took: {0:.00} seconds for {1} points",
                    end.Subtract(start).TotalSeconds, totalPoints);

                start = DateTime.Now;
                scatterSeries1.Points.AddRange(pointList);
                plotModel1.Series.Add(scatterSeries1);
                end = DateTime.Now;
                Console.WriteLine("Scatter Plot Part of Test Took: {0:.00} seconds for {1} points",
                    end.Subtract(start).TotalSeconds, totalPoints);

                start = DateTime.Now;
                var svg = new SvgExporter();
                var svgString = svg.ExportToString(plotModel1);

                var xml = new XmlDocument();
                xml.LoadXml(svgString);
                var x = SvgDocument.Open(xml); // Svg.SvgDocument();            
                var bmp = x.Draw();
                var outputFilePath = GetPath(@"testResults\ScatterPlot\testScatter" + totalPoints + ".jpg");
                bmp.Save(outputFilePath);

                end = DateTime.Now;
                Console.WriteLine("Saving Part of Test Took: {0:.00} seconds for {1} points",
                    end.Subtract(start).TotalSeconds, totalPoints);
            }
        }

        [Test]
        [UsedImplicitly]
        [TestCase(
            @"Data\QC_Shew\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14_isos.csv",
            @"Data\QC_Shew\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14_isos.csv",
            @"testResults\ScatterPlot\QC_Shew_13_04_1b_scatter.png"
            )
        ]
        public void TestMsFeatureScatterPlot(string path1, string path2, string pngPath)
        {
            // Convert relative paths to absolute paths
            path1 = GetPath(path1);
            path2 = GetPath(path2);
            pngPath = GetPath(pngPath);

            var fiOutput = new FileInfo(pngPath);
            var didirectory = fiOutput.Directory;
            if (didirectory == null)
                throw new DirectoryNotFoundException(pngPath);

            if (!didirectory.Exists)
                didirectory.Create();

            var aligner = new LcmsWarpFeatureAligner();

            var baselineMs = UmcLoaderFactory.LoadMsFeatureData(path1);
            var aligneeMs = UmcLoaderFactory.LoadMsFeatureData(path2);
            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);

            var tolerances = new FeatureTolerances
            {
                FragmentationWindowSize = .5,
                Mass = 13,
                DriftTime = .3,
                Net = .01
            };
            var options = new LcmsFeatureFindingOptions(tolerances);
            options.MaximumNetRange = .002;

            var baseline = finder.FindFeatures(baselineMs, options, null);
            var alignee = finder.FindFeatures(aligneeMs, options, null);
            var alignmentResults = aligner.Align(baseline, alignee);

            var plotModel1 = new PlotModel
            {
                Subtitle = "Interpolated, cartesian axes",
                Title = "HeatMapSeries"
            };

            var palette = OxyPalettes.Hot(200);
            var linearColorAxis1 = new LinearColorAxis
            {
                InvalidNumberColor = OxyColors.Gray,
                Position = AxisPosition.Right,
                Palette = palette
            };
            plotModel1.Axes.Add(linearColorAxis1);


            // linearColorAxis1.

            var linearAxis1 = new LinearAxis {Position = AxisPosition.Bottom};
            plotModel1.Axes.Add(linearAxis1);

            var linearAxis2 = new LinearAxis();
            plotModel1.Axes.Add(linearAxis2);

            var heatMapSeries1 = new HeatMapSeries
            {
                X0 = 0,
                X1 = 1,
                Y0 = 0,
                Y1 = 1,
                FontSize = .2
            };

            var scores = alignmentResults.HeatScores;
            var width = scores.GetLength(0);
            var height = scores.GetLength(1);

            heatMapSeries1.Data = new double[width, height];

            var seriesData = heatMapSeries1.Data;
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    seriesData[i, j] = Convert.ToDouble(scores[i, j]);
                }
            }

            plotModel1.Series.Add(heatMapSeries1);

            var svg = new SvgExporter();
            var svgString = svg.ExportToString(plotModel1);

            var xml = new XmlDocument();
            xml.LoadXml(svgString);
            var x = SvgDocument.Open(xml); // Svg.SvgDocument();            
            var bmp = x.Draw();

            bmp.Save(pngPath);


            var heatmap = HeatmapFactory.CreateAlignedHeatmap(alignmentResults.HeatScores, false);
            var netHistogram = HistogramFactory.CreateHistogram(alignmentResults.NetErrorHistogram, "NET Error", "NET Error");
            var massHistogram = HistogramFactory.CreateHistogram(alignmentResults.MassErrorHistogram, "Mass Error", "Mass Error (ppm)");          

            var baseName = Path.Combine(didirectory.FullName, Path.GetFileNameWithoutExtension(fiOutput.Name));

            var encoder = new SvgEncoder();
            PlotImageUtility.SaveImage(heatmap, baseName + "_heatmap.svg", encoder);
            PlotImageUtility.SaveImage(netHistogram, baseName + "_netHistogram.svg", encoder);
            PlotImageUtility.SaveImage(massHistogram, baseName + "_massHistogram.svg", encoder);


        }
    }
}

//        [Example("Peaks")]
//        public static PlotModel Peaks()
//        {
//            var plotModel1 = new PlotModel();
//            plotModel1.Title = "Peaks";

//var linearColorAxis1 = new LinearColorAxis();
//            linearColorAxis1.HighColor = OxyColors.Gray;
//            linearColorAxis1.LowColor = OxyColors.Black;
//            linearColorAxis1.Position = AxisPosition.Right;

//plotModel1.Axes.Add(linearColorAxis1);

//var linearAxis1 = new LinearAxis();

//linearAxis1.Position = AxisPosition.Bottom;
//            plotModel1.Axes.Add(linearAxis1);

//var linearAxis2 = new LinearAxis();
//            plotModel1.Axes.Add(linearAxis2);