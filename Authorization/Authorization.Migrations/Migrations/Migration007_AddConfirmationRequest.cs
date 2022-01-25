using System;
using Authorization.Infrastructure.Persistence.ConfigurationClasses;
using FluentMigrator;
using RestSharp.Extensions;

namespace Authorization.Migrations.Migrations
{
    [Migration(7)]
    public class Migration007_AddConfirmationRequest : Migration
    {
        private readonly string _schema;

        public Migration007_AddConfirmationRequest(IPersistenceConfiguration configuration)
        {
            _schema = configuration.Schema;
        }

        public override void Up()
        {
            Create.Table("confirmation_request").InSchema(_schema)
                .WithColumn("cr_id").AsGuid().NotNullable().PrimaryKey("confirmation_request_pk")
                .WithColumn("u_id").AsGuid().NotNullable()
                .ForeignKey("confirmation_request_user_fk", _schema, "user", "u_id")
                .WithColumn("subject").AsString().NotNullable()
                .WithColumn("additional_subject").AsString().NotNullable()
                .WithColumn("confirmed").AsBoolean().NotNullable()
                .WithColumn("request_type").AsString().NotNullable()
                .WithColumn("receiver").AsString().Nullable()
                .WithColumn("revoked_at").AsString().Nullable()
                .WithColumn("created_at").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn("expired_at").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Table("confirmation_request").InSchema(_schema);
        }
    }
}
