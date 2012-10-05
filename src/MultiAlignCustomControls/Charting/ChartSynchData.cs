
using PNNLControls;


namespace MultiAlignCustomControls.Charting
{
    /// <summary>
    /// Class that helps synch two views
    /// </summary>
    public class ChartSynchData
    {
        public ChartSynchData()
        {
        }

        public ChartSynchType SynchType
        {
            get;
            set;
        }
        public ctlChartBase Target
        {
            get;
            set;
        }
    }

    public enum ChartSynchType
    {
        XAxis,
        YAxis,
        XToYAxis,
        YToXAxis,
        Both
    }
}
