using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCommentsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_issues_issue_id",
                schema: "project_tracker",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "fk_comment_users_user_id",
                schema: "project_tracker",
                table: "comment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comment",
                schema: "project_tracker",
                table: "comment");

            migrationBuilder.RenameTable(
                name: "comment",
                schema: "project_tracker",
                newName: "comments",
                newSchema: "project_tracker");

            migrationBuilder.RenameIndex(
                name: "ix_comment_user_id",
                schema: "project_tracker",
                table: "comments",
                newName: "ix_comments_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_comment_issue_id",
                schema: "project_tracker",
                table: "comments",
                newName: "ix_comments_issue_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comments",
                schema: "project_tracker",
                table: "comments",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_issues_issue_id",
                schema: "project_tracker",
                table: "comments",
                column: "issue_id",
                principalSchema: "project_tracker",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_id",
                schema: "project_tracker",
                table: "comments",
                column: "user_id",
                principalSchema: "project_tracker",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_issues_issue_id",
                schema: "project_tracker",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_id",
                schema: "project_tracker",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comments",
                schema: "project_tracker",
                table: "comments");

            migrationBuilder.RenameTable(
                name: "comments",
                schema: "project_tracker",
                newName: "comment",
                newSchema: "project_tracker");

            migrationBuilder.RenameIndex(
                name: "ix_comments_user_id",
                schema: "project_tracker",
                table: "comment",
                newName: "ix_comment_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_issue_id",
                schema: "project_tracker",
                table: "comment",
                newName: "ix_comment_issue_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comment",
                schema: "project_tracker",
                table: "comment",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_issues_issue_id",
                schema: "project_tracker",
                table: "comment",
                column: "issue_id",
                principalSchema: "project_tracker",
                principalTable: "issues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comment_users_user_id",
                schema: "project_tracker",
                table: "comment",
                column: "user_id",
                principalSchema: "project_tracker",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
