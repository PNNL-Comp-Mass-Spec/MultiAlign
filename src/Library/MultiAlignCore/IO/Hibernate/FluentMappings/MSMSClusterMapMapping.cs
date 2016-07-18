using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MSMSClusterMapMapping : ClassMap<MSMSClusterMap>
    {
        public MSMSClusterMapMapping()
        {
            Table("T_Msn_Clusters");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.ClusterID, "Cluster_ID")
                .KeyProperty(x => x.MSMSID, "MSn_ID")
                .KeyProperty(x => x.GroupID, "Dataset_ID");

            //<class name="MSMSClusterMap" table="T_Msn_Clusters" lazy="false">
            //  <composite-id>
            //    <key-property name="ClusterID"  column="Cluster_ID"  type="int" />
            //    <key-property name="MSMSID"      column="MSn_ID"  type="int" />
            //    <key-property name="GroupID"      column="Dataset_ID"  type="int" />
            //  </composite-id>
            //</class>
        }
    }
}
