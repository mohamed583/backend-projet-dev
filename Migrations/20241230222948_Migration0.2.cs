using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_Personnes_CandidatId",
                table: "Candidatures");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_Personnes_CandidatId",
                table: "Candidatures",
                column: "CandidatId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_Personnes_CandidatId",
                table: "Candidatures");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_Personnes_CandidatId",
                table: "Candidatures",
                column: "CandidatId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
