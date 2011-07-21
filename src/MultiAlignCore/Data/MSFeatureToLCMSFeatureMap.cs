using System.Runtime.Serialization;

namespace MultiAlignCore.Data
{
    public class MSFeatureToLCMSFeatureMap
    {
        public MSFeatureToLCMSFeatureMap()
        {
        }


        public override bool Equals(object obj)
        {
            MSFeatureToLCMSFeatureMap other = (MSFeatureToLCMSFeatureMap)obj;

            if (other == null)
            {
                return false;
            }
            else if (!this.LCMSFeatureID.Equals(other.LCMSFeatureID))
            {
                return false;
            }
            else if (!this.MSFeatureID.Equals(other.MSFeatureID))
            {
                return false;
            }
            else
            {
                return this.DatasetID.Equals(other.DatasetID);
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + MSFeatureID.GetHashCode();
            hash = hash * 23 + LCMSFeatureID.GetHashCode();
            hash = hash * 23 + DatasetID.GetHashCode();

            return hash;
        }

        public int DatasetID
        {
            get;
            set;
        }

        public int LCMSFeatureID
        {
            get;
            set;
        }
        public int MSFeatureID
        {
            get;
            set;
        }
    }
}
