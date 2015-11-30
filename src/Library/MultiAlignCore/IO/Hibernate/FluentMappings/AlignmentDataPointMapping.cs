using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class AlignmentDataPointMapping : ClassMap<AlignmentDataPoint>
    {
        public AlignmentDataPointMapping()
        {
            Table("T_AlignmentDataPoints");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.GroupId, "Dataset_ID")
                .KeyProperty(x => x.ScanNumber, "Scan_Number");

            Map(x => x.RetentionTime).Column("Retention_Time").Nullable();
            Map(x => x.NET).Column("NET").Nullable();
            Map(x => x.AlignedNET).Column("Aligned_NET").Nullable();
        }
    }
}
