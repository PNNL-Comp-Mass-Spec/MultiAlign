using FluentNHibernate.Mapping;
using MultiAlignCore.Data.SequenceData;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class DatabaseSearchSequenceMapping : ClassMap<DatabaseSearchSequence>
    {
        public DatabaseSearchSequenceMapping()
        {
            Table("T_DatabaseSearch");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "Sequence_ID")
                .KeyProperty(x => x.GroupId, "Dataset_ID");

            Map(x => x.UmcFeatureId).Column("LCMS_Feature_ID").Nullable();
            Map(x => x.Sequence).Column("Sequence").Nullable();
            Map(x => x.Scan).Column("Scan").Nullable();
            Map(x => x.Score).Column("Score").Nullable();

            //<class name="DatabaseSearchSequence" table="T_DatabaseSearch" lazy="false">
            //  <composite-id>
            //    <key-property name="Id"             column="Sequence_ID"      type="integer" />
            //    <key-property name="GroupId"        column="Dataset_ID"       type="integer" />
            //  </composite-id>
            //  <property name="UmcFeatureId"         column="LCMS_Feature_ID"  type="integer" />
            //  <property name="Sequence"             column="Sequence"         type="string"   not-null="false" />
            //  <property name="Scan"                 column="Scan"             type="integer"  not-null="false" />
            //  <property name="Score"                column="Score"            type="double"   not-null="false" />
            //</class>
        }
    }
}
