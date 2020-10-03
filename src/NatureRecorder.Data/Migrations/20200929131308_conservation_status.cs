using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NatureRecorder.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class conservation_status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatusSchemes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusSchemes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatusSchemeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusRatings_StatusSchemes_StatusSchemeId",
                        column: x => x.StatusSchemeId,
                        principalTable: "StatusSchemes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesStatusRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpeciesId = table.Column<int>(nullable: false),
                    StatusRatingId = table.Column<int>(nullable: false),
                    Region = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: true),
                    End = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesStatusRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpeciesStatusRatings_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeciesStatusRatings_StatusRatings_StatusRatingId",
                        column: x => x.StatusRatingId,
                        principalTable: "StatusRatings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesStatusRatings_SpeciesId",
                table: "SpeciesStatusRatings",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesStatusRatings_StatusRatingId",
                table: "SpeciesStatusRatings",
                column: "StatusRatingId");

            migrationBuilder.CreateIndex(
                name: "IX_StatusRatings_StatusSchemeId",
                table: "StatusRatings",
                column: "StatusSchemeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpeciesStatusRatings");

            migrationBuilder.DropTable(
                name: "StatusRatings");

            migrationBuilder.DropTable(
                name: "StatusSchemes");
        }
    }
}
