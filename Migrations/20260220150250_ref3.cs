using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class ref3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ═══ 1. Создаём DutyTemplates ПЕРВОЙ ═══
            migrationBuilder.CreateTable(
                name: "DutyTemplates",
                columns: table => new
                {
                    DutyTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TemplateName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyTemplates", x => x.DutyTemplateId);
                });

            // ═══ 2. Шаблон по умолчанию (ID=1) для существующих DutyOrders ═══
            migrationBuilder.Sql(
                "INSERT INTO DutyTemplates (TemplateName, Description, IsActive, CreatedAt, UpdatedAt) " +
                "VALUES ('Шаблон за замовчуванням', 'Автоматично створений', 1, datetime('now'), datetime('now'));");

            // ═══ 3. DutyOrders: SourceTemplateId = 1 (ДО пересоздания таблицы!) ═══
            migrationBuilder.AddColumn<int>(
                name: "SourceTemplateId", table: "DutyOrders",
                type: "INTEGER", nullable: false, defaultValue: 1);
            migrationBuilder.Sql("UPDATE DutyOrders SET SourceTemplateId = 1;");

            // ═══ 4. Positions: убираем поля ═══
            migrationBuilder.DropColumn(name: "HasAmmo", table: "Positions");
            migrationBuilder.DropColumn(name: "HasWeapon", table: "Positions");
            migrationBuilder.DropColumn(name: "IsDriver", table: "Positions");

            // ═══ 5. DutySectionNodes: убираем LocationText ═══
            migrationBuilder.DropColumn(name: "LocationText", table: "DutySectionNodes");

            // ═══ 6. DutyTimeRanges: Label + DutyOrderId ═══
            migrationBuilder.AddColumn<string>(
                name: "Label", table: "DutyTimeRanges", type: "TEXT",
                maxLength: 200, nullable: false, defaultValue: "Зміна");
            migrationBuilder.AddColumn<int>(
                name: "DutyOrderId", table: "DutyTimeRanges",
                type: "INTEGER", nullable: false, defaultValue: 0);
            migrationBuilder.Sql(
                "UPDATE DutyTimeRanges SET DutyOrderId = " +
                "(SELECT DutyOrderId FROM DutyOrders LIMIT 1) " +
                "WHERE DutyOrderId = 0 AND EXISTS (SELECT 1 FROM DutyOrders);");
            migrationBuilder.Sql("DELETE FROM DutyTimeRanges WHERE DutyOrderId = 0;");

            // ═══ 7. DutySectionNodes: новые поля ═══
            migrationBuilder.AlterColumn<int>(
                name: "DutyOrderId", table: "DutySectionNodes", type: "INTEGER",
                nullable: true, oldClrType: typeof(int), oldType: "INTEGER");
            migrationBuilder.AddColumn<int>(
                name: "DutyTemplateId", table: "DutySectionNodes",
                type: "INTEGER", nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "HasAmmo", table: "DutySectionNodes",
                type: "INTEGER", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(
                name: "HasVehicle", table: "DutySectionNodes",
                type: "INTEGER", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(
                name: "HasWeapon", table: "DutySectionNodes",
                type: "INTEGER", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<int>(
                name: "MaxAssignments", table: "DutySectionNodes",
                type: "INTEGER", nullable: false, defaultValue: 1);
            migrationBuilder.AddColumn<string>(
                name: "TimeRangeLabel", table: "DutySectionNodes",
                type: "TEXT", maxLength: 200, nullable: true);

            // ═══ 8. Индексы ═══
            migrationBuilder.CreateIndex(
                name: "IX_DutyTimeRanges_DutyOrderId",
                table: "DutyTimeRanges", column: "DutyOrderId");
            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_DutyTemplateId",
                table: "DutySectionNodes", column: "DutyTemplateId");
            migrationBuilder.CreateIndex(
                name: "IX_DutyOrders_SourceTemplateId",
                table: "DutyOrders", column: "SourceTemplateId");

            // ═══ 9. FK (данные уже заполнены корректно) ═══
            migrationBuilder.AddForeignKey(
                name: "FK_DutyOrders_DutyTemplates_SourceTemplateId",
                table: "DutyOrders", column: "SourceTemplateId",
                principalTable: "DutyTemplates", principalColumn: "DutyTemplateId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DutySectionNodes_DutyTemplates_DutyTemplateId",
                table: "DutySectionNodes", column: "DutyTemplateId",
                principalTable: "DutyTemplates", principalColumn: "DutyTemplateId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_DutyTimeRanges_DutyOrders_DutyOrderId",
                table: "DutyTimeRanges", column: "DutyOrderId",
                principalTable: "DutyOrders", principalColumn: "DutyOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DutyOrders_DutyTemplates_SourceTemplateId",
                table: "DutyOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_DutySectionNodes_DutyTemplates_DutyTemplateId",
                table: "DutySectionNodes");

            migrationBuilder.DropForeignKey(
                name: "FK_DutyTimeRanges_DutyOrders_DutyOrderId",
                table: "DutyTimeRanges");

            migrationBuilder.DropTable(
                name: "DutyTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DutyTimeRanges_DutyOrderId",
                table: "DutyTimeRanges");

            migrationBuilder.DropIndex(
                name: "IX_DutySectionNodes_DutyTemplateId",
                table: "DutySectionNodes");

            migrationBuilder.DropIndex(
                name: "IX_DutyOrders_SourceTemplateId",
                table: "DutyOrders");

            migrationBuilder.DropColumn(
                name: "DutyOrderId",
                table: "DutyTimeRanges");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "DutyTimeRanges");

            migrationBuilder.DropColumn(
                name: "DutyTemplateId",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "HasAmmo",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "HasVehicle",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "HasWeapon",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "MaxAssignments",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "TimeRangeLabel",
                table: "DutySectionNodes");

            migrationBuilder.DropColumn(
                name: "SourceTemplateId",
                table: "DutyOrders");

            migrationBuilder.AddColumn<bool>(
                name: "HasAmmo",
                table: "Positions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasWeapon",
                table: "Positions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDriver",
                table: "Positions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DutyOrderId",
                table: "DutySectionNodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationText",
                table: "DutySectionNodes",
                type: "TEXT",
                maxLength: 300,
                nullable: true);
        }
    }
}
