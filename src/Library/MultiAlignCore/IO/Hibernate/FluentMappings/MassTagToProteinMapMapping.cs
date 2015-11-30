using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MassTagToProteinMapMapping : ClassMap<MassTagToProteinMap>
    {
        public MassTagToProteinMapMapping()
        {
            Table("T_Mass_Tag_To_Protein_Map");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.MassTagId, "Mass_Tag_ID")
                .KeyProperty(x => x.ProteinId, "Protein_ID")
                .KeyProperty(x => x.RefId, "Ref_ID");

            Map(x => x.CleavageState).Column("Cleavage_State").Nullable();
            Map(x => x.TerminusState).Column("Terminus_State").Nullable();

            //<class name="MassTagToProteinMap" table="T_Mass_Tag_To_Protein_Map" lazy="false">
            //  <composite-id>
            //    <key-property name="MassTagId" column="Mass_Tag_ID" type="int" />
            //    <key-property name="ProteinId" column="Protein_ID" type="int" />
            //    <key-property name="RefId"     column="Ref_ID" type="int" />
            //  </composite-id>
            //  <property name="CleavageState" column="Cleavage_State" type="int" not-null="false" />
            //  <property name="TerminusState" column="Terminus_State" type="int" not-null="false" />
            //</class>
        }
    }
}
