using FluentNHibernate.Mapping;
using MultiAlignCore.Data.SequenceData;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class SequenceToMsnFeatureMapping : ClassMap<SequenceToMsnFeature>
    {
        public SequenceToMsnFeatureMapping()
        {
            Table("T_DatabaseSearch_To_MsnFeature");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "ID")
                .KeyProperty(x => x.DatasetId, "Dataset_ID")
                .KeyProperty(x => x.SequenceId, "Sequence_ID")
                .KeyProperty(x => x.MsnFeatureId, "MSn_Feature_ID")
                .KeyProperty(x => x.UmcFeatureId, "LCMS_Feature_ID");

            //<class name="SequenceToMsnFeature" table="T_DatabaseSearch_To_MsnFeature" lazy="false">
            //  <composite-id>
            //    <key-property name="Id"      column="ID"       type="integer" />
            //    <key-property name="DatasetId"      column="Dataset_ID"       type="integer" />
            //    <key-property name="SequenceId"     column="Sequence_ID"      type="integer" />
            //    <key-property name="MsnFeatureId"   column="MSn_Feature_ID"   type="integer" />
            //    <key-property name="UmcFeatureId"   column="LCMS_Feature_ID"   type="integer" />
            //  </composite-id>          
            //</class>
        }
    }
}
