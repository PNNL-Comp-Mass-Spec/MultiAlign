using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using PNNLControls;
using PNNLProteomics.Data.Analysis;
using PNNLControls.Drawing.Plotting;
using PNNLControls.Drawing.Charting;

namespace MultiAlignWin.Drawing
{
    public partial class controlHistogram : ctlHistogram
    {

        public controlHistogram()
        {
            InitializeComponent();
        }

        public controlHistogram(float [] bins, float [] freqs, string name)
        {
            InitializeComponent();

            AddData(bins, freqs, name);
        }

        /// <summary>
        /// Adds data to the histogram data series.
        /// </summary>
        /// <param name="bins">X-component to render as bins</param>
        /// <param name="freq">Y-component to render as counts </param>
        /// <param name="name">Name to display for series.</param>
        public void AddData(float[] bins, float[] freq, string name, Color color)
        {
            clsShape shape = new PNNLControls.DiamondShape(1, false); ;
            clsPlotParams plotParams = new PNNLControls.clsPlotParams(shape, color);
            plotParams.Name = name;

            this.ViewPortHistory.Clear();
            this.AutoViewPortOnAddition = true;
            this.AddSeries(new PNNLControls.clsSeries(ref bins, ref freq, plotParams));
        }

        /// <summary>
        /// Adds data to the histogram data series.
        /// </summary>
        /// <param name="bins">X-component to render as bins</param>
        /// <param name="freq">Y-component to render as counts </param>
        /// <param name="name">Name to display for series.</param>
        public void AddData(float[] bins, float[] freq, string name)
        {
            AddData(bins, freq, name, Color.Red);            
        }
    }
}
