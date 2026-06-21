using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailPostSendEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "email_post_send_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    recipient = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    sent_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_success = table.Column<bool>(type: "boolean", nullable: false, defaultValueSql: "false"),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    identity_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("email_post_send_events_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_email_post_send_events_post",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_email_post_send_events_identity_user_id",
                table: "email_post_send_events",
                column: "identity_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_email_post_send_events_post_id",
                table: "email_post_send_events",
                column: "post_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_post_send_events");
        }
    }
}
