using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Options;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class StacOptionsMapping : ClassMap<StacOptions>
    {
        public StacOptionsMapping()
        {
            Table("T_OptionsStac");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.IdentificationAlgorithm).Column("IdentificationAlgorithm").Nullable();
            Map(x => x.UsePriors).Column("UsePriors");
            Map(x => x.HistogramBinWidth).Column("HistogramBinWidth");
            Map(x => x.HistogramMultiplier).Column("HistogramMultiplier");
            Map(x => x.ShiftAmount).Column("ShiftAmount");
            Map(x => x.ShouldCalculateHistogramFDR).Column("ShouldCalculateHistogramFDR");
            Map(x => x.ShouldCalculateShiftFDR).Column("ShouldCalculateShiftFDR");
            Map(x => x.ShouldCalculateSLiC).Column("ShouldCalculateSLiC");
            Map(x => x.ShouldCalculateSTAC).Column("ShouldCalculateSTAC");
            Map(x => x.UseDriftTime).Column("UseDriftTime");
            Map(x => x.UseEllipsoid).Column("UseEllipsoid");
            Map(x => x.DriftTimeTolerance).Column("DriftTimeTolerance");
            Map(x => x.MassTolerancePPM).Column("MassTolerancePPM");
            Map(x => x.NETTolerance).Column("NETTolerance");
            Map(x => x.Refined).Column("Refined");
        }
    }
}
