using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{
	public class ClusterToMassTagMap : ISerializable
	{
		private int m_clusterId;
		private int m_massTagId;

		public ClusterToMassTagMap()
		{

		}

		public ClusterToMassTagMap(int clusterId, int massTagId)
		{
			m_clusterId = clusterId;
			m_massTagId = massTagId;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool Equals(object obj)
		{
			ClusterToMassTagMap clusterToMassTagMap = (ClusterToMassTagMap)obj;

			if (clusterToMassTagMap == null)
			{
				return false;
			}
			else if (!this.ClusterId.Equals(clusterToMassTagMap.ClusterId))
			{
				return false;
			}
			else
			{
				return this.MassTagId.Equals(clusterToMassTagMap.MassTagId);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_clusterId.GetHashCode();
			hash = hash * 23 + m_massTagId.GetHashCode();

			return hash;
		}

		public int ClusterId
		{
			get { return m_clusterId; }
			set { m_clusterId = value; }
		}

		public int MassTagId
		{
			get { return m_massTagId; }
			set { m_massTagId = value; }
		}
	}
}
