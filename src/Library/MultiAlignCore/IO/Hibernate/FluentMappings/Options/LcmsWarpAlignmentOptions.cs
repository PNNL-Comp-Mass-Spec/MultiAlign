using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class LcmsWarpAlignmentOptionsMapping : ClassMap<LcmsWarpAlignmentOptions>
    {
        public LcmsWarpAlignmentOptionsMapping()
        {
            Table("T_OptionsLcmsWarpAlignment");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.NumTimeSections).Column("NumTimeSections");
            Map(x => x.ContractionFactor).Column("ContractionFactor");
            Map(x => x.MaxTimeDistortion).Column("MaxTimeDistortion");
            Map(x => x.MaxPromiscuity).Column("MaxPromiscuity");
            Map(x => x.UsePromiscuousPoints).Column("UsePromiscuousPoints");
            Map(x => x.MassCalibUseLsq).Column("MassCalibUseLsq");
            Map(x => x.MassCalibrationWindow).Column("MassCalibrationWindow");
            Map(x => x.MassCalibNumXSlices).Column("MassCalibNumXSlices");
            Map(x => x.MassCalibNumYSlices).Column("MassCalibNumYSlices");
            Map(x => x.MassCalibMaxJump).Column("MassCalibMaxJump");
            Map(x => x.MassCalibMaxZScore).Column("MassCalibMaxZScore");
            Map(x => x.MassCalibLsqMaxZScore).Column("MassCalibLsqMaxZScore");
            Map(x => x.MassCalibLsqNumKnots).Column("MassCalibLsqNumKnots");
            Map(x => x.MassTolerance).Column("MassTolerance");
            Map(x => x.NetTolerance).Column("NetTolerance");
            Map(x => x.AlignType).Column("AlignType").Nullable();
            Map(x => x.CalibrationType).Column("CalibrationType").Nullable();
            Map(x => x.AlignToMassTagDatabase).Column("AlignToMassTagDatabase");
            Map(x => x.AMTTagFilterNETMin).Column("AMTTagFilterNETMin");
            Map(x => x.AMTTagFilterNETMax).Column("AMTTagFilterNETMax");
            Map(x => x.AMTTagFilterMassMin).Column("AMTTagFilterMassMin");
            Map(x => x.AMTTagFilterMassMax).Column("AMTTagFilterMassMax");
            Map(x => x.MinimumAMTTagObsCount).Column("MinimumAMTTagObsCount");
            Map(x => x.MassBinSize).Column("MassBinSize");
            Map(x => x.NetBinSize).Column("NetBinSize");
            Map(x => x.DriftTimeBinSize).Column("DriftTimeBinSize");
            Map(x => x.TopFeatureAbundancePercent).Column("TopFeatureAbundancePercent");
            Map(x => x.StoreAlignmentFunction).Column("StoreAlignmentFunction");
            Map(x => x.AlignmentAlgorithmType).Column("AlignmentAlgorithmType").Nullable();

            //HasMany(x => x.SeparationTypes).Cascade.AllDeleteOrphan();
        }
    }
}
