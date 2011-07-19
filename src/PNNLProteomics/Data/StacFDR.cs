using System;
using System.Runtime.Serialization;
using PNNLProteomics.SMART;

namespace MultiAlignCore.Data
{
	public class StacFDR : ISerializable
	{
		private double m_stacCutoff;
		private int m_numMatches;
		private double m_errors;
		private double m_fdr;

		public StacFDR()
		{
		}

		public StacFDR(classSMARTFdrResult fdrResult)
		{
			m_stacCutoff = fdrResult.Cutoff;
			m_numMatches = fdrResult.NumMatches;
			m_errors = fdrResult.Error;
			m_fdr = fdrResult.FDR;
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
			else if (!this.Errors.Equals(stacFDR.Errors))
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
			hash = hash * 23 + m_errors.GetHashCode();
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

		public double Errors
		{
			get { return m_errors; }
			set { m_errors = value; }
		}

		public double FDR
		{
			get { return m_fdr; }
			set { m_fdr = value; }
		}
	}
}
