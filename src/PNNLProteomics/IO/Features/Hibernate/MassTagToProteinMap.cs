using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{
	public class MassTagToProteinMap : ISerializable
	{
		private int m_massTagId;
		private int m_proteinId;
		private int m_cleavageState;
		private int m_terminusState;

		public MassTagToProteinMap()
		{

		}

		public MassTagToProteinMap(int massTagId, int proteinId)
		{
			m_massTagId = massTagId;
			m_proteinId = proteinId;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override bool Equals(object obj)
		{
			MassTagToProteinMap massTagToProteinMap = (MassTagToProteinMap)obj;

			if (massTagToProteinMap == null)
			{
				return false;
			} 
			else if (!this.MassTagId.Equals(massTagToProteinMap.MassTagId))
			{
				return false;
			}
			else
			{
				return this.ProteinId.Equals(massTagToProteinMap.ProteinId);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_massTagId.GetHashCode();
			hash = hash * 23 + m_proteinId.GetHashCode();

			return hash;
		}

		public int MassTagId
		{
			get { return m_massTagId; }
			set { m_massTagId = value; }
		}

		public int ProteinId
		{
			get { return m_proteinId; }
			set { m_proteinId = value; }
		}

		public int CleavageState
		{
			get { return m_cleavageState; }
			set { m_cleavageState = value; }
		}

		public int TerminusState
		{
			get { return m_terminusState; }
			set { m_terminusState = value; }
		}
	}
}
