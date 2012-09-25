using System;
namespace MultiAlignCore.IO
{
    public interface IAnalysisReportGenerator
    {
        MultiAlignCore.Data.AnalysisConfig Config { get; set; }
        void CreateAlignmentPlots(MultiAlignCore.Data.FeaturesAlignedEventArgs e);
        void CreateBaselinePlots(MultiAlignCore.Data.BaselineFeaturesLoadedEventArgs e);
        void CreateFinalAnalysisPlots();
        void CreateFinalAnalysisPlots(MultiAlignCore.IO.Features.IUmcDAO cache, MultiAlignCore.IO.Features.IUmcClusterDAO clusterCache);
        void CreateMassTagPlot(MultiAlignCore.Data.MassTagsLoadedEventArgs e);
        void CreatePeakMatchedPlots(MultiAlignCore.Data.FeaturesPeakMatchedEventArgs e);
        void CreatePlotReport();
        string PlotPath { get; set; }
        void SaveImage(System.Drawing.Image image, string name);
    }
}
