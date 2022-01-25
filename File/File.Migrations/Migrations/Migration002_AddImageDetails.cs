using File.Infrastructure.Persistence.ConfigurationClasses;
using FluentMigrator;

namespace File.Migrations.Migrations
{
    [Migration(2)]
    public class Migration002_AddImageDetails : Migration
    {
        private readonly string _schema;

        public Migration002_AddImageDetails(IPersistenceConfiguration configuration)
        {
            _schema = configuration.Schema;
        }

        public override void Up()
        {
            Create.Table("image").InSchema(_schema)
                .WithColumn("image_id").AsGuid().NotNullable().PrimaryKey("image_pk")
                .WithColumn("size").AsString().NotNullable()
                .WithColumn("file_id").AsGuid().NotNullable().ForeignKey("file_id_fk", _schema, "file", "file_id");
        }

        public override void Down()
        {
            Delete.Table("image").InSchema(_schema);
        }
    }
}
