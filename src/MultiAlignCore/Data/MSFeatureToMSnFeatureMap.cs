using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiAlignCore.Data
{
    
    /// <summary>
    /// Maps a MS Feature to a MSMS feature 
    /// </summary>
    public class MSFeatureToMSnFeatureMap
    {
        /// <summary>
        /// The raw dataset ID link.
        /// </summary>
        public int RawDatasetID
        {
            get;
            set;
        }
        /// <summary>
        /// The MS dataset ID link
        /// </summary>
        public int MSDatasetID
        {
            get;
            set;
        }
        /// <summary>
        /// The MS Feature ID
        /// </summary>
        public int MSFeatureID
        {
            get;
            set;
        }
        /// <summary>
        /// The MSMS Feature ID
        /// </summary>
        public int MSMSFeatureID
        {
            get;
            set;
        }


        public override bool Equals(object obj)
        {
            MSFeatureToMSnFeatureMap other = (MSFeatureToMSnFeatureMap)obj;

            if (other == null)
            {
                return false;
            }
            else if (!this.RawDatasetID.Equals(other.RawDatasetID))
            {
                return false;
            }
            else if (!this.MSFeatureID.Equals(other.MSFeatureID))
            {
                return false;
            }
            else if (!this.MSMSFeatureID.Equals(other.MSMSFeatureID))
            {
                return false;
            }
            else
            {
                return this.MSDatasetID.Equals(other.MSDatasetID);
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + MSFeatureID.GetHashCode();
            hash = hash * 23 + MSMSFeatureID.GetHashCode();
            hash = hash * 23 + MSDatasetID.GetHashCode();
            hash = hash * 23 + RawDatasetID.GetHashCode();

            return hash;
        }
    }
}
