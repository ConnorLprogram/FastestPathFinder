using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PathFinderCorrected.Migrations
{
    public partial class AddLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    longitude = table.Column<double>(nullable: false),
                    latitude = table.Column<double>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    possibleLocation = table.Column<bool>(nullable: false),
                    closePlace = table.Column<bool>(nullable: false),
                    distanceFromLocation = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pathLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    longitude = table.Column<double>(nullable: false),
                    latitude = table.Column<double>(nullable: false),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pathLocations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "pathLocations");
        }
    }
}
