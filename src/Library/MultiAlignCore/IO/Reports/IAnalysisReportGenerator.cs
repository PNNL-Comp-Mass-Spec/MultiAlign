using System.Collections.Generic;
using MultiAlignCore.Algorithms;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO
{
    public interface IAnalysisReportGenerator
    {
        AnalysisConfig Config { get; set; }
        void CreateAlignmentPlots(FeaturesAlignedEventArgs e);
        void CreateBaselinePlots(BaselineFeaturesLoadedEventArgs e);
        void CreateMassTagPlot(MassTagsLoadedEventArgs e);
        void CreatePeakMatchedPlots(FeaturesPeakMatchedEventArgs e);
        void CreatePlotReport();
        string PlotPath { get; set; }
        //void SaveImage(System.Drawing.Image image, string name, string description);
        void CreateClusterPlots(FeaturesClusteredEventArgs clusters);
        void CreateChargePlots(Dictionary<int, int> chargeMap);
    }
}
