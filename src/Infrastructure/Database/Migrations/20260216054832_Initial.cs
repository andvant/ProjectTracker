using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    registration_date = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    key = table.Column<string>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    owner_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    updated_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "issues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    key = table.Column<string>(type: "TEXT", nullable: false),
                    number = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    reporter_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    project_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    assignee_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    priority = table.Column<int>(type: "INTEGER", nullable: false),
                    due_date = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    estimation_minutes = table.Column<int>(type: "INTEGER", nullable: true),
                    parent_issue_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    created_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    updated_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issues", x => x.id);
                    table.ForeignKey(
                        name: "fk_issues_issues_parent_issue_id",
                        column: x => x.parent_issue_id,
                        principalTable: "issues",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_issues_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issues_users_assignee_id",
                        column: x => x.assignee_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_issues_users_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_user",
                columns: table => new
                {
                    members_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    projects_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_user", x => new { x.members_id, x.projects_id });
                    table.ForeignKey(
                        name: "fk_project_user_projects_projects_id",
                        column: x => x.projects_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_project_user_users_members_id",
                        column: x => x.members_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    storage_key = table.Column<string>(type: "TEXT", nullable: false),
                    issue_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    project_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    created_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    created_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    updated_by = table.Column<Guid>(type: "TEXT", nullable: false),
                    updated_on = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_attachments_issues_issue_id",
                        column: x => x.issue_id,
                        principalTable: "issues",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_attachments_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "issue_user",
                columns: table => new
                {
                    watched_issues_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    watchers_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_issue_user", x => new { x.watched_issues_id, x.watchers_id });
                    table.ForeignKey(
                        name: "fk_issue_user_issues_watched_issues_id",
                        column: x => x.watched_issues_id,
                        principalTable: "issues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_issue_user_users_watchers_id",
                        column: x => x.watchers_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_issue_id",
                table: "attachments",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_project_id",
                table: "attachments",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_issue_user_watchers_id",
                table: "issue_user",
                column: "watchers_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_assignee_id",
                table: "issues",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_key",
                table: "issues",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_issues_parent_issue_id",
                table: "issues",
                column: "parent_issue_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_project_id",
                table: "issues",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_issues_reporter_id",
                table: "issues",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_user_projects_id",
                table: "project_user",
                column: "projects_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_key",
                table: "projects",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_projects_owner_id",
                table: "projects",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments");

            migrationBuilder.DropTable(
                name: "issue_user");

            migrationBuilder.DropTable(
                name: "project_user");

            migrationBuilder.DropTable(
                name: "issues");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
