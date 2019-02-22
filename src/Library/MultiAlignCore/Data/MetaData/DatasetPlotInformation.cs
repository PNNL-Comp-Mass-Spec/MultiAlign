namespace MultiAlignCore.Data.MetaData
{
    /// <summary>
    /// Class for tracking the plot paths for a dataset
    /// </summary>
    public class DatasetPlotInformation
    {
        public string Alignment { get; set; }
        public string Features { get; set; }
        public string MassErrorHistogram { get; set; }
        public string NetErrorHistogram { get; set; }
        public string MassVsNetResidual { get; set; }
        public string MassScanResidual { get; set; }
        public string MassMzResidual { get; set; }
        public string NetResiduals { get; set; }
    }
}