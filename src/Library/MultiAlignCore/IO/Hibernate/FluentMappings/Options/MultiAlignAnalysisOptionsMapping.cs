using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MultiAlignAnalysisOptionsMapping : ClassMap<MultiAlignAnalysisOptions>
    {
        public MultiAlignAnalysisOptionsMapping()
        {
            Table("T_OptionsAnalysis");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.HasMsMs).Column("HasMsMs");
            Map(x => x.UsedIonMobility).Column("UsedIonMobility");

            References(x => x.InstrumentTolerances).Cascade.All().Nullable();
            References(x => x.MassTagDatabaseOptions).Cascade.All().Nullable();
            References(x => x.MsFilteringOptions).Cascade.All().Nullable();
            References(x => x.LcmsFindingOptions).Cascade.All().Nullable();
            References(x => x.LcmsFilteringOptions).Cascade.All().Nullable();
            References(x => x.AlignmentOptions).Cascade.All().Nullable();
            References(x => x.LcmsClusteringOptions).Cascade.All().Nullable();
            References(x => x.StacOptions).Cascade.All().Nullable();
            References(x => x.SpectralOptions).Cascade.All().Nullable();
        }
    }
}
