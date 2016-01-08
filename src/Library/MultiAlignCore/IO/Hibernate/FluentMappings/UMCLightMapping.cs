using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class UMCLightMapping : ClassMap<UMCLight>
    {
        public UMCLightMapping()
        {
            Table("T_LCMS_Features");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "Feature_ID")
                .KeyProperty(x => x.GroupId, "Dataset_ID");

            Map(x => x.ClusterId).Column("Cluster_ID").Nullable();
            Map(x => x.ConformationId).Column("Conformation_ID").Nullable();
            Map(x => x.MassMonoisotopic).Column("Mass").Nullable();
            Map(x => x.MassMonoisotopicAligned).Column("Mass_Aligned").Nullable();
            Map(x => x.Net).Column("NET").Nullable();
            Map(x => x.NetAligned).Column("NET_Aligned").Nullable();
            Map(x => x.Mz).Column("MZ").Nullable();
            Map(x => x.Scan).Column("Scan_LC").Nullable();
            Map(x => x.ScanStart).Column("Scan_LC_Start").Nullable();
            Map(x => x.ScanEnd).Column("Scan_LC_End").Nullable();
            Map(x => x.ChargeState).Column("Charge").Nullable();
            Map(x => x.Abundance).Column("Abundance_Max").Nullable();
            Map(x => x.AbundanceSum).Column("Abundance_Sum").Nullable();
            Map(x => x.DriftTime).Column("Drift_Time").Nullable();
            Map(x => x.AverageInterferenceScore).Column("Average_Interference_Score").Nullable();
            Map(x => x.AverageDeconFitScore).Column("Average_Decon_Fit_Score").Nullable();
            Map(x => x.SpectralCount).Column("UMC_Member_Count").Nullable();
            Map(x => x.SaturatedMemberCount).Column("Saturated_Member_Count").Nullable();
            Map(x => x.MsMsCount).Column("MsMs_Spectra_Count").Nullable();
            Map(x => x.IdentifiedSpectraCount).Column("Identified_Spectra_Count").Nullable();

            //<class name="UMCLight" table="T_LCMS_Features" lazy="false">
            //  <composite-id>
            //    <key-property name="Id"                   column="Feature_ID" type="integer" />
            //    <key-property name="GroupId"              column="Dataset_ID" type="integer" />
            //  </composite-id>		
            //  <property name="ClusterId"                  column="Cluster_ID"                 type="int" not-null="false"/>
            //  <property name="ConformationId"             column="Conformation_ID"            type="int" not-null="false" />
            //  <property name="MassMonoisotopic"           column="Mass"                       type="double" not-null="false" />
            //  <property name="MassMonoisotopicAligned"    column="Mass_Aligned"               type="double" not-null="false" />
            //  <property name="Net"                        column="NET"                        type="double" not-null="false" />
            //  <property name="NetAligned"                 column="NET_Aligned"                type="double" not-null="false" />
            //  <property name="Mz"                         column="MZ"                         type="double" not-null="false" />
            //  <property name="Scan"                       column="Scan_LC"                    type="int" not-null="false" />
            //  <property name="ScanStart"                  column="Scan_LC_Start"              type="int" not-null="false" />
            //  <property name="ScanEnd"                    column="Scan_LC_End"                type="int" not-null="false" />		
            //  <property name="ChargeState"                column="Charge"                     type="int" not-null="false" />
            //  <property name="Abundance"                  column="Abundance_Max"              type="double" not-null="false" />
            //  <property name="AbundanceSum"               column="Abundance_Sum"              type="double" not-null="false" />
            //  <property name="DriftTime"                  column="Drift_Time"                 type="double" not-null="false" />
            //  <property name="AverageInterferenceScore"   column="Average_Interference_Score" type="double" not-null="false" />
            //  <property name="AverageDeconFitScore"       column="Average_Decon_Fit_Score"    type="double" not-null="false" />
            //  <property name="SpectralCount"              column="UMC_Member_Count"           type="int" not-null="false" />
            //  <property name="SaturatedMemberCount"       column="Saturated_Member_Count"     type="int" not-null="false" />
            //  <property name="MsMsCount"                  column="MsMs_Spectra_Count"         type="int"    not-null="false" />
            //  <property name="IdentifiedSpectraCount"     column="Identified_Spectra_Count"   type="int"    not-null="false" />
            //</class>
        }
    }
}
