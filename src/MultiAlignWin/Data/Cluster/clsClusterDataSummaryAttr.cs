using System;

namespace MultiAlignWin.Data
{
	/// <summary>
	/// Summary description for DataSummaryAttr.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class ClusterDataSummary : System.Attribute
	{		
		public readonly string Description;
		public ClusterDataSummary(string description)
		{
			Description			= description;					
		}
	}
}
