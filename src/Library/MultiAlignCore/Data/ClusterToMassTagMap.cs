namespace MultiAlignCore.Data
{
    public class ClusterToMassTagMap
    {
        private int m_clusterId;
        private int m_massTagId;
        private double m_stacScore;
        private double m_stacUP;

        public ClusterToMassTagMap()
        {
        }

        public ClusterToMassTagMap(int clusterId, int massTagId)
        {
            m_clusterId = clusterId;
            m_massTagId = massTagId;
        }

        public MassTagToCluster MassTag { get; set; }

        public override bool Equals(object obj)
        {
            var clusterToMassTagMap = (ClusterToMassTagMap) obj;

            if (clusterToMassTagMap == null)
            {
                return false;
            }
            if (!ClusterId.Equals(clusterToMassTagMap.ClusterId))
            {
                return false;
            }
            return MassTagId.Equals(clusterToMassTagMap.MassTagId);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            hash = hash*23 + m_clusterId.GetHashCode();
            hash = hash*23 + m_massTagId.GetHashCode();
            hash = hash*23 + ConformerId.GetHashCode();

            return hash;
        }

        public int ClusterId
        {
            get { return m_clusterId; }
            set { m_clusterId = value; }
        }

        public int MassTagId
        {
            get { return m_massTagId; }
            set { m_massTagId = value; }
        }

        public int ConformerId { get; set; }

        public double StacScore
        {
            get { return m_stacScore; }
            set { m_stacScore = value; }
        }

        public double StacUP
        {
            get { return m_stacUP; }
            set { m_stacUP = value; }
        }
    }
}