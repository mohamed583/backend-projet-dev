using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projetERP.Migrations
{
    /// <inheritdoc />
    public partial class Migration06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EstApprouve",
                table: "Evaluations",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "EstApprouve",
                table: "Evaluations",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
