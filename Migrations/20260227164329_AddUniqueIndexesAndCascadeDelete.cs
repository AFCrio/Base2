using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesAndCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutySectionNodes_DutySectionNodes_ParentDutySectionNodeId",
                table: "DutySectionNodes");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionName",
                table: "Positions",
                column: "PositionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_LocationName",
                table: "Locations",
                column: "LocationName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DutySectionNodes_DutySectionNodes_ParentDutySectionNodeId",
                table: "DutySectionNodes",
                column: "ParentDutySectionNodeId",
                principalTable: "DutySectionNodes",
                principalColumn: "DutySectionNodeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutySectionNodes_DutySectionNodes_ParentDutySectionNodeId",
                table: "DutySectionNodes");

            migrationBuilder.DropIndex(
                name: "IX_Positions_PositionName",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Locations_LocationName",
                table: "Locations");

            migrationBuilder.AddForeignKey(
                name: "FK_DutySectionNodes_DutySectionNodes_ParentDutySectionNodeId",
                table: "DutySectionNodes",
                column: "ParentDutySectionNodeId",
                principalTable: "DutySectionNodes",
                principalColumn: "DutySectionNodeId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
