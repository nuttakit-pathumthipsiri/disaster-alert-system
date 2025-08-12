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
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    ThresholdValue = table.Column<double>(type: "float", nullable: false),
                    EmailSent = table.Column<bool>(type: "bit", nullable: false),
                    EmailSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AlertMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExternalApiData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Regions_RegionId",
                        column: x => x.RegionId,
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
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegionId",
                table: "Users",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RegionId_DisasterTypeId",
                table: "Alerts",
                columns: new[] { "RegionId", "DisasterTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_EmailSent",
                table: "Alerts",
                column: "EmailSent");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DetectedAt",
                table: "Alerts",
                column: "DetectedAt");

            // Seed data for DisasterTypes
            migrationBuilder.InsertData(
                table: "DisasterTypes",
                columns: new[] { "Name", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { "Earthquake", true, DateTime.UtcNow },
                    { "Flood", true, DateTime.UtcNow },
                    { "Wildfire", true, DateTime.UtcNow }
                });

            // Seed data for Regions
            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Name", "Latitude", "Longitude", "MonitoredDisasterTypes", "CreatedAt" },
                values: new object[,]
                {
                    { "กรุงเทพมหานคร", 13.7563, 100.5018, "[2,3]", DateTime.UtcNow }, // Flood(2), Wildfire(3)
                    { "เชียงใหม่", 18.7883, 98.9853, "[1,3]", DateTime.UtcNow }, // Earthquake(1), Wildfire(3)
                    { "ภูเก็ต", 7.8804, 98.3923, "[2,3]", DateTime.UtcNow }, // Flood(2), Wildfire(3)
                    { "อุบลราชธานี", 15.2288, 104.8594, "[2,3]", DateTime.UtcNow }, // Flood(2), Wildfire(3)
                    { "สงขลา", 7.1907, 100.5951, "[2,3]", DateTime.UtcNow } // Flood(2), Wildfire(3)
                });

            // Seed data for AlertSettings with realistic thresholds based on actual data
            migrationBuilder.InsertData(
                table: "AlertSettings",
                columns: new[] { "RegionId", "DisasterTypeId", "ThresholdRiskScore", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    // Bangkok (Region ID: 1) - Monitor Flood, Wildfire
                    { 1, 2, 25.0, true, DateTime.UtcNow }, // Flood: 25.0 (based on actual data showing 0 risk)
                    { 1, 3, 70.0, true, DateTime.UtcNow }, // Wildfire: 70.0 (based on actual data showing 75 risk)
                    
                    // Chiang Mai (Region ID: 2) - Monitor Earthquake, Wildfire
                    { 2, 1, 70.0, true, DateTime.UtcNow }, // Earthquake: 70.0 (based on actual data showing 75 risk)
                    { 2, 3, 70.0, true, DateTime.UtcNow }, // Wildfire: 70.0 (based on actual data showing 100 risk)
                    
                    // Phuket (Region ID: 3) - Monitor Flood, Wildfire
                    { 3, 2, 25.0, true, DateTime.UtcNow }, // Flood: 25.0 (based on actual data showing 0 risk)
                    { 3, 3, 70.0, true, DateTime.UtcNow }, // Wildfire: 70.0 (based on actual data showing 75 risk)
                    
                    // Ubon Ratchathani (Region ID: 4) - Monitor Flood, Wildfire
                    { 4, 2, 70.0, true, DateTime.UtcNow }, // Flood: 70.0 (based on actual data showing 75 risk)
                    { 4, 3, 70.0, true, DateTime.UtcNow }, // Wildfire: 70.0 (based on actual data showing 75 risk)
                    
                    // Songkhla (Region ID: 5) - Monitor Flood, Wildfire
                    { 5, 2, 70.0, true, DateTime.UtcNow }, // Flood: 70.0 (based on actual data showing 75 risk)
                    { 5, 3, 70.0, true, DateTime.UtcNow }  // Wildfire: 70.0 (based on actual data showing 75 risk)
                });

            // Seed data for Users (sample users for each region)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "RegionId", "Email", "Phone", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    // Bangkok (Region ID: 1) - Sample users
                    { 1, "user.bangkok1@example.com", "+66-81-234-5678", true, DateTime.UtcNow },
                    { 1, "user.bangkok2@example.com", "+66-82-345-6789", true, DateTime.UtcNow },
                    
                    // Chiang Mai (Region ID: 2) - Sample users
                    { 2, "user.chiangmai1@example.com", "+66-83-456-7890", true, DateTime.UtcNow },
                    { 2, "user.chiangmai2@example.com", "+66-84-567-8901", true, DateTime.UtcNow },
                    
                    // Phuket (Region ID: 3) - Sample users
                    { 3, "user.phuket1@example.com", "+66-85-678-9012", true, DateTime.UtcNow },
                    { 3, "user.phuket2@example.com", "+66-86-789-0123", true, DateTime.UtcNow },
                    
                    // Ubon Ratchathani (Region ID: 4) - Sample users
                    { 4, "user.ubon1@example.com", "+66-87-890-1234", true, DateTime.UtcNow },
                    
                    // Songkhla (Region ID: 5) - Sample users
                    { 5, "user.songkhla1@example.com", "+66-88-901-2345", true, DateTime.UtcNow }
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
                name: "Users");

            migrationBuilder.DropTable(
                name: "DisasterTypes");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
