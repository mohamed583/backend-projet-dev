using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatutPoste",
                table: "Postes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatutPoste",
                table: "Postes");
        }
    }
}
