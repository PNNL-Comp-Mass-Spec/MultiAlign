using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PNNLControls.Drawing.Charting
{
    public partial class formLinearPointFilter : Form
    {
        /// <summary>
        /// Fired when the filter values have changed.
        /// </summary>
        public event System.EventHandler UpdatedFilters;

        private classLinearPointFilter mobj_filterX;
        private classLinearPointFilter mobj_filterY;

        private double mdouble_dataMinX;
        private double mdouble_dataMaxX;

        private double mdouble_dataMinY;
        private double mdouble_dataMaxY;

        public formLinearPointFilter(classLinearPointFilter xfilter, 
                                     classLinearPointFilter yfilter,
                                     double minX,
                                     double maxX,
                                     double minY,
                                     double maxY)
        {
            InitializeComponent();

            mobj_filterX = xfilter;
            mobj_filterY = yfilter;

            mdouble_dataMaxX = maxX;
            mdouble_dataMaxY = maxY;
            mdouble_dataMinX = minX;
            mdouble_dataMinY = minY;

            mtrackbar_xMaximum.Maximum  = 100;
            mtrackbar_xMaximum.Minimum  = 0;
            mtrackbar_xMaximum.Value    = 100;
            mtrackbar_xMinimum.Maximum  = 100;
            mtrackbar_xMinimum.Minimum  = 0;
            mtrackbar_xMinimum.Value    = 0;

            mtrackbar_yMaximum.Maximum  = 100;
            mtrackbar_yMaximum.Minimum  = 0;
            mtrackbar_yMaximum.Value    = 100;
            mtrackbar_yMinimum.Maximum  = 100;
            mtrackbar_yMinimum.Minimum  = 0;
            mtrackbar_yMinimum.Value    = 0;

            FormClosing += new FormClosingEventHandler(formLinearPointFilter_FormClosing);
   
        }

        /// <summary>
        /// Handles to see if the user is closing the tool window, to only hide it so the object reference is not destroyed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void formLinearPointFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void mtrackbar_xMinimum_Scroll(object sender, EventArgs e)
        {
            mobj_filterX.Minimum = CalculateValue(mtrackbar_xMinimum.Value, 
                                                    mdouble_dataMaxX,
                                                    mdouble_dataMinX);

            UpdateText();
            if (UpdatedFilters != null)
                UpdatedFilters(this, null);
        }
        private void mtrackbar_xMaximum_Scroll(object sender, EventArgs e)
        {
            mobj_filterX.Maximum = CalculateValue(mtrackbar_xMaximum.Value,
                                                    mdouble_dataMaxX,
                                                    mdouble_dataMinX);

            UpdateText();
            if (UpdatedFilters != null)
                UpdatedFilters(this, null);
        }
        private void mtrackbar_yMinimum_Scroll(object sender, EventArgs e)
        {
            mobj_filterY.Minimum = CalculateValue(mtrackbar_yMinimum.Value,
                                                    mdouble_dataMaxY, 
                                                    mdouble_dataMinY);


            UpdateText();
            if (UpdatedFilters != null)
                UpdatedFilters(this, null);
        }
        private void mtrackbar_yMaximum_Scroll(object sender, EventArgs e)
        {
            mobj_filterY.Maximum = CalculateValue(mtrackbar_yMaximum.Value, 
                                                    mdouble_dataMaxY, 
                                                    mdouble_dataMinY);
            
            UpdateText();
            if (UpdatedFilters !=null)
                UpdatedFilters(this, null);
        }

        private double CalculateValue(int x, double max, double min)
        {
            double percent       = Convert.ToDouble(x) / 100.0;
            double value         = min + (max - min) * percent;

            return value;
        }

        private void UpdateText()
        {
            mlabel_xMinimum.Text = string.Format("Minimum: {0:.0}", mobj_filterX.Minimum);
            mlabel_xMaximum.Text = string.Format("Maximum: {0:.0}", mobj_filterX.Maximum);            
            mlabel_yMinimum.Text = string.Format("Minimum: {0:.0}", mobj_filterY.Minimum);            
            mlabel_yMaximum.Text = string.Format("Maximum: {0:.0}", mobj_filterY.Maximum);
        }

        public void UpdateValues()
        {

            mobj_filterX.Minimum = CalculateValue(mtrackbar_xMinimum.Value,
                                                    mdouble_dataMaxX,
                                                    mdouble_dataMinX);
            mobj_filterX.Maximum = CalculateValue(mtrackbar_xMaximum.Value,
                                                    mdouble_dataMaxX,
                                                    mdouble_dataMinX);


            mobj_filterY.Minimum = CalculateValue(mtrackbar_yMinimum.Value,
                                                    mdouble_dataMaxY,
                                                    mdouble_dataMinY);
            mobj_filterY.Maximum = CalculateValue(mtrackbar_yMaximum.Value,
                                                    mdouble_dataMaxY,
                                                    mdouble_dataMinY);

            UpdateText();
        }

        public double MaxX
        {
            get
            {
                return mdouble_dataMaxX;
            }
            set
            {
                mdouble_dataMaxX = value;
                UpdateText();
            }
        }
        public double MinX
        {
            get
            {
                return mdouble_dataMinX;
            }
            set
            {
                mdouble_dataMinX = value;
                UpdateText();
            }
        }
        public double MaxY
        {
            get
            {
                return mdouble_dataMaxY;
            }
            set
            {
                mdouble_dataMaxY = value;
                UpdateText();
            }
        }
        public double MinY
        {
            get
            {
                return mdouble_dataMinY;
            }
            set
            {
                mdouble_dataMinY = value;
                UpdateText();
            }
        }

    }
}