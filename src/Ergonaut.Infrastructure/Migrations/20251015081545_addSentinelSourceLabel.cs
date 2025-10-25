using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ergonaut.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSentinelSourceLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Tasks");
        }
    }
}
