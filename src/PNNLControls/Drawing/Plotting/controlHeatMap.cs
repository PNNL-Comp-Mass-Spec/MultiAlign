using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PNNLControls.Drawing.Plotting
{
    public partial class controlHeatMap : UserControl
    {
        /// <summary>
        /// Reference to the class who knows how to draw the heatmap data.
        /// </summary>
        private classHeatMap mobj_heatMap;
        private float[,]    marray_data;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public controlHeatMap()
        {
            InitializeComponent();
            mobj_heatMap = new classHeatMap();

        }
        /// <summary>
        /// Gets or sets the title of the plot.
        /// </summary>
        //public string Title
        //{
        //    get
        //    {
        //        return mlabel_title.Text;
        //    }
        //    set
        //    {
        //        mlabel_title.Text = value;
        //        PerformLayout();
        //    }
        //}
        /// <summary>
        /// Gets or sets the data to render on the graphics object.
        /// </summary>
        public float[,] Data
        {
            get
            {
                return marray_data;
            }
            set
            {
                marray_data = value;
            }
        }
        /// <summary>
        /// Overrides the OnPaint method and renders the heatmap to the 
        /// heatmap panel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = Graphics.FromHwnd(mpanel_drawingArea.Handle);
            if (marray_data != null)
            {
                mobj_heatMap.Draw(g, mpanel_drawingArea.Bounds, marray_data);
            }
        }
    }
}
