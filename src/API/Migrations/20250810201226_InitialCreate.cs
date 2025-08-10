using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisasterTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    MonitoredDisasterTypes = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    DisasterTypeId = table.Column<int>(type: "int", nullable: false),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    AlertMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_DisasterTypes_DisasterTypeId",
                        column: x => x.DisasterTypeId,
                        principalTable: "DisasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Alerts_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    DisasterTypeId = table.Column<int>(type: "int", nullable: false),
                    ThresholdRiskScore = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertSettings_DisasterTypes_DisasterTypeId",
                        column: x => x.DisasterTypeId,
                        principalTable: "DisasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertSettings_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisasterRisks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    DisasterTypeId = table.Column<int>(type: "int", nullable: false),
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    ShouldTriggerAlert = table.Column<bool>(type: "bit", nullable: false),
                    ThresholdValue = table.Column<double>(type: "float", nullable: false),
                    ExternalApiData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterRisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisasterRisks_DisasterTypes_DisasterTypeId",
                        column: x => x.DisasterTypeId,
                        principalTable: "DisasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisasterRisks_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisasterTypeRegion",
                columns: table => new
                {
                    DisasterTypesId = table.Column<int>(type: "int", nullable: false),
                    RegionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisasterTypeRegion", x => new { x.DisasterTypesId, x.RegionsId });
                    table.ForeignKey(
                        name: "FK_DisasterTypeRegion_DisasterTypes_DisasterTypesId",
                        column: x => x.DisasterTypesId,
                        principalTable: "DisasterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DisasterTypeRegion_Regions_RegionsId",
                        column: x => x.RegionsId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DisasterTypeId",
                table: "Alerts",
                column: "DisasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RegionId",
                table: "Alerts",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertSettings_DisasterTypeId",
                table: "AlertSettings",
                column: "DisasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertSettings_RegionId",
                table: "AlertSettings",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterRisks_DisasterTypeId",
                table: "DisasterRisks",
                column: "DisasterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterRisks_RegionId",
                table: "DisasterRisks",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_DisasterTypeRegion_RegionsId",
                table: "DisasterTypeRegion",
                column: "RegionsId");

            // Seed data for DisasterTypes
            migrationBuilder.InsertData(
                table: "DisasterTypes",
                columns: new[] { "Name", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { "Earthquake", true, DateTime.UtcNow },
                    { "Tsunami", true, DateTime.UtcNow },
                    { "Flood", true, DateTime.UtcNow },
                    { "Landslide", true, DateTime.UtcNow },
                    { "Wildfire", true, DateTime.UtcNow },
                    { "Storm", true, DateTime.UtcNow },
                    { "Drought", true, DateTime.UtcNow }
                });

            // Seed data for Regions
            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Name", "Latitude", "Longitude", "MonitoredDisasterTypes", "CreatedAt" },
                values: new object[,]
                {
                    { "กรุงเทพมหานคร", 13.7563, 100.5018, "[\"Flood\",\"Storm\",\"Drought\"]", DateTime.UtcNow },
                    { "เชียงใหม่", 18.7883, 98.9853, "[\"Earthquake\",\"Landslide\",\"Wildfire\"]", DateTime.UtcNow },
                    { "ภูเก็ต", 7.8804, 98.3923, "[\"Tsunami\",\"Storm\",\"Flood\"]", DateTime.UtcNow },
                    { "อุบลราชธานี", 15.2288, 104.8594, "[\"Flood\",\"Drought\",\"Storm\"]", DateTime.UtcNow },
                    { "สงขลา", 7.1907, 100.5951, "[\"Flood\",\"Storm\",\"Landslide\"]", DateTime.UtcNow }
                });

            // Seed data for AlertSettings
            migrationBuilder.InsertData(
                table: "AlertSettings",
                columns: new[] { "RegionId", "DisasterTypeId", "ThresholdRiskScore", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 3, 7.5, true, DateTime.UtcNow }, // กรุงเทพฯ - Flood
                    { 1, 6, 6.0, true, DateTime.UtcNow }, // กรุงเทพฯ - Storm
                    { 1, 7, 8.0, true, DateTime.UtcNow }, // กรุงเทพฯ - Drought
                    { 2, 1, 8.5, true, DateTime.UtcNow }, // เชียงใหม่ - Earthquake
                    { 2, 4, 7.0, true, DateTime.UtcNow }, // เชียงใหม่ - Landslide
                    { 2, 5, 6.5, true, DateTime.UtcNow }, // เชียงใหม่ - Wildfire
                    { 3, 2, 9.0, true, DateTime.UtcNow }, // ภูเก็ต - Tsunami
                    { 3, 6, 7.0, true, DateTime.UtcNow }, // ภูเก็ต - Storm
                    { 3, 3, 6.5, true, DateTime.UtcNow }, // ภูเก็ต - Flood
                    { 4, 3, 8.0, true, DateTime.UtcNow }, // อุบลราชธานี - Flood
                    { 4, 7, 7.5, true, DateTime.UtcNow }, // อุบลราชธานี - Drought
                    { 4, 6, 6.5, true, DateTime.UtcNow }, // อุบลราชธานี - Storm
                    { 5, 3, 7.0, true, DateTime.UtcNow }, // สงขลา - Flood
                    { 5, 6, 6.5, true, DateTime.UtcNow }, // สงขลา - Storm
                    { 5, 4, 7.5, true, DateTime.UtcNow }  // สงขลา - Landslide
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "AlertSettings");

            migrationBuilder.DropTable(
                name: "DisasterRisks");

            migrationBuilder.DropTable(
                name: "DisasterTypeRegion");

            migrationBuilder.DropTable(
                name: "DisasterTypes");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
