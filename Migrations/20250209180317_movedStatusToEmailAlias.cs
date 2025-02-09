using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEmailAliasesApi.Migrations
{
    /// <inheritdoc />
    public partial class movedStatusToEmailAlias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmailForwardings");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EmailAliases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmailAliases");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EmailForwardings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
