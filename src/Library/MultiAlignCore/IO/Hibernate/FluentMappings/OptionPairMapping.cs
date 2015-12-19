using FluentNHibernate.Mapping;
using MultiAlignCore.IO.Options;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class OptionPairMapping : ClassMap<OptionPair>
    {
        public OptionPairMapping()
        {
            Table("T_AnalysisOptions");
            Not.LazyLoad();
            Id(x => x.Name).Column("SettingName").GeneratedBy.Assigned();
            Map(x => x.Value).Column("SettingValue");
        }
    }
}
