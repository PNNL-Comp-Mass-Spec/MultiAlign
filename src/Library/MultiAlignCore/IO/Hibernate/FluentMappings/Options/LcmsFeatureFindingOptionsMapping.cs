using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Clustering;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class LcmsFeatureFindingOptionsMapping : ClassMap<LcmsFeatureFindingOptions>
    {
        public LcmsFeatureFindingOptionsMapping()
        {
            Table("T_OptionsLcmsFeatureFinding");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.FirstPassClusterer).Column("FirstPassClusterer").Nullable();
            Map(x => x.SecondPassClusterer).Column("SecondPassClusterer").Nullable();
            Map(x => x.FindXics).Column("FindXics").Nullable();
            Map(x => x.RefineXics).Column("RefineXics").Nullable();
            Map(x => x.SmoothingWindowSize).Column("SmoothingWindowSize").Nullable();
            Map(x => x.SmoothingPolynomialOrder).Column("SmoothingPolynomialOrder").Nullable();
            Map(x => x.XicRelativeIntensityThreshold).Column("XicRelativeIntensityThreshold").Nullable();
            Map(x => x.SecondPassClustering).Column("SecondPassClustering").Nullable();
            Map(x => x.MaximumScanRange).Column("MaximumScanRange").Nullable();
            Map(x => x.MaximumNetRange).Column("MaximumNetRange").Nullable();

            References(x => x.InstrumentTolerances).Cascade.All().Nullable();
        }
    }
}
