using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{
	public class Factor : ISerializable
	{
		private int m_id;
		private int m_datasetId;
		private string m_factorName;
		private string m_factorValue;

		public Factor()
		{
		}

		public Factor(int factorId, int datasetId, string factorName, string factorValue)
		{
			m_id = factorId;
			m_datasetId = datasetId;
			m_factorName = factorName;
			m_factorValue = factorValue;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool Equals(object obj)
		{
			Factor factor = (Factor)obj;

			if (factor == null)
			{
				return false;
			}
			else if (!this.Id.Equals(factor.Id))
			{
				return false;
			}
			else if (!this.DatasetId.Equals(factor.DatasetId))
			{
				return false;
			}
			else if (!this.FactorName.Equals(factor.FactorName))
			{
				return false;
			}
			else
			{
				return this.FactorValue.Equals(factor.FactorValue);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_id.GetHashCode();
			hash = hash * 23 + m_datasetId.GetHashCode();
			hash = hash * 23 + m_factorName.GetHashCode();
			hash = hash * 23 + m_factorValue.GetHashCode();

			return hash;
		}

		public int Id
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public int DatasetId
		{
			get { return m_datasetId; }
			set { m_datasetId = value; }
		}

		public string FactorName
		{
			get { return m_factorName; }
			set { m_factorName = value; }
		}

		public string FactorValue
		{
			get { return m_factorValue; }
			set { m_factorValue = value; }
		}
	}
}
