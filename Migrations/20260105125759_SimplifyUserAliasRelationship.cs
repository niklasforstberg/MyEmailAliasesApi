using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEmailAliasesApi.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyUserAliasRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmailAliases");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EmailForwardings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailForwardings_UserId",
                table: "EmailForwardings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailForwardings_Users_UserId",
                table: "EmailForwardings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailForwardings_Users_UserId",
                table: "EmailForwardings");

            migrationBuilder.DropIndex(
                name: "IX_EmailForwardings_UserId",
                table: "EmailForwardings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmailForwardings");

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
        }
    }
}
