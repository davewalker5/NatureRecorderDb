using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NatureRecorder.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class sighting_enhancements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WithYoung",
                table: "Sightings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithYoung",
                table: "Sightings");
        }
    }
}
