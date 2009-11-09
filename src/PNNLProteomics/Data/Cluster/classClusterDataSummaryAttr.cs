using System;

namespace PNNLProteomics.Data.Cluster
{
	/// <summary>
	/// Summary description for DataSummaryAttr.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ClusterDataSummary : Attribute
	{		
		public readonly string Description;
		public ClusterDataSummary(string description)
		{
			Description			= description;					
		}
	}
}
