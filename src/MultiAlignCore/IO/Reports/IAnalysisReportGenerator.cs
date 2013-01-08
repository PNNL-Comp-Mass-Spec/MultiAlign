using System;
using PNNLOmics.Data.Features;
using System.Collections.Generic;
using MultiAlignCore.Algorithms;

namespace MultiAlignCore.IO
{
    public interface IAnalysisReportGenerator
    {
        MultiAlignCore.Data.AnalysisConfig Config { get; set; }
        void CreateAlignmentPlots(MultiAlignCore.Data.FeaturesAlignedEventArgs e);
        void CreateBaselinePlots(MultiAlignCore.Data.BaselineFeaturesLoadedEventArgs e);
        void CreateMassTagPlot(MultiAlignCore.Data.MassTagsLoadedEventArgs e);
        void CreatePeakMatchedPlots(MultiAlignCore.Data.FeaturesPeakMatchedEventArgs e);
        void CreatePlotReport();
        string PlotPath { get; set; }
        //void SaveImage(System.Drawing.Image image, string name, string description);
        void CreateClusterPlots(FeaturesClusteredEventArgs clusters);
        void CreateChargePlots(Dictionary<int, int> chargeMap);
    }
}
