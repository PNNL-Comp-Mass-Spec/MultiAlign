using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Factors;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class ExperimentalFactorMapping : ClassMap<ExperimentalFactor>
    {
        public ExperimentalFactorMapping()
        {
            Table("T_Factors");
            Not.LazyLoad();
            Id(x => x.FactorID).Column("Factor_ID").GeneratedBy.Assigned();
            Map(x => x.Name).Column("Factor_Name").Nullable();
            Map(x => x.Value).Column("Factor_Value").Nullable();

            //<class name="ExperimentalFactor" table="T_Factors" lazy="false">
            //  <composite-id>
            //    <key-property name="FactorID" column="Factor_ID" type="int" />
            //  </composite-id>
            //  <property name="Name" column="Factor_Name" type="string" not-null="false" />
            //  <property name="Value" column="Factor_Value" type="string" not-null="false" />
            //</class>
        }
    }
}
