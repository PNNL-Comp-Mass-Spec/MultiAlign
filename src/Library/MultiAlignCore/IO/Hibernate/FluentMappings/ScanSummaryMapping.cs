using FluentNHibernate.Mapping;
using MultiAlignCore.Data;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class ScanSummaryMapping : ClassMap<ScanSummary>
    {
        public ScanSummaryMapping()
        {
            Table("T_ScanSummaries");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Scan, "Scan_Num")
                .KeyProperty(x => x.DatasetId, "Dataset_ID");

            Map(x => x.Time).Column("Time").Nullable();
            Map(x => x.MsLevel).Column("MS_Level").Nullable();
            Map(x => x.TotalIonCurrent).Column("Total_Ion_Current").Nullable();
            Map(x => x.NumberOfPeaks).Column("Number_of_Peaks").Nullable();
            Map(x => x.NumberOfDeisotoped).Column("Number_of_Deisotoped").Nullable();
            Map(x => x.PrecursorMz).Column("Precursor_Mz").Nullable();
            Map(x => x.Bpi).Column("Base_peak_intensity").Nullable();
            Map(x => x.BpiMz).Column("Base_peak_Mz").Nullable();
            Map(x => x.CollisionType).Column("Collision_Type");

            //<class name="ScanSummary" table="T_ScanSummaries" lazy="false">
            //  <composite-id>
            //    <key-property name="Scan"               column="Scan_Num"             type="int" />
            //    <key-property name="DatasetId"          column="Dataset_ID"           type="int" />
            //  </composite-id>
            //  <property name="Time"                     column="Time"                 type="double" not-null="false" />
            //  <property name="MsLevel"                  column="MS_Level"             type="int"    not-null="false" />
            //  <property name="TotalIonCurrent"          column="Total_Ion_Current"    type="double" not-null="false" />
            //  <property name="NumberOfPeaks"            column="Number_of_Peaks"      type="double" not-null="false" />
            //  <property name="NumberOfDeisotoped"       column="Number_of_Deisotoped"  type="double" not-null="false" />
            //  <property name="PrecursorMz"              column="Precursor_Mz"         type="double" not-null="false" />
            //  <property name="Bpi"                      column="Base_peak_intensity"  type="double" not-null="false" />
            //  <property name="BpiMz"                    column="Base_peak_Mz"         type="double" not-null="false" />
            //  <property name="CollisionType"            column="Collision_Type"/>
            //</class>
        }
    }
}
