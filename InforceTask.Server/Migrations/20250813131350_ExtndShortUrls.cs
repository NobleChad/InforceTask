using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforceTask.Server.Migrations
{
    /// <inheritdoc />
    public partial class ExtndShortUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorEmail",
                table: "ShortUrls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorEmail",
                table: "ShortUrls");
        }
    }
}
