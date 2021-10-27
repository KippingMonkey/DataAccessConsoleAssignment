using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessConsoleAssignment.Migrations
{
    public partial class UpdateForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screenings_Movies_MoviesID",
                table: "Screenings");

            migrationBuilder.DropIndex(
                name: "IX_Screenings_MoviesID",
                table: "Screenings");

            migrationBuilder.DropColumn(
                name: "MoviesID",
                table: "Screenings");

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_MovieID",
                table: "Screenings",
                column: "MovieID");

            migrationBuilder.AddForeignKey(
                name: "FK_Screenings_Movies_MovieID",
                table: "Screenings",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screenings_Movies_MovieID",
                table: "Screenings");

            migrationBuilder.DropIndex(
                name: "IX_Screenings_MovieID",
                table: "Screenings");

            migrationBuilder.AddColumn<int>(
                name: "MoviesID",
                table: "Screenings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screenings_MoviesID",
                table: "Screenings",
                column: "MoviesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Screenings_Movies_MoviesID",
                table: "Screenings",
                column: "MoviesID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
