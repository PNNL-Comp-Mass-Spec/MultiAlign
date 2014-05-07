using System;
using System.Collections.Generic;
using MultiAlignCore.Data.Factors;
using MultiAlignEngine;
using System.IO;

namespace MultiAlignCore.Data.Factors
{
	/// <summary>
	/// Contains information about a dataset used for analysis.r
	/// </summary>
	[Serializable]
	public class DatasetToExperimentalFactorMap 
	{
        public int FactorID
        {
            get;
            set;
        }
        public int DatasetID
        {
            get;
            set;
        }

		public override bool Equals(object obj)
		{
            DatasetToExperimentalFactorMap map = obj as DatasetToExperimentalFactorMap;
            if (map == null)
                return false;


            return map.DatasetID == DatasetID && FactorID == map.FactorID;
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + FactorID.GetHashCode();
			hash = hash * 23 + DatasetID.GetHashCode();

			return hash;
		}
	}
}
