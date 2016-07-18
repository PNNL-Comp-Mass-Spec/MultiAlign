using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Alignment;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class AlignmentDataMapping : ClassMap<AlignmentData>
    {
        public AlignmentDataMapping()
        {
            Table("T_AlignmentData");
            Not.LazyLoad();
            Id(x => x.DatasetID).GeneratedBy.Assigned();
            Map(x => x.NETRsquared).Column("Net_R_Squared").Nullable();
            Map(x => x.NETSlope).Column("Net_Slope").Nullable();
            Map(x => x.NETIntercept).Column("Net_Intercept").Nullable();
            Map(x => x.NETMean).Column("Net_Mean").Nullable();
            Map(x => x.NETStandardDeviation).Column("Net_Stdev").Nullable();
            Map(x => x.MassMean).Column("Mass_Mean").Nullable();
            Map(x => x.MassStandardDeviation).Column("Mass_Stdev").Nullable();

            References(x => x.MassAlignmentFunction).Cascade.All().Nullable();
            HasMany(x => x.AlignmentFunctions).AsMap(x => x.SeparationType).Cascade.AllDeleteOrphan();

            //<class name="AlignmentData" table="T_AlignmentData" lazy="false">
            //  <composite-id>
            //    <key-property name="DatasetID" column="Dataset_ID" type="int" />
            //  </composite-id>
            //<property name="NETRsquared"              column="Net_R_Squared"  type="double" not-null="false" />
            //<property name="NETSlope"                 column="Net_Slope"      type="double" not-null="false" />
            //<property name="NETIntercept"             column="Net_Intercept"  type="double" not-null="false" />
            //<property name="NETMean"                  column="Net_Mean"       type="double" not-null="false" />
            //<property name="NETStandardDeviation"     column="Net_Stdev"      type="double" not-null="false" />
            //<property name="MassMean"                 column="Mass_Mean"      type="double" not-null="false" />
            //<property name="MassStandardDeviation"    column="Mass_Stdev"     type="double" not-null="false" />
        }
    }
}
