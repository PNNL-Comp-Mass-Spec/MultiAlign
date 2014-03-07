using System;
using System.Runtime.Serialization;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignCore.Data.Factors
{
	public class Factor : ISerializable
	{
		private int m_id;
		private DatasetInformation m_dataset;
		private string m_factorName;
		private string m_factorValue;

		public Factor()
		{
		}

		public Factor(int factorId, DatasetInformation datasetId, string factorName, string factorValue)
		{
			m_id = factorId;
			m_dataset = datasetId;
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
			else if (!this.FactorId.Equals(factor.FactorId))
			{
				return false;
			}
			else if (!this.Dataset.Equals(factor.Dataset))
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
			hash = hash * 23 + m_dataset.GetHashCode();
			hash = hash * 23 + m_factorName.GetHashCode();
			hash = hash * 23 + m_factorValue.GetHashCode();

			return hash;
		}

		public int FactorId
		{
			get { return m_id; }
			set { m_id = value; }
		}

		public DatasetInformation Dataset
		{
			get { return m_dataset; }
			set { m_dataset = value; }
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
