using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MSFeatureToMSnFeatureMapMapping : ClassMap<MSFeatureToMSnFeatureMap>
    {
        public MSFeatureToMSnFeatureMapMapping()
        {
            Table("T_MSnFeature_To_MSFeature_Map");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.RawDatasetID, "Raw_Dataset_ID")
                .KeyProperty(x => x.MSDatasetID, "MS_Dataset_ID")
                .KeyProperty(x => x.MSFeatureID, "MS_Feature_ID")
                .KeyProperty(x => x.MSMSFeatureID, "MSn_Feature_ID")
                .KeyProperty(x => x.LCMSFeatureID, "LCMS_Feature_ID");

            //<class name="MSFeatureToMSnFeatureMap" table="T_MSnFeature_To_MSFeature_Map" lazy="false">
            //  <composite-id>
            //    <key-property name="RawDatasetID"     column="Raw_Dataset_ID"     type="int" />
            //    <key-property name="MSDatasetID"      column="MS_Dataset_ID"      type="int" />
            //    <key-property name="MSFeatureID"      column="MS_Feature_ID"      type="int" />
            //    <key-property name="MSMSFeatureID"    column="MSn_Feature_ID"     type="int" />
            //    <key-property name="LCMSFeatureID"    column="LCMS_Feature_ID"    type="int" />
            //  </composite-id>
            //</class>
        }
    }
}
