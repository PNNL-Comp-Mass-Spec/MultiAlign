using FeatureAlignment.Algorithms.Alignment.LcmsWarp.NetCalibration;
using FluentNHibernate.Mapping;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class LcmsWarpNetAlignmentFunctionMapping : ClassMap<LcmsWarpNetAlignmentFunction>
    {
        public LcmsWarpNetAlignmentFunctionMapping()
        {
            Table("T_LcmsWarpNetAlignmentFunctions");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.SeparationType).Column("SeparationType").Nullable();

            References(x => x.AligneeSections).Cascade.All().Nullable();
            HasMany(x => x.Matches).Cascade.AllDeleteOrphan().Table("T_LcmsWarpAlignmentMatches");
        }
    }
}
