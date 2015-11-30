using FluentNHibernate.Mapping;
using MultiAlignCore.Data.MassTags;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class MassTagLightMapping : ClassMap<MassTagLight>
    {
        public MassTagLightMapping()
        {
            Table("T_Mass_Tags");
            Not.LazyLoad();

            CompositeId()
                .KeyProperty(x => x.Id, "Mass_Tag_ID")
                .KeyProperty(x => x.ConformationId, "Conformer_ID");

            Map(x => x.MassMonoisotopic).Column("Mass").Nullable();
            Map(x => x.NetAverage).Column("NET_Average").Nullable();
            Map(x => x.NetPredicted).Column("NET_Predicted").Nullable();
            Map(x => x.NetStandardDeviation).Column("NET_Std_Dev").Nullable();
            Map(x => x.DiscriminantMax).Column("Normalized_Score_Max").Nullable();
            Map(x => x.MsgfSpecProbMax).Column("MSGF_Spec_Prob_Max").Nullable();
            Map(x => x.CleavageState).Column("Cleavage_State").Nullable();
            Map(x => x.Modifications).Column("Modifications").Nullable();
            Map(x => x.PeptideSequence).Column("Peptide").Nullable();
            Map(x => x.PeptideSequenceEx).Column("Peptide_Ex").Nullable();
            Map(x => x.ObservationCount).Column("MS_MS_Observed").Nullable();
            Map(x => x.ModificationCount).Column("Mod_Count").Nullable();
            Map(x => x.ChargeState).Column("Charge").Nullable();
            Map(x => x.DriftTime).Column("Drift_Time").Nullable();

            //<class name="MassTagLight" table="T_Mass_Tags" lazy="false">
            //  <composite-id>
            //    <key-property name="Id" column="Mass_Tag_ID" type="int" />
            //    <key-property name="ConformationId" column="Conformer_ID" type="int" />
            //  </composite-id>
            //  <property name="MassMonoisotopic"     column="Mass"                 type="double" not-null="false" />
            //  <property name="NetAverage"           column="NET_Average"          type="double" not-null="false" />
            //  <property name="NetPredicted"         column="NET_Predicted"        type="double" not-null="false" />
            //  <property name="NetStandardDeviation" column="NET_Std_Dev"          type="double" not-null="false" />
            //  <property name="DiscriminantMax"      column="Normalized_Score_Max" type="double" not-null="false" />
            //  <property name="MsgfSpecProbMax"      column="MSGF_Spec_Prob_Max"   type="double" not-null="false" />
            //  <property name="CleavageState"        column="Cleavage_State"       type="int"    not-null="false" />
            //  <property name="Modifications"        column="Modifications"        type="string" not-null="false" />
            //  <property name="PeptideSequence"      column="Peptide"              type="string" not-null="false" />
            //  <property name="PeptideSequenceEx"    column="Peptide_Ex"           type="string" not-null="false" />
            //  <property name="ObservationCount"     column="MS_MS_Observed"       type="int"    not-null="false" />
            //  <property name="ModificationCount"    column="Mod_Count"            type="int"    not-null="false" />
            //  <property name="ChargeState"          column="Charge" type="int" />
            //  <property name="DriftTime"            column="Drift_Time" type="double" />
            //</class>
        }
    }
}
