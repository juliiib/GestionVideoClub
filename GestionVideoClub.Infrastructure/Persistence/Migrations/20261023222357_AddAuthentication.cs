using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionVideoClub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "DNI", "Email", "LastName", "Name", "PasswordHash", "Phone", "Salary", "Shift" },
                values: new object[] { new Guid("9f6f6b1e-6b9a-4e9d-9c2e-9b7f6a2d4e10"), "Sin domicilio", 1, "admin@videoclub.com", "VideoClub", "Admin", "$2a$11$2b/Af2TsTkMq5.WzDc5h3eRJ9gqIpjC6.UiFRD9Gy3gaTmnAUVqU6", "0000000000", 1m, "Mañana" });

            // Las filas de Clients/Employees que ya existían antes de esta migración quedaron con
            // Email = "" (el defaultValue de la columna nueva). No se puede crear el índice único
            // de más abajo si hay más de una fila con el mismo valor, así que a cada una se le
            // asigna un email placeholder único basado en su propio Id. Esas cuentas no van a poder
            // loguearse hasta que se les asigne un email y un password reales.
            migrationBuilder.Sql("UPDATE [Clients] SET [Email] = CONCAT('client-', CONVERT(varchar(36), [Id]), '@placeholder.local') WHERE [Email] = ''");
            migrationBuilder.Sql("UPDATE [Employees] SET [Email] = CONCAT('employee-', CONVERT(varchar(36), [Id]), '@placeholder.local') WHERE [Email] = ''");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Email",
                table: "Clients");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("9f6f6b1e-6b9a-4e9d-9c2e-9b7f6a2d4e10"));

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Clients");
        }
    }
}
