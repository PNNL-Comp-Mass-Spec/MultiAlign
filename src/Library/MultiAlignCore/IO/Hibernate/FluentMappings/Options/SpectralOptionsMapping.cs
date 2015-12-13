using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Alignment.SpectralMatching;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class SpectralOptionsMapping : ClassMap<SpectralOptions>
    {
        public SpectralOptionsMapping()
        {
            Table("T_OptionsSpectral");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.MzBinSize).Column("MzBinSize");
            Map(x => x.NetTolerance).Column("NetTolerance");
            Map(x => x.MzTolerance).Column("MzTolerance");
            Map(x => x.TopIonPercent).Column("TopIonPercent");
            Map(x => x.Fdr).Column("Fdr");
            Map(x => x.SimilarityCutoff).Column("SimilarityCutoff");
            Map(x => x.IdScore).Column("IdScore");
            Map(x => x.ComparerType).Column("ComparerType").Nullable();
            Map(x => x.RequiredPeakCount).Column("RequiredPeakCount");
        }
    }
}
