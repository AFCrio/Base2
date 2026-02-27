using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemplateNode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateNodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateNodes",
                columns: table => new
                {
                    TemplateNodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemSeparator = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    NodeType = table.Column<string>(type: "TEXT", nullable: false),
                    RenderMode = table.Column<string>(type: "TEXT", nullable: false),
                    RequiresAmmo = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresVehicle = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresWeapon = table.Column<bool>(type: "INTEGER", nullable: false),
                    Separator = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TemplateText = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Terminator = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateNodes", x => x.TemplateNodeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateNodes_NodeType",
                table: "TemplateNodes",
                column: "NodeType",
                unique: true);
        }
    }
}
