using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class AddVersioningAndChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "DutyTemplates",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "SourceTemplateVersion",
                table: "DutyOrders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            // Ініціалізуємо існуючі дані
            migrationBuilder.Sql("UPDATE DutyTemplates SET Version = 1 WHERE Version = 0;");
            migrationBuilder.Sql("UPDATE DutyOrders SET SourceTemplateVersion = 1 WHERE SourceTemplateVersion = 0;");

            migrationBuilder.CreateTable(
                name: "TemplateChangeLogs",
                columns: table => new
                {
                    TemplateChangeLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DutyTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ChangeDescription = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateChangeLogs", x => x.TemplateChangeLogId);
                    table.ForeignKey(
                        name: "FK_TemplateChangeLogs_DutyTemplates_DutyTemplateId",
                        column: x => x.DutyTemplateId,
                        principalTable: "DutyTemplates",
                        principalColumn: "DutyTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateChangeLogs_DutyTemplateId",
                table: "TemplateChangeLogs",
                column: "DutyTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateChangeLogs_DutyTemplateId_Version",
                table: "TemplateChangeLogs",
                columns: new[] { "DutyTemplateId", "Version" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateChangeLogs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "DutyTemplates");

            migrationBuilder.DropColumn(
                name: "SourceTemplateVersion",
                table: "DutyOrders");
        }
    }
}
