using System;
using System.Collections.Generic;
using System.Drawing;
using PNNLControls;
using PNNLControls.Drawing.Charting;

namespace MultiAlignCustomControls.Charting
{
    public partial class GenericCDF: ctlLineChart        
    {
        private List<KeyValuePair<float, float>> m_cdf;

        public GenericCDF()
        {
            InitializeComponent();
            IsClipboardCopyEnabled      = true;
            m_cdf                    = new List<KeyValuePair<float,float>>();
        }

        public override void CopyDataToClipboard()
        {
            ApplicationClipboard.ClearApplicationClipboard();
            string data = "Distance\tCount\n";
            foreach (KeyValuePair<float, float> key in m_cdf)
            {
                data += string.Format("{0}\t{1}\n", key.Key, key.Value);
            }
            ApplicationClipboard.SetData(data);
        }

        /// <summary>
        /// Creates a charge state histogram from the data provided.
        /// </summary>
        /// <param name="features"></param>
        public void ConstructHistogram(List<KeyValuePair<float, float>> cdf)
        {
            if (cdf.Count < 1)
                return;
            m_cdf = cdf;
            SeriesCollection.Clear();
            Color color                 = Color.Red;
            clsShape shape              = new PNNLControls.DiamondShape(1, false); ;
            clsPlotParams plotParams    = new PNNLControls.clsPlotParams(shape, color);

            
            float[] bins = new float[cdf.Count];
            float[] freq = new float[cdf.Count];

            int i = 0;
            foreach (KeyValuePair<float, float> point in cdf)
            {                              
                bins[i]     = point.Key;
                freq[i++]   = point.Value;
            }
            
            this.AddSeries(new clsSeries(ref bins, ref freq, plotParams));
        }
    }
}
