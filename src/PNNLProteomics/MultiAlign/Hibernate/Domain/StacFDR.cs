using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{
	public class StacFDR : ISerializable
	{
		private double m_stacCutoff;
		private int m_numMatches;
		private int m_numFalseMatches;
		private double m_fdr;

		public StacFDR()
		{
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool Equals(object obj)
		{
			StacFDR stacFDR = (StacFDR)obj;

			if (stacFDR == null)
			{
				return false;
			}
			else if (!this.StacCutoff.Equals(stacFDR.StacCutoff))
			{
				return false;
			}
			else if (!this.NumMatches.Equals(stacFDR.NumMatches))
			{
				return false;
			}
			else if (!this.NumFalseMatches.Equals(stacFDR.NumFalseMatches))
			{
				return false;
			}
			else
			{
				return this.FDR.Equals(stacFDR.FDR);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_stacCutoff.GetHashCode();
			hash = hash * 23 + m_numMatches.GetHashCode();
			hash = hash * 23 + m_numFalseMatches.GetHashCode();
			hash = hash * 23 + m_fdr.GetHashCode();

			return hash;
		}

		public double StacCutoff
		{
			get { return m_stacCutoff; }
			set { m_stacCutoff = value; }
		}

		public int NumMatches
		{
			get { return m_numMatches; }
			set { m_numMatches = value; }
		}

		public int NumFalseMatches
		{
			get { return m_numFalseMatches; }
			set { m_numFalseMatches = value; }
		}

		public double FDR
		{
			get { return m_fdr; }
			set { m_fdr = value; }
		}
	}
}
