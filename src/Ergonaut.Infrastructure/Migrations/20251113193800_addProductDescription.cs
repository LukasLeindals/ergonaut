using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergonaut.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addProductDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "SourceLabel",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceLabel",
                table: "Projects",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceLabel",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "SourceLabel",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
