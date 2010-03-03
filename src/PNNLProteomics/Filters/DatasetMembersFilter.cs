using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
    /// <summary>
    /// Filter for minimum number of members.
    /// </summary>
	public class DatasetMembersFilter : IFilter<clsCluster>
    {
        private int m_maxDatasetMembersRequired;
        private int m_minDatasetMembersRequired;
		
        /// <summary>
        /// Gets or sets the minimum number of data members requierd in a UMC cluster.
        /// </summary>
        public int MinimumMemberCount
        {
            get { return m_minDatasetMembersRequired; }
            set { m_minDatasetMembersRequired = value; }
        }
        /// <summary>
        /// Gets or sets the maximum number of data members requierd ax a UMC cluster.
        /// </summary>
        public int MaximumMemberCount
        {
            get { return m_maxDatasetMembersRequired; }
            set { m_maxDatasetMembersRequired = value; }
        }

        /// <summary>
        /// Checks to see if the UMC Cluster has at least the minimum number of members present.
        /// </summary>
        /// <param name="umcCluster"></param>
        /// <returns></returns>
        public bool DoesPassFilter(clsCluster umcCluster)
		{
			return (umcCluster.mshort_num_dataset_members >= m_minDatasetMembersRequired && umcCluster.mshort_num_dataset_members <= m_maxDatasetMembersRequired);
		}
        public override string ToString()
        {
            return "Cluster Member Count";
        }
	}
}
