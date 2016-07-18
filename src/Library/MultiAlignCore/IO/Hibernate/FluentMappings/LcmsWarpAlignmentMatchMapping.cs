using FluentNHibernate.Mapping;
using MultiAlignCore.Algorithms.Alignment.LcmsWarp;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class LcmsWarpAlignmentMatchMapping : ClassMap<LcmsWarpAlignmentMatch>
    {
        public LcmsWarpAlignmentMatchMapping()
        {
            Table("T_LcmsWarpAlignmentMatches");
            Not.LazyLoad();

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.AligneeNetStart).Column("AligneeNetStart").Nullable();
            Map(x => x.AligneeNetEnd).Column("AligneeNetEnd").Nullable();
            Map(x => x.AligneeSectionStart).Column("AligneeSectionStart").Nullable();
            Map(x => x.AligneeSectionEnd).Column("AligneeSectionEnd").Nullable();
            Map(x => x.BaselineNetStart).Column("BaselineNetStart").Nullable();
            Map(x => x.BaselineNetEnd).Column("BaselineNetEnd").Nullable();
            Map(x => x.BaselineSectionStart).Column("BaselineSectionStart").Nullable();
            Map(x => x.BaselineSectionEnd).Column("BaselineSectionEnd").Nullable();
        }
    }
}