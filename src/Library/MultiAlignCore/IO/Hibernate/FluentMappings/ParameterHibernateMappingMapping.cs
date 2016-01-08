using FluentNHibernate.Mapping;
using MultiAlignCore.IO.Parameters;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class ParameterHibernateMappingMapping : ClassMap<ParameterHibernateMapping>
    {
        public ParameterHibernateMappingMapping()
        {
            Table("T_Analysis_Info");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.OptionGroup, "Option")
                .KeyProperty(x => x.Parameter, "Parameter");

            Map(x => x.Value).Column("Value").Nullable();

            //<class name="ParameterHibernateMapping" table="T_Analysis_Info" lazy="false">
            //  <composite-id>
            //    <key-property name="OptionGroup"          column="Option"      type="string" />
            //    <key-property name="Parameter"            column="Parameter"         type="string" />
            //  </composite-id>
            //  <property name="Value"                      column="Value"             type="string"    not-null="false" />		
            //</class>
        }
    }
}
