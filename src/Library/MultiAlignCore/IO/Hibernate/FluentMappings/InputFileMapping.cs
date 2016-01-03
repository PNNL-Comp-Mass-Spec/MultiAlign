
using FluentNHibernate.Mapping;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class InputFileMapping : ClassMap<InputFile>
    {
        public InputFileMapping()
        {
            Table("T_InputFiles");
            Not.LazyLoad();
            //Id(x => x.Path).Column("Path").GeneratedBy.Assigned();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();
            Map(x => x.Path).Column("Path").Nullable();
            Map(x => x.RelativePath).Column("RelativePath").Nullable();
            Map(x => x.FileType).Column("FileType").Nullable();
            Map(x => x.Extension).Column("Extension").Nullable();
        }
    }
}
