using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Factors;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class DatasetToExperimentalFactorMapMapping : ClassMap<DatasetToExperimentalFactorMap>
    {
        public DatasetToExperimentalFactorMapMapping()
        {
            Table("T_Datasets_To_Factors_Map");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.DatasetID, "Dataset_ID")
                .KeyProperty(x => x.FactorID, "Factor_ID");

            //<class name="DatasetToExperimentalFactorMap" table="T_Datasets_To_Factors_Map" lazy="false">
            //	<composite-id>
            //    <key-property name="DatasetID" column="Dataset_ID" type="int" />
            //    <key-property name="FactorID" column="Factor_ID" type="int" />
            //	</composite-id>		
            //</class>
        }
    }
}
