using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToSectionNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "DutySectionNodes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_LocationId",
                table: "DutySectionNodes",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DutySectionNodes_Locations_LocationId",
                table: "DutySectionNodes",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutySectionNodes_Locations_LocationId",
                table: "DutySectionNodes");

            migrationBuilder.DropIndex(
                name: "IX_DutySectionNodes_LocationId",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "DutySectionNodes");
        }
    }
}
