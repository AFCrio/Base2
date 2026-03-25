using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddDutyAssignmentRenderedLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RenderedLine",
                table: "DutyAssignments",
                type: "TEXT",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenderedLine",
                table: "DutyAssignments");
        }
    }
}
