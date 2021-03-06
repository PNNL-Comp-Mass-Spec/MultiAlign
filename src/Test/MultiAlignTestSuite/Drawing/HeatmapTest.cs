﻿#region

using System;
using System.IO;
using System.Xml;
using FeatureAlignment.Algorithms.Alignment.LcmsWarp;
using MultiAlignCore;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Algorithms.Clustering;
using MultiAlignCore.Algorithms.FeatureFinding;
using MultiAlignCore.Drawing;
using MultiAlignCore.IO.Features;
using MultiAlignCore.IO.TextFiles;
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
    public class HeatmapTest: TestBase
    {
        const string HEATMAP_RESULTS_FOLDER_BASE = @"testResults\HeatMap\";

        [Test]
        [UsedImplicitly]
        public void TestSimpleCreation()
        {

            var plotModel1 = new PlotModel
            {
                PlotType = PlotType.Cartesian,
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

            var linearAxis1 = new LinearAxis {Position = AxisPosition.Bottom};
            plotModel1.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            plotModel1.Axes.Add(linearAxis2);
            var heatMapSeries1 = new HeatMapSeries
            {
                X0 = 0.0,
                X1 = 1.0,
                Y0 = 0.0,
                Y1 = 1.0,
                FontSize = .2,
                Data = new Double[2, 3]
            };
            //heatMapSeries1.LabelFontSize = 0.2;
            heatMapSeries1.Data[0, 0] = 0;
            heatMapSeries1.Data[0, 1] = 10.2;
            heatMapSeries1.Data[0, 2] = 20.4;
            heatMapSeries1.Data[1, 0] = 0.1;
            heatMapSeries1.Data[1, 1] = 0.3;
            heatMapSeries1.Data[1, 2] = 0.2;
            plotModel1.Series.Add(heatMapSeries1);

            var svg = new SvgExporter();
            var svgString = svg.ExportToString(plotModel1);

            var xml = new XmlDocument();
            xml.LoadXml(svgString);
            var x = SvgDocument.Open(xml); // Svg.SvgDocument();
            var bmp = x.Draw();
            bmp.Save(GetPath(HEATMAP_RESULTS_FOLDER_BASE + "testbmp.jpg"));

            var encoder = new PngPlotModelEncoder();
            encoder.SaveImage(plotModel1, GetPath(HEATMAP_RESULTS_FOLDER_BASE + "mine.png"));
        }

        [Test]
        [UsedImplicitly]
        [TestCase(
            @"Data\QC_Shew\QC_Shew_13_04_1b_6Oct13_Cougar_13-06-14_isos.csv",
            @"Data\QC_Shew\QC_Shew_13_04_1b_18Sep13_Cougar_13-06-14_isos.csv",
            @"QC_Shew_13_04_1b_Heatmap"
            )
        ]
        public void TestLcmsWarpAlignment(string path1, string path2, string svgPath)
        {
            // Convert relative paths to absolute paths
            path1 = GetPath(path1);
            path2 = GetPath(path2);
            svgPath = GetPath(HEATMAP_RESULTS_FOLDER_BASE + svgPath);

            var aligner = new LcmsWarpFeatureAligner(new LcmsWarpAlignmentOptions());
            var isosFilterOptions = new DeconToolsIsosFilterOptions();

            var baselineMs = UmcLoaderFactory.LoadMsFeatureData(path1, isosFilterOptions);
            var aligneeMs = UmcLoaderFactory.LoadMsFeatureData(path2, isosFilterOptions);
            var finder = FeatureFinderFactory.CreateFeatureFinder(FeatureFinderType.TreeBased);

            var tolerances = new FeatureTolerances
            {
                FragmentationWindowSize = .5,
                Mass = 13,
                DriftTime = .3,
                Net = .01
            };
            var options = new LcmsFeatureFindingOptions(tolerances)
            {
                MaximumNetRange = .002
            };

            var baseline = finder.FindFeatures(baselineMs, options, null);
            var alignee = finder.FindFeatures(aligneeMs, options, null);
            var data = aligner.Align(baseline, alignee);

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

            var scores = data.HeatScores;
            var width = scores.GetLength(0);
            var height = scores.GetLength(1);

            heatMapSeries1.Data = new double[width, height];


            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    heatMapSeries1.Data[i, j] = Convert.ToDouble(scores[i, j]);
                }
            }

            plotModel1.Series.Add(heatMapSeries1);


            var svg = new SvgExporter();
            var svgString = svg.ExportToString(plotModel1);
            using (var writer = File.CreateText(svgPath + ".svg"))
            {
                writer.Write(svgString);
            }
        }
    }
}