using File.Infrastructure.Persistence.ConfigurationClasses;
using FluentMigrator;

namespace File.Migrations.Migrations
{
    [Migration(1)]
    public class Migration001_AddFileDetails : Migration
    {
        private readonly string _schema;

        public Migration001_AddFileDetails(IPersistenceConfiguration configuration)
        {
            _schema = configuration.Schema;
        }

        public override void Up()
        {
            Create.Schema(_schema);

            Create.Table("file").InSchema(_schema)
                .WithColumn("file_id").AsGuid().NotNullable().PrimaryKey("file_pk")
                .WithColumn("owner_id").AsGuid().NotNullable()
                .WithColumn("file_type").AsString().NotNullable()
                .WithColumn("original_name").AsString().NotNullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("removed_at").AsDate().Nullable();
        }

        public override void Down()
        {
            Delete.Table("image").InSchema(_schema);
        }
    }
}
