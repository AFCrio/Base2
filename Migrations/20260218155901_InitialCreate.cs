using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Base2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DutyOrders",
                columns: table => new
                {
                    DutyOrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OrderDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CommanderInfo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyOrders", x => x.DutyOrderId);
                });

            migrationBuilder.CreateTable(
                name: "DutyTimeRanges",
                columns: table => new
                {
                    DutyTimeRangeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyTimeRanges", x => x.DutyTimeRangeId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocationId);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PositionName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    HasWeapon = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasAmmo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDriver = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                });

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    RankId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RankName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    RankLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.RankId);
                });

            migrationBuilder.CreateTable(
                name: "TemplateNodes",
                columns: table => new
                {
                    TemplateNodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NodeType = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateText = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    RequiresWeapon = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresAmmo = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresVehicle = table.Column<bool>(type: "INTEGER", nullable: false),
                    RenderMode = table.Column<string>(type: "TEXT", nullable: false),
                    Separator = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ItemSeparator = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Terminator = table.Column<string>(type: "TEXT", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateNodes", x => x.TemplateNodeId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VehicleNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VehicleType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "DutySectionNodes",
                columns: table => new
                {
                    DutySectionNodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentDutySectionNodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    DutyOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    NodeType = table.Column<string>(type: "TEXT", nullable: false),
                    OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DutyPositionTitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LocationText = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    DutyTimeRangeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutySectionNodes", x => x.DutySectionNodeId);
                    table.ForeignKey(
                        name: "FK_DutySectionNodes_DutyOrders_DutyOrderId",
                        column: x => x.DutyOrderId,
                        principalTable: "DutyOrders",
                        principalColumn: "DutyOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DutySectionNodes_DutySectionNodes_ParentDutySectionNodeId",
                        column: x => x.ParentDutySectionNodeId,
                        principalTable: "DutySectionNodes",
                        principalColumn: "DutySectionNodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DutySectionNodes_DutyTimeRanges_DutyTimeRangeId",
                        column: x => x.DutyTimeRangeId,
                        principalTable: "DutyTimeRanges",
                        principalColumn: "DutyTimeRangeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WeaponType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WeaponNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StoredInLocationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.WeaponId);
                    table.ForeignKey(
                        name: "FK_Weapons_Locations_StoredInLocationId",
                        column: x => x.StoredInLocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Initials = table.Column<string>(type: "TEXT", nullable: true),
                    RankId = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_People_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_People_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DutyAssignments",
                columns: table => new
                {
                    DutyAssignmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DutySectionNodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: true),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: true),
                    AmmoCount = table.Column<int>(type: "INTEGER", nullable: true),
                    AmmoType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DutyAssignments", x => x.DutyAssignmentId);
                    table.ForeignKey(
                        name: "FK_DutyAssignments_DutySectionNodes_DutySectionNodeId",
                        column: x => x.DutySectionNodeId,
                        principalTable: "DutySectionNodes",
                        principalColumn: "DutySectionNodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DutyAssignments_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DutyAssignments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DutyAssignments_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "WeaponId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DutyAssignments_DutySectionNodeId",
                table: "DutyAssignments",
                column: "DutySectionNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyAssignments_PersonId",
                table: "DutyAssignments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyAssignments_VehicleId",
                table: "DutyAssignments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyAssignments_WeaponId",
                table: "DutyAssignments",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_DutyOrders_OrderNumber",
                table: "DutyOrders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_DutyOrderId",
                table: "DutySectionNodes",
                column: "DutyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_DutyTimeRangeId",
                table: "DutySectionNodes",
                column: "DutyTimeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_ParentDutySectionNodeId",
                table: "DutySectionNodes",
                column: "ParentDutySectionNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_DutySectionNodes_ParentDutySectionNodeId_OrderIndex",
                table: "DutySectionNodes",
                columns: new[] { "ParentDutySectionNodeId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_People_LastName",
                table: "People",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_People_PositionId",
                table: "People",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_People_RankId",
                table: "People",
                column: "RankId");

            migrationBuilder.CreateIndex(
                name: "IX_Ranks_RankName",
                table: "Ranks",
                column: "RankName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemplateNodes_NodeType",
                table: "TemplateNodes",
                column: "NodeType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleNumber",
                table: "Vehicles",
                column: "VehicleNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_StoredInLocationId",
                table: "Weapons",
                column: "StoredInLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_WeaponNumber",
                table: "Weapons",
                column: "WeaponNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DutyAssignments");

            migrationBuilder.DropTable(
                name: "TemplateNodes");

            migrationBuilder.DropTable(
                name: "DutySectionNodes");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropTable(
                name: "DutyOrders");

            migrationBuilder.DropTable(
                name: "DutyTimeRanges");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Ranks");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
