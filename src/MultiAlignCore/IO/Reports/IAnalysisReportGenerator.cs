using System;
using PNNLOmics.Data.Features;
using System.Collections.Generic;

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
        void SaveImage(System.Drawing.Image image, string name);
        void CreateClusterPlots(List<UMCClusterLight> clusters);
        void CreateChargePlots(Dictionary<int, int> chargeMap);
    }
}
