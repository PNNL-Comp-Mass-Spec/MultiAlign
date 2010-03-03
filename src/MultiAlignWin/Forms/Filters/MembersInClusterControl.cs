using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


using MultiAlignEngine.Features;
using PNNLProteomics.Filters;

namespace MultiAlignWin.Forms.Filters
{
    public partial class MembersInClusterControl : UserControl, IFilterControl<clsCluster> 
    {
        DatasetMembersFilter m_filter;

        public MembersInClusterControl()
        {
            InitializeComponent();
            m_filter = new DatasetMembersFilter();

            this.Name = "MemberFilter";            
        }

        public override string ToString()
        {
            return Name;
        }
    
        #region IFilterControl Members
        /// <summary>
        /// Gets or sets the filter to use for the data.
        /// </summary>
        public IFilter<clsCluster> Filter
        {
	        get
            {
                m_filter.MaximumMemberCount = Convert.ToInt32(mnum_maxCount.Value);
                m_filter.MinimumMemberCount = Convert.ToInt32(mnum_minMemberCount.Value);
                return m_filter;
	        }
	        set 
	        {                
                if (value != null)
                {                    
                    m_filter            = value as DatasetMembersFilter;
                    if (m_filter != null)
                    {
                        mnum_maxCount.Value         = Convert.ToInt32(m_filter.MaximumMemberCount);
                        mnum_minMemberCount.Value   = Convert.ToInt32(m_filter.MinimumMemberCount);
                    }
                }
	        }
        }
        #endregion
    }
}
