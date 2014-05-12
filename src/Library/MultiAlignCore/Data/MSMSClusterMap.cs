namespace MultiAlignCore.Data
{
    public class MSMSClusterMap
    {
        public override bool Equals(object obj)
        {
            var other = (MSMSClusterMap) obj;

            if (other == null)
            {
                return false;
            }
            if (!ClusterID.Equals(other.ClusterID))
            {
                return false;
            }
            if (!GroupID.Equals(other.GroupID))
            {
                return false;
            }
            return MSMSID.Equals(other.MSMSID);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + ClusterID.GetHashCode();
            hash = hash*23 + MSMSID.GetHashCode();
            hash = hash*23 + GroupID.GetHashCode();

            return hash;
        }

        public int ClusterID { get; set; }

        public int MSMSID { get; set; }
        public int GroupID { get; set; }
    }
}