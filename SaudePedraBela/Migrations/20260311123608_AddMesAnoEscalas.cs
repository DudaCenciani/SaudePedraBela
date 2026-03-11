using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudePedraBela.Migrations
{
    /// <inheritdoc />
    public partial class AddMesAnoEscalas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ano",
                table: "Escalas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mes",
                table: "Escalas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ano",
                table: "Escalas");

            migrationBuilder.DropColumn(
                name: "Mes",
                table: "Escalas");
        }
    }
}
