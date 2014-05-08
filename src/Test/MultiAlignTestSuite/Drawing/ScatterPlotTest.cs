using System.Collections.Generic;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.IO.Features;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PNNLOmics.Algorithms;
using PNNLOmics.Algorithms.FeatureClustering;
using PNNLOmics.Annotations;
using System;
using System.Xml;

namespace MultiAlignTestSuite.Drawing.Graphics
{
    [TestFixture]
    [UsedImplicitly]
    public class ScatterPlotTest
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
                var plotModel1 = new PlotModel { Subtitle = "No 'binning'", Title = "ScatterSeries (n=32768)" };

                var linearAxis1 = new LinearAxis { Position = AxisPosition.Bottom };
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
                Console.WriteLine("Creation Part of Test Took: {0:.00} seconds for {1} points", end.Subtract(start).TotalSeconds, totalPoints);

                start = DateTime.Now;
                scatterSeries1.Points.AddRange(pointList);
                plotModel1.Series.Add(scatterSeries1);
                end = DateTime.Now;
                Console.WriteLine("ScCatter Plot Part of Test Took: {0:.00} seconds for {1} points", end.Subtract(start).TotalSeconds, totalPoints);

                start = DateTime.Now;
                var svg = new SvgExporter();
                var svgString = svg.ExportToString(plotModel1);

                var xml = new XmlDocument();
                xml.LoadXml(svgString);
                var x = Svg.SvgDocument.Open(xml); // Svg.SvgDocument();            
                var bmp = x.Draw();
                bmp.Save(string.Format(@"m:\testScatter{0}.jpg", totalPoints));

                end = DateTime.Now;
                Console.WriteLine("Saving Part of Test Took: {0:.00} seconds for {1} points", end.Subtract(start).TotalSeconds, totalPoints);
            }


        }

        [Test]
        [UsedImplicitly]
        [TestCase(
            @"M:\data\proteomics\TestData\QC-Shew-Annotated2\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14_isos.csv",
            @"M:\scatter.png"
            )        
        ]
        public void TestMsFeatureScatterPlot(string path1, string path2, string svgPath)
        {
            var aligner = new  MultiAlignCore.Algorithms.Alignment.LcmsWarpFeatureAligner();

            var baselineMs    = UmcLoaderFactory.LoadMsFeatureData(path1);
            var aligneeMs     = UmcLoaderFactory.LoadMsFeatureData(path2);
            var finder        = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);

            var tolerances  = new FeatureTolerances
            {
                FragmentationWindowSize = .5,
                Mass            = 13,
                DriftTime       = .3,
                RetentionTime   = .01
            };
            var options     = new LcmsFeatureFindingOptions(tolerances);
            options.MaximumNetRange = .002;

            var baseline    = finder.FindFeatures(baselineMs, options, null);
            var alignee     = finder.FindFeatures(aligneeMs, options, null);
            var data        = aligner.Align(baseline, alignee);

            var plotModel1 = new PlotModel
            {                
                Subtitle = "Interpolated, cartesian axes",
                Title = "HeatMapSeries"
            };

            var palette = OxyPalettes.Hot(200);
            var linearColorAxis1 = new LinearColorAxis
            {                
                InvalidNumberColor = OxyColors.Gray,                
                Position  = AxisPosition.Right,             
                Palette = palette
            };
            plotModel1.Axes.Add(linearColorAxis1);
            

           // linearColorAxis1.

            var linearAxis1 = new LinearAxis { Position = AxisPosition.Bottom };
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

            var scores = data.heatScores;
            var width  = scores.GetLength(0);
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
            var x = Svg.SvgDocument.Open(xml); // Svg.SvgDocument();            
            var bmp = x.Draw();
            bmp.Save(svgPath);
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
            
           
