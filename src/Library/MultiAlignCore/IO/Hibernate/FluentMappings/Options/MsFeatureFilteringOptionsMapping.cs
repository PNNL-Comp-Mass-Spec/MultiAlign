using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MsFeatureFilteringOptionsMapping : ClassMap<MsFeatureFilteringOptions>
    {
        public MsFeatureFilteringOptionsMapping()
        {
            Table("T_OptionsMsFeatureFiltering");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.MinimumIntensity).Column("MinimumIntensity");
            Map(x => x.MinimumDeisotopingScore).Column("MinimumDeisotopingScore");
            Map(x => x.ShouldUseMzFilter).Column("ShouldUseMzFilter");
            Map(x => x.ShouldUseIntensityFilter).Column("ShouldUseIntensityFilter");
            Map(x => x.ShouldUseChargeFilter).Column("ShouldUseChargeFilter");
            Map(x => x.ShouldUseDeisotopingFilter).Column("ShouldUseDeisotopingFilter");

            References(x => x.ChargeRange).Cascade.All().Nullable();
            References(x => x.MzRange).Cascade.All().Nullable();
        }
    }
}
