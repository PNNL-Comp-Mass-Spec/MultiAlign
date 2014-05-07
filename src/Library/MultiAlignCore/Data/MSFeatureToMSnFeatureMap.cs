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
        /// <summary>
        /// Gets or sets the ID of the parent feature to the MS feature. 
        /// </summary>
        public int LCMSFeatureID
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            var other = (MSFeatureToMSnFeatureMap)obj;

            if (other == null)
            {
                return false;
            }
            if (!RawDatasetID.Equals(other.RawDatasetID))
            {
                return false;
            }
            if (!MSFeatureID.Equals(other.MSFeatureID))
            {
                return false;
            }
            if (!MSMSFeatureID.Equals(other.MSMSFeatureID))
            {
                return false;
            }
            return MSDatasetID.Equals(other.MSDatasetID);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash * 23 + MSFeatureID.GetHashCode();
            hash = hash * 23 + MSMSFeatureID.GetHashCode();
            hash = hash * 23 + MSDatasetID.GetHashCode();
            hash = hash * 23 + RawDatasetID.GetHashCode();

            return hash;
        }
    }
}
