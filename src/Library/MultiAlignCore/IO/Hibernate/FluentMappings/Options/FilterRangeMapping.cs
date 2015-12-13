using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings.Options
{
    public class FilterRangeMapping : ClassMap<FilterRange>
    {
        public FilterRangeMapping()
        {
            Table("T_OptionsFilterRange");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.Minimum).Column("Minimum");
            Map(x => x.Maximum).Column("Maximum");
        }
    }
}
