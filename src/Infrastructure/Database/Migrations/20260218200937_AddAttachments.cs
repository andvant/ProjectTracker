using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_issues_issue_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_projects_project_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_issue_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_project_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "issue_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "project_id",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.AddColumn<string>(
                name: "mime_type",
                schema: "project_tracker",
                table: "attachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "issue_attachment",
                schema: "project_tracker",
                columns: table => new
                {
                    issue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_attachment", x => new { x.issue_id, x.attachment_id });
                    table.ForeignKey(
                        name: "fk_issue_attachment_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalSchema: "project_tracker",
                        principalTable: "attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_attachment_issues_issue_id",
                        column: x => x.issue_id,
                        principalSchema: "project_tracker",
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_attachment",
                schema: "project_tracker",
                columns: table => new
                {
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_attachment", x => new { x.project_id, x.attachment_id });
                    table.ForeignKey(
                        name: "fk_project_attachment_attachments_attachment_id",
                        column: x => x.attachment_id,
                        principalSchema: "project_tracker",
                        principalTable: "attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_attachment_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "project_tracker",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_issue_attachment_attachment_id",
                schema: "project_tracker",
                table: "issue_attachment",
                column: "attachment_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_attachment_attachment_id",
                schema: "project_tracker",
                table: "project_attachment",
                column: "attachment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "issue_attachment",
                schema: "project_tracker");

            migrationBuilder.DropTable(
                name: "project_attachment",
                schema: "project_tracker");

            migrationBuilder.DropColumn(
                name: "mime_type",
                schema: "project_tracker",
                table: "attachments");

            migrationBuilder.AddColumn<Guid>(
                name: "issue_id",
                schema: "project_tracker",
                table: "attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "project_id",
                schema: "project_tracker",
                table: "attachments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_attachments_issue_id",
                schema: "project_tracker",
                table: "attachments",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_project_id",
                schema: "project_tracker",
                table: "attachments",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_issues_issue_id",
                schema: "project_tracker",
                table: "attachments",
                column: "issue_id",
                principalSchema: "project_tracker",
                principalTable: "issues",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_projects_project_id",
                schema: "project_tracker",
                table: "attachments",
                column: "project_id",
                principalSchema: "project_tracker",
                principalTable: "projects",
                principalColumn: "id");
        }
    }
}
