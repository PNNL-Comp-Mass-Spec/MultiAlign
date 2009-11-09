using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLControls.Drawing.Charting
{
    /// <summary>
    /// Class that filters based on user defined criteria.
    /// </summary>
    public class classLinearPointFilter : classPointFilter
    {
        /// <summary>
        /// Minimum value to pass.
        /// </summary>
        private double mdouble_min;
        /// <summary>
        /// Maximum value to pass.
        /// </summary>
        private double mdouble_max;

        public classLinearPointFilter(double min, double max)
        {
            mdouble_max = max;
            mdouble_min = min;
        }


        /// <summary>
        /// filters based off if x is less than min and x is greater than max.
        /// </summary>
        /// <param name="x">Value to evaluate</param>
        /// <returns>True if it passes the filter, False if not.</returns>
        public override bool Filter(double x)
        {

            bool passed = (x < mdouble_max && x > mdouble_min);

            if (mbool_invert == true)
                return passed == false;
            return passed;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the maximum filter value.
        /// </summary>
        public double Minimum
        {
            get
            {
                return mdouble_min;
            }
            set
            {
                mdouble_min = value;
            }
        }
        /// <summary>
        /// Gets or sets the maximum filter value.
        /// </summary>
        public double Maximum
        {
            get
            {
                return mdouble_max;
            }
            set
            {
                mdouble_max = value;
            }
        }
        #endregion
    }
 
}
