using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessConsoleAssignment.Migrations
{
    public partial class AddScreeningList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screenings_Movies_MoviesID",
                table: "Screenings");

            migrationBuilder.AlterColumn<int>(
                name: "MoviesID",
                table: "Screenings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Screenings_Movies_MoviesID",
                table: "Screenings",
                column: "MoviesID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screenings_Movies_MoviesID",
                table: "Screenings");

            migrationBuilder.AlterColumn<int>(
                name: "MoviesID",
                table: "Screenings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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
