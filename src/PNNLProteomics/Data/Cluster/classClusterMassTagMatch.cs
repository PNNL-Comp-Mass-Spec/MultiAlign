using System;

namespace PNNLProteomics.Data.Cluster
{
	/// <summary>
	/// Summary description for clsClusterMassTagMatch.
	/// </summary>
	public class classClusterMassTagMatch
	{
		private int mint_mass_tag_id ; 
		private int mint_cluster_id ; 
		public classClusterMassTagMatch()
		{
			//
			// TODO: Add constructor logic here
			//
			Set(-1,-1) ;
		}

		public classClusterMassTagMatch(int mt_id, int clust_id)
		{
			//
			// TODO: Add constructor logic here
			//
			Set(mt_id, clust_id) ; 
		}

		void Set(int mt_id, int cluster_id)
		{
			mint_mass_tag_id = mt_id ; 
			mint_cluster_id = cluster_id ;
		}
		public int ClusterId
		{
			get
			{
				return mint_cluster_id ; 
			}
			set
			{
				mint_cluster_id = value ; 
			}
		}
		public int TagIndex
		{
			get
			{
				return mint_mass_tag_id ; 
			}
			set
			{
				mint_mass_tag_id = value ; 
			}
		}
	}
}
