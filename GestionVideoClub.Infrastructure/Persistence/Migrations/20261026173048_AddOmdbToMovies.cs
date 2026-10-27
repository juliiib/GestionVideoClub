using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionVideoClub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOmdbToMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Plot",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            // Las películas que ya existían antes de esta migración quedaron con ImdbId = ""
            // (el defaultValue de la columna nueva). No se puede crear el índice único de más
            // abajo si hay más de una fila con el mismo valor, así que a cada una se le asigna
            // un placeholder único basado en su propio Id. Esas películas no van a quedar
            // vinculadas a ninguna ficha real de OMDb hasta que se vuelvan a cargar.
            migrationBuilder.Sql("UPDATE [Movies] SET [ImdbId] = CONCAT('placeholder-', CONVERT(varchar(36), [Id])) WHERE [ImdbId] = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ImdbId",
                table: "Movies",
                column: "ImdbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_ImdbId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Plot",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "Movies");
        }
    }
}
