using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class LcmsWarpResultsMapping : ClassMap<LcmsWarpResults>
    {
        public LcmsWarpResultsMapping()
        {
            Table("T_LcmsWarpResults");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            // Implicit polymorphism - One table per concrete type of the interface "IAlignmentFunction"
            ReferencesAny(x => x.AlignmentFunction).MetaType<string>().EntityTypeColumn("AlignFuncType").EntityIdentifierColumn("AlignFuncId").IdentityType<int>().Cascade.All();
        }
    }
}