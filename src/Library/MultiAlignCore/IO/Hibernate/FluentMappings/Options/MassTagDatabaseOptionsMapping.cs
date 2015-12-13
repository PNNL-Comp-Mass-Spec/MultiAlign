using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class MassTagDatabaseOptionsMapping : ClassMap<MassTagDatabaseOptions>
    {
        public MassTagDatabaseOptionsMapping()
        {
            Table("T_OptionsMassTagDatabase");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.ExperimentFilter).Column("ExperimentFilter").Nullable();
            Map(x => x.ExperimentExclusionFilter).Column("ExperimentExclusionFilter").Nullable();
            Map(x => x.OnlyLoadTagsWithDriftTime).Column("OnlyLoadTagsWithDriftTime");
            Map(x => x.MinimumXCorr).Column("MinimumXCorr");
            Map(x => x.MinimumObservationCountFilter).Column("MinimumObservationCountFilter");
            Map(x => x.MinimumPmtScore).Column("MinimumPmtScore");
            Map(x => x.MinimumDiscriminant).Column("MinimumDiscriminant");
            Map(x => x.MinimumPeptideProphetScore).Column("MinimumPeptideProphetScore");
            Map(x => x.MinimumNet).Column("MinimumNet");
            Map(x => x.MaximumNet).Column("MaximumNet");
            Map(x => x.MinimumMass).Column("MinimumMass");
            Map(x => x.MaximumMass).Column("MaximumMass");
        }
    }
}
