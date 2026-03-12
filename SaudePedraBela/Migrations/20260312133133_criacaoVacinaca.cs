using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudePedraBela.Migrations
{
    /// <inheritdoc />
    public partial class criacaoVacinaca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "CampanhasVacinacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "CampanhasVacinacao");
        }
    }
}
