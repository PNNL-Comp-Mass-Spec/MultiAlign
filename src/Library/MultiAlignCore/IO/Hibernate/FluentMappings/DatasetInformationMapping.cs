using FluentNHibernate.Mapping;
using MultiAlignCore.Data.MetaData;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class DatasetInformationMapping : ClassMap<DatasetInformation>
    {
        public DatasetInformationMapping()
        {
            Table("T_Datasets");
            Not.LazyLoad();
            Id(x => x.DatasetId).GeneratedBy.Assigned();
            Map(x => x.DMSDatasetID).Column("DMS_Dataset_ID").Nullable();
            Map(x => x.JobID).Column("Job").Nullable();
            Map(x => x.DatasetName).Column("File_Name").Nullable();
            //Map(x => x.ParameterFile).Column("Parameter_File").Nullable();
            //Map(x => x.Path).Column("Directory_Path").Nullable();
            Map(x => x.Name).Column("Name").Nullable();
            //Map(x => x.RawPath).Column("Raw_Path").Nullable();
            //Map(x => x.SequencePath).Column("Sequence_Path").Nullable();
            Map(x => x.IsBaseline).Column("Is_Baseline").Nullable();
            Map(x => x.FeaturesFound).Column("FeaturesFound").Nullable();
            Map(x => x.IsAligned).Column("IsAligned").Nullable();
            Map(x => x.IsClustered).Column("IsClustered").Nullable();
            //HasMany(x => x.InputFiles).Cascade.AllDeleteOrphan().Table("T_InputFiles");
            HasMany(x => x.InputFilesNHibernate).Cascade.AllDeleteOrphan().Table("T_InputFiles");

            //<class name="DatasetInformation" table="T_Datasets" lazy="false">
            //  <composite-id>
            //    <key-property name="DatasetId"    column="Dataset_ID"     type="int" />
            //  </composite-id>
            //  <property name="DMSDatasetID"       column="DMS_Dataset_ID" type="int" />
            //  <property name="JobID"              column="Job"            type="int"      not-null="false" />
            //  <property name="DatasetName"        column="File_Name"      type="string"   not-null="false" />
            //  <property name="ParameterFile"      column="Parameter_File" type="string"   not-null="false" />
            //  <property name="Path"               column="Directory_Path" type="string"   not-null="false" />
            //  <property name="Alias"              column="Alias"          type="string"   not-null="false" />
            //  <property name="RawPath"            column="Raw_Path"       type="string"   not-null="false" />
            //  <property name="SequencePath"       column="Sequence_Path"  type="string"   not-null="false" />
            //  <property name="IsBaseline"         column="Is_Baseline"    type="Boolean"  not-null="false" />
            //</class>
        }
    }
}
