using FluentNHibernate.Mapping;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class ProteinMapping : ClassMap<Protein>
    {
        public ProteinMapping()
        {
            Table("T_Proteins");
            Not.LazyLoad();
            Id(x => x.RefId).Column("Ref_Id").GeneratedBy.Assigned();
            Map(x => x.Sequence).Column("Protein").Nullable();
            Map(x => x.ProteinDescription).Column("Protein_Description").Nullable();
            Map(x => x.ProteinId).Column("Protein_ID").Nullable();

            //<class name="Protein" table="T_Proteins" lazy="false">
            //  <id name="RefId" column="Ref_Id" type="integer">
            //    <generator class="assigned" />      
            //  </id>
            //  <property name="Sequence"            column="Protein"              type="string"   not-null="false" />
            //  <property name="ProteinDescription"  column="Protein_Description"  type="string"   not-null="false" />
            //  <property name="ProteinId"           column="Protein_ID"           type="int"      not-null="false" />
            //</class>
        }
    }
}
