using System;
using System.Collections.Generic;
using System.Text;
using MultiAlignEngine.Features;

namespace PNNLProteomics.Filters
{
	public class DatasetMembersFilter : IFilter<clsCluster>
	{
		private int m_datasetMembersRequired;

		public DatasetMembersFilter(int datasetMembersRequired)
		{
			m_datasetMembersRequired = datasetMembersRequired;
		}

		public Boolean DoesPassFilter(clsCluster umcCluster)
		{
			return (umcCluster.mshort_num_dataset_members >= m_datasetMembersRequired);
		}
	}
}
