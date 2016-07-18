using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class ClusterToMassTagMapMapping : ClassMap<ClusterToMassTagMap>
    {
        public ClusterToMassTagMapMapping()
        {
            Table("T_Cluster_To_Mass_Tag_Map");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.ClusterId, "Cluster_ID")
                .KeyProperty(x => x.MassTagId, "Mass_Tag_ID")
                .KeyProperty(x => x.ConformerId, "Conformer_ID");

            Map(x => x.StacScore).Column("STAC_Score").Nullable();
            Map(x => x.StacUP).Column("STAC_UP").Nullable();

            //<class name="ClusterToMassTagMap" table="T_Cluster_To_Mass_Tag_Map" lazy="false">
            //  <composite-id>
            //    <key-property name="ClusterId" column="Cluster_ID" type="int" />
            //    <key-property name="MassTagId" column="Mass_Tag_ID" type="int" />
            //    <key-property name="ConformerId" column="Conformer_ID" type="int" />
            //  </composite-id>
            //<property name="StacScore" column="STAC_Score" type="double" not-null="false" />
            //<property name="StacUP" column="STAC_UP" type="double" not-null="false" />
            //</class>
        }
    }
}
