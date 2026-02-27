using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddWeaponAssignedToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToPersonId",
                table: "Weapons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_AssignedToPersonId",
                table: "Weapons",
                column: "AssignedToPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Weapons_People_AssignedToPersonId",
                table: "Weapons",
                column: "AssignedToPersonId",
                principalTable: "People",
                principalColumn: "PersonId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Weapons_People_AssignedToPersonId",
                table: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_Weapons_AssignedToPersonId",
                table: "Weapons");

            migrationBuilder.DropColumn(
                name: "AssignedToPersonId",
                table: "Weapons");
        }
    }
}
