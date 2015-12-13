using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class LcmsFeatureFilteringOptionsMapping : ClassMap<LcmsFeatureFilteringOptions>
    {
        public LcmsFeatureFilteringOptionsMapping()
        {
            Table("T_OptionsLcmsFeatureFiltering");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.FilterOnMinutes).Column("FilterOnMinutes");
            Map(x => x.MinimumDataPoints).Column("MinimumDataPoints");

            References(x => x.FeatureLengthRangeScans).Cascade.All().Nullable();
            References(x => x.FeatureLengthRangeMinutes).Cascade.All().Nullable();
        }
    }
}
