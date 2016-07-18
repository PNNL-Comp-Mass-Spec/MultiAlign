using FluentNHibernate.Mapping;
using MultiAlignCore.Data.Features;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class UMCClusterLightMapping : ClassMap<UMCClusterLight>
    {
        public UMCClusterLightMapping()
        {
            Table("T_Clusters");
            Not.LazyLoad();
            Id(x => x.Id).Column("Cluster_ID").GeneratedBy.Assigned();
            Map(x => x.MassMonoisotopic).Column("Mass").Nullable();
            Map(x => x.Net).Column("NET").Nullable();
            Map(x => x.DriftTime).Column("Drift_Time").Nullable();
            Map(x => x.Tightness).Column("Tightness_Score").Nullable();
            Map(x => x.AmbiguityScore).Column("Ambiguity_Score").Nullable();
            Map(x => x.Abundance).Column("Abundance").Nullable();
            Map(x => x.MemberCount).Column("Member_Count").Nullable();
            Map(x => x.DatasetMemberCount).Column("Dataset_Member_Count").Nullable();
            Map(x => x.ChargeState).Column("Charge").Nullable();
            Map(x => x.MsMsCount).Column("MsMs_Spectra_Count").Nullable();
            Map(x => x.IdentifiedSpectraCount).Column("Identified_Spectra_Count").Nullable();
            Map(x => x.MeanSpectralSimilarity).Column("Mean_Spectral_Similarity_Score").Nullable();

            //<class name="UMCClusterLight" table="T_Clusters" lazy="false">
            //  <id name="Id" column="Cluster_ID" type="int">
            //    <generator class="assigned" />
            //  </id>
            //  <property name="MassMonoisotopic"         column="Mass"                 type="double" not-null="false" />
            //  <property name="Net"                      column="NET"                  type="double" not-null="false" />
            //  <property name="DriftTime"                column="Drift_Time"           type="double" not-null="false" />
            //  <property name="Tightness"                column="Tightness_Score"      type="double" not-null="false" />
            //  <property name="AmbiguityScore"           column="Ambiguity_Score"      type="double" not-null="false" />
            //  <property name="Abundance"                column="Abundance"            type="double" not-null="false" />
            //  <property name="MemberCount"              column="Member_Count"         type="int"    not-null="false" />
            //  <property name="DatasetMemberCount"       column="Dataset_Member_Count" type="int"    not-null="false" />
            //  <property name="ChargeState"              column="Charge"               type="int"    not-null="false" />
            //  <property name="MsMsCount"                column="MsMs_Spectra_Count"         type="int"    not-null="false" />
            //  <property name="IdentifiedSpectraCount"   column="Identified_Spectra_Count"   type="int"    not-null="false" />
            //  <property name="MeanSpectralSimilarity"   column="Mean_Spectral_Similarity_Score"      type="double" not-null="false" />
            //</class>
        }
    }
}
