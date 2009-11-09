using System;
using System.Collections.Generic;
using System.Text;

namespace PNNLControls.Drawing.Charting
{
    /// <summary>
    /// Class that defines filter criteria for a given point.
    /// </summary>
    public abstract class classPointFilter
    {
        /// <summary>
        /// Determines if the filter should invert the filter or not.
        /// </summary>
        protected bool mbool_invert;

        /// <summary>
        /// Function called to determine if x passes the inherent filter.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public abstract bool Filter(double x);

        /// <summary>
        /// Gets or sets whether to invert the filter criteria.
        /// </summary>
        public bool Invert
        {
            get
            {
                return mbool_invert;
            }
            set
            {
                mbool_invert = value;
            }
        }
    }  
}
