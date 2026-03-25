using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupTemplatesToDutySectionNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupItemTemplate",
                table: "DutySectionNodes",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupItemTemplate",
                table: "DutySectionNodes");
        }
    }
}
