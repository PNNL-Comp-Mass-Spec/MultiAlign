using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MultiAlignCore.Data
{
	public class MassTagConformation : ISerializable
	{
		private int m_massTagId;
		private int m_charge;
		private double m_driftTime;

		public MassTagConformation()
		{
		}

		public MassTagConformation(int massTagId, int charge, double driftTime)
		{
			m_massTagId = massTagId;
			m_charge = charge;
			m_driftTime = driftTime;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool Equals(object obj)
		{
			MassTagConformation massTagConformation = (MassTagConformation)obj;

			if (massTagConformation == null)
			{
				return false;
			}
			else if (!this.MassTagId.Equals(massTagConformation.MassTagId))
			{
				return false;
			}
			else if (!this.Charge.Equals(massTagConformation.Charge))
			{
				return false;
			}
			else
			{
				return this.DriftTime.Equals(massTagConformation.DriftTime);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_massTagId.GetHashCode();
			hash = hash * 23 + m_charge.GetHashCode();
			hash = hash * 23 + m_driftTime.GetHashCode();

			return hash;
		}

		public int MassTagId
		{
			get { return m_massTagId; }
			set { m_massTagId = value; }
		}

		public int Charge
		{
			get { return m_charge; }
			set { m_charge = value; }
		}

		public double DriftTime
		{
			get { return m_driftTime; }
			set { m_driftTime = value; }
		}
	}
}
