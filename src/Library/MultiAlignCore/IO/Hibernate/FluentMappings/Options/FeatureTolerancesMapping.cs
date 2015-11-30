using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class FeatureTolerancesMapping : ClassMap<FeatureTolerances>
    {
        public FeatureTolerancesMapping()
        {
            Table("T_OptionsFeatureTolerances");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.FragmentationWindowSize).Column("FragmentationWindowSize");
            Map(x => x.DriftTime).Column("DriftTime");
            Map(x => x.Mass).Column("Mass");
            Map(x => x.Net).Column("Net");
        }
    }
}
