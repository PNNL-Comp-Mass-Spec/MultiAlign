using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class AlignmentOptionsMapping : ClassMap<AlignmentOptions>
    {
        public AlignmentOptionsMapping()
        {
            Table("T_OptionsAlignment");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.IsAlignmentBaselineAMasstagDB).Column("IsAlignmentBaselineAMasstagDB");
            Map(x => x.MassTagObservationCount).Column("MassTagObservationCount");
            Map(x => x.AlignmentBaselineName).Column("AlignmentBaselineName");
            Map(x => x.AlignToAMT).Column("AlignToAMT");

            References(x => x.LCMSWarpOptions).Cascade.All().Not.Nullable();
            References(x => x.InputDatabase).Cascade.All().Nullable();
        }
    }
}
