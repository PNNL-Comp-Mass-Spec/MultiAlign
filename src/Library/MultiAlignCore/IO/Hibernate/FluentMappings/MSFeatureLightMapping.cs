using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MSFeatureLightMapping : ClassMap<MSFeatureLight>
    {
        public MSFeatureLightMapping()
        {
            Table("T_MSFeatures");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "FEATURE_ID")
                .KeyProperty(x => x.GroupId, "DATASET_ID");

            Map(x => x.DriftTime).Column("DriftTime").Nullable();
            Map(x => x.Scan).Column("SCAN_NUM").Nullable();
            Map(x => x.ChargeState).Column("CHARGE").Nullable();
            Map(x => x.Abundance).Column("ABUNDANCE").Nullable();
            Map(x => x.Mz).Column("MZ").Nullable();
            Map(x => x.MassMonoisotopic).Column("MONOISOTOPIC_MW").Nullable();
            Map(x => x.UmcId).Column("LCMS_FEATURE_ID").Nullable();

            //<class name="MSFeatureLight" table="T_MSFeatures" lazy="false">
            //  <composite-id>
            //    <key-property name="Id"                     column="FEATURE_ID" type="integer" />
            //    <key-property name="GroupId"                column="DATASET_ID" type="integer" />
            //  </composite-id>
            //  <property name="DriftTime"                    column="DriftTime"                  type="double"   not-null="false" />
            //  <property name="Scan"                         column="SCAN_NUM"                   type="int"  not-null="false" />
            //  <property name="ChargeState"                  column="CHARGE"                     type="int"      not-null="false" />
            //  <property name="Abundance"                    column="ABUNDANCE"                  type="double"     not-null="false" />
            //  <property name="Mz"                           column="MZ"                         type="double"   not-null="false" />
            //  <property name="MassMonoisotopic"             column="MONOISOTOPIC_MW"            type="double"   not-null="false" />
            //  <property name="UmcId"                        column="LCMS_FEATURE_ID"   type="int"   not-null="false" />
            //</class>
        }
    }
}
