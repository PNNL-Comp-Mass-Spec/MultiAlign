using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.FeatureMatcher.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class STACFDRMapping : ClassMap<STACFDR>
    {
        public STACFDRMapping()
        {
            Table("T_STAC_FDR");
            Not.LazyLoad();
            Id(x => x.Cutoff).Column("STAC_Cutoff").GeneratedBy.Assigned();
            Map(x => x.FDR).Column("FDR").Nullable();
            Map(x => x.AMTMatches).Column("Num_AMT_Matches").Nullable();
            Map(x => x.ConformationMatches).Column("Conformation_Matches").Nullable();
            Map(x => x.FalseMatches).Column("False_Matches").Nullable();
            Map(x => x.Label).Column("Label").Nullable();

            //<class name="STACFDR" table="T_STAC_FDR" lazy="false">
            //  <id name="Cutoff" column="STAC_Cutoff" type="double">
            //    <generator class="assigned" />
            //  </id>
            //  <property name="FDR" column="FDR" type="int" not-null="false" />
            //  <property name="AMTMatches" column="Num_AMT_Matches" type="int" not-null="false" />
            //  <property name="ConformationMatches" column="Conformation_Matches" type="int" not-null="false" />
            //  <property name="FalseMatches" column="False_Matches" type="int" not-null="false" />
            //  <property name="Label" column="Label" type="string" not-null="false" />    
            //</class>
        }
    }
}
