using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paies_Personnes_EmployeId",
                table: "Paies");

            migrationBuilder.RenameColumn(
                name: "EmployeId",
                table: "Paies",
                newName: "PersonneId");

            migrationBuilder.RenameIndex(
                name: "IX_Paies_EmployeId",
                table: "Paies",
                newName: "IX_Paies_PersonneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paies_Personnes_PersonneId",
                table: "Paies",
                column: "PersonneId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paies_Personnes_PersonneId",
                table: "Paies");

            migrationBuilder.RenameColumn(
                name: "PersonneId",
                table: "Paies",
                newName: "EmployeId");

            migrationBuilder.RenameIndex(
                name: "IX_Paies_PersonneId",
                table: "Paies",
                newName: "IX_Paies_EmployeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paies_Personnes_EmployeId",
                table: "Paies",
                column: "EmployeId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
