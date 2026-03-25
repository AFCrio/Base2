using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGroupHeaderTemplateFromDutySectionNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupHeaderTemplate",
                table: "DutySectionNodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupHeaderTemplate",
                table: "DutySectionNodes",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }
    }
}
