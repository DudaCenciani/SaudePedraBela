using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaudePedraBela.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaEscalas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escalas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaminhoArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escalas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documento_IdCategoria",
                table: "Documento",
                column: "IdCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Documento_Categorias_IdCategoria",
                table: "Documento",
                column: "IdCategoria",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documento_Categorias_IdCategoria",
                table: "Documento");

            migrationBuilder.DropTable(
                name: "Escalas");

            migrationBuilder.DropIndex(
                name: "IX_Documento_IdCategoria",
                table: "Documento");
        }
    }
}
