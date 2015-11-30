using FluentNHibernate.Mapping;
using MultiAlignCore.IO.InputFiles;

namespace MultiAlignCore.IO.Hibernate.FluentMappings
{
    public class InputDatabaseMapping : ClassMap<InputDatabase>
    {
        public InputDatabaseMapping()
        {
            Table("T_OptionsInputDatabases");
            Not.LazyLoad();
            Id(x => x.Id).Column("Id").GeneratedBy.Native();

            Map(x => x.Organism).Column("Organism").Nullable();
            Map(x => x.Jobs).Column("Jobs");
            Map(x => x.Description).Column("Description").Nullable();
            Map(x => x.DatabaseName).Column("DatabaseName").Nullable();
            Map(x => x.DatabaseServer).Column("DatabaseServer").Nullable();
            Map(x => x.LocalPath).Column("LocalPath").Nullable();
            Map(x => x.DatabaseFormat).Column("DatabaseFormat").Nullable();
            Map(x => x.UserName).Column("UserName").Nullable(); // TODO: Should probably be encrypted...
            Map(x => x.Password).Column("Password").Nullable(); // TODO: Should probably be encrypted...
        }
    }
}
