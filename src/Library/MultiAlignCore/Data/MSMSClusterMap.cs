using System.Runtime.Serialization;

namespace MultiAlignCore.Data
{
    public class MSMSClusterMap
    {
        public MSMSClusterMap()
        {
        }

        public override bool Equals(object obj)
        {
            MSMSClusterMap other = (MSMSClusterMap)obj;

            if (other == null)
            {
                return false;
            }
            else if (!this.ClusterID.Equals(other.ClusterID))
            {
                return false;
            }
            else if (!this.GroupID.Equals(other.GroupID))
            {
                return false;
            }
            else
            {
                return this.MSMSID.Equals(other.MSMSID);
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + ClusterID.GetHashCode();
            hash = hash * 23 + MSMSID.GetHashCode();
            hash = hash * 23 + GroupID.GetHashCode();

            return hash;
        }

        public int ClusterID
        {
            get;
            set;
        }

        public int MSMSID
        {
            get;
            set;
        }
        public int GroupID
        {
            get;
            set;
        }
    }
}
