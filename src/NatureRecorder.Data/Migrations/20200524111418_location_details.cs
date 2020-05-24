using Microsoft.EntityFrameworkCore.Migrations;

namespace NatureRecorder.Data.Migrations
{
    public partial class location_details : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Locations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "County",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Locations");
        }
    }
}
