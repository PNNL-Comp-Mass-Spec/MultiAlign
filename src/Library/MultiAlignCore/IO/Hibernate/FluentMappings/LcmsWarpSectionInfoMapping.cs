using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp.NetCalibration;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class LcmsWarpSectionInfoMapping : ClassMap<LcmsWarpSectionInfo>
    {
        public LcmsWarpSectionInfoMapping()
        {
            Table("T_LcmsWarpSectionInfo");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.NumSections).Column("NumSections").Nullable();
            Map(x => x.SectionWidth).Column("SectionWidth").Nullable();
            Map(x => x.MinNet).Column("MinNet").Nullable();
            Map(x => x.MaxNet).Column("MaxNet").Nullable();

        }


    }
}
