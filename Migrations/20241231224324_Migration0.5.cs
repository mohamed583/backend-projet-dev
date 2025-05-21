using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "objectifs",
                table: "Evaluations",
                newName: "Objectifs");

            migrationBuilder.RenameColumn(
                name: "Commentaires",
                table: "Evaluations",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "CommentairesEmploye",
                table: "Evaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CommentairesResponsable",
                table: "Evaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EstApprouve",
                table: "Evaluations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FinaliseParEmploye",
                table: "Evaluations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FinaliseParManager",
                table: "Evaluations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableId",
                table: "Evaluations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ResponsableId",
                table: "Evaluations",
                column: "ResponsableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Personnes_ResponsableId",
                table: "Evaluations",
                column: "ResponsableId",
                principalTable: "Personnes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Personnes_ResponsableId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_ResponsableId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "CommentairesEmploye",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "CommentairesResponsable",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "EstApprouve",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "FinaliseParEmploye",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "FinaliseParManager",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ResponsableId",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "Objectifs",
                table: "Evaluations",
                newName: "objectifs");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Evaluations",
                newName: "Commentaires");
        }
    }
}
