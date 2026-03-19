using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudePedraBela.Migrations
{
    /// <inheritdoc />
    public partial class AddArquivoFarmaciaCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Arquivo",
                table: "FarmaciaCards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arquivo",
                table: "FarmaciaCards");
        }
    }
}
