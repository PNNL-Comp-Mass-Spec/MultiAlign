using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using PNNLProteomics.Filters;
using MultiAlignWin.Forms.Filters;

namespace MultiAlignWin.Forms.Filters
{
    public partial class CalibratedMassFilterControl<T> : UserControl, IFilterControl<T>
    {

        IFilter<T> mobj_filter;

        public CalibratedMassFilterControl()
        {
            InitializeComponent();
        }
        public override string ToString()
        {
            return "Calibrated Mass Filter";
        }

        #region IFilterControl<T> Members

        /// <summary>
        /// Gets or sets the filter associated with this control.
        /// </summary>
        public PNNLProteomics.Filters.IFilter<T> Filter
        {
            get
            {
                return mobj_filter;
            }
            set
            {
                mobj_filter = value;
            }
        }

        #endregion
    }
}
