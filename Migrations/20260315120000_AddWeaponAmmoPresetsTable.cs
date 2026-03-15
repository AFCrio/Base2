using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddWeaponAmmoPresetsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeaponAmmoPresets",
                columns: table => new
                {
                    WeaponAmmoPresetId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WeaponType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AmmoType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AmmoCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponAmmoPresets", x => x.WeaponAmmoPresetId);
                });

            migrationBuilder.InsertData(
                table: "WeaponAmmoPresets",
                columns: new[] { "WeaponAmmoPresetId", "AmmoCount", "AmmoType", "WeaponType" },
                values: new object[,]
                {
                    { 1, 16, "9 мм", "ПМ" },
                    { 2, 120, "5,45 мм", "АК-47" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeaponAmmoPresets_WeaponType",
                table: "WeaponAmmoPresets",
                column: "WeaponType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeaponAmmoPresets");
        }
    }
}
