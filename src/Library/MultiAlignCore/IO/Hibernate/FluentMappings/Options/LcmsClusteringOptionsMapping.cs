using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Clustering;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class LcmsClusteringOptionsMapping : ClassMap<LcmsClusteringOptions>
    {
        public LcmsClusteringOptionsMapping()
        {
            Table("T_OptionsLcmsClustering");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.ShouldSeparateCharge).Column("ShouldSeparateCharge");
            Map(x => x.DistanceFunction).Column("DistanceFunction").Nullable();
            Map(x => x.LcmsFeatureClusteringAlgorithm).Column("LcmsFeatureClusteringAlgorithm").Nullable();
            Map(x => x.ClusterCentroidRepresentation).Column("ClusterCentroidRepresentation").Nullable();

            References(x => x.InstrumentTolerances).Cascade.All().Nullable();
        }
    }
}
