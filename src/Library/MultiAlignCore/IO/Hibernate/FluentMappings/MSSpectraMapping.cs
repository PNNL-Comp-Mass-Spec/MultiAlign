using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MSSpectraMapping : ClassMap<MSSpectra>
    {
        public MSSpectraMapping()
        {
            Table("T_Msn_Features");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "SPECTRA_ID")
                .KeyProperty(x => x.GroupId, "DATASET_ID");

            Map(x => x.MsLevel).Column("MS_LEVEL").Nullable();
            Map(x => x.Net).Column("Net").Nullable();
            Map(x => x.Scan).Column("SCAN").Nullable();
            Map(x => x.PrecursorMz).Column("PRECURSOR_MZ").Nullable();
            Map(x => x.TotalIonCurrent).Column("TOTAL_ION_CURRENT").Nullable();
            Map(x => x.CollisionType).Column("COLLISION_TYPE").Nullable();

            //<class name="MSSpectra" table="T_Msn_Features" lazy="false">		
            //  <composite-id>
            //    <key-property name="Id"                 column="SPECTRA_ID" type="integer" />
            //    <key-property name="GroupId"            column="DATASET_ID" type="integer" />
            //  </composite-id>
            //  <property name="MsLevel"                  column="MS_LEVEL"           type="int"    not-null="false" />
            //	<property name="Net"                      column="Net"     type="double" not-null="false" />
            //	<property name="Scan"                     column="SCAN"               type="int"    not-null="false" />
            //	<property name="PrecursorMz"              column="PRECURSOR_MZ"       type="double" not-null="false" />
            //	<property name="TotalIonCurrent"          column="TOTAL_ION_CURRENT"  type="double" not-null="false" />
            //	<property name="CollisionType"            column="COLLISION_TYPE"/>		
            //</class>
        }
    }
}
