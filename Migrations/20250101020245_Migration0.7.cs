using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration07 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusInscription",
                table: "Conges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusInscription",
                table: "Conges");
        }
    }
}
