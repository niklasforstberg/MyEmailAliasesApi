using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEmailAliasesApi.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToManyToManyUserAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create the join table first
            migrationBuilder.CreateTable(
                name: "UserEmailAliases",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EmailAliasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmailAliases", x => new { x.UserId, x.EmailAliasId });
                    table.ForeignKey(
                        name: "FK_UserEmailAliases_EmailAliases_EmailAliasId",
                        column: x => x.EmailAliasId,
                        principalTable: "EmailAliases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserEmailAliases_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAliases_EmailAliasId",
                table: "UserEmailAliases",
                column: "EmailAliasId");

            // 2. Migrate existing data - preserve current user-alias associations
            migrationBuilder.Sql(@"
                INSERT INTO UserEmailAliases (UserId, EmailAliasId)
                SELECT UserId, Id FROM EmailAliases WHERE UserId IS NOT NULL
            ");

            // 3. Now drop the old foreign key, index, and column
            migrationBuilder.DropForeignKey(
                name: "FK_EmailAliases_Users_UserId",
                table: "EmailAliases");

            migrationBuilder.DropIndex(
                name: "IX_EmailAliases_UserId",
                table: "EmailAliases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailAliases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmailAliases");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EmailAliases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmailAliases_UserId",
                table: "EmailAliases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAliases_Users_UserId",
                table: "EmailAliases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
