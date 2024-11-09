#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SMAIAXBackend.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "domain");

            migrationBuilder.CreateTable(
                name: "Measurement",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measurement_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    positiveActivePower = table.Column<double>(type: "double precision", nullable: false),
                    positiveActiveEnergyTotal = table.Column<double>(type: "double precision", nullable: false),
                    negativeActivePower = table.Column<double>(type: "double precision", nullable: false),
                    negativeActiveEnergyTotal = table.Column<double>(type: "double precision", nullable: false),
                    reactiveEnergyQuadrant1Total = table.Column<double>(type: "double precision", nullable: false),
                    reactiveEnergyQuadrant3Total = table.Column<double>(type: "double precision", nullable: false),
                    totalPower = table.Column<double>(type: "double precision", nullable: false),
                    currentPhase1 = table.Column<double>(type: "double precision", nullable: false),
                    voltagePhase1 = table.Column<double>(type: "double precision", nullable: false),
                    currentPhase2 = table.Column<double>(type: "double precision", nullable: false),
                    voltagePhase2 = table.Column<double>(type: "double precision", nullable: false),
                    currentPhase3 = table.Column<double>(type: "double precision", nullable: false),
                    voltagePhase3 = table.Column<double>(type: "double precision", nullable: false),
                    uptime = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    smartMeterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Measurement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Policy",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    measurementResolution = table.Column<string>(type: "text", nullable: false),
                    locationResolution = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    smartMeterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Policy", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRequest",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isAutomaticContractingEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    measurementResolution = table.Column<string>(type: "text", nullable: false),
                    minHouseHoldSize = table.Column<int>(type: "integer", nullable: false),
                    maxHouseHoldSize = table.Column<int>(type: "integer", nullable: false),
                    locations = table.Column<string>(type: "text", nullable: false),
                    locationResolution = table.Column<string>(type: "text", nullable: false),
                    maxPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_PolicyRequest", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SmartMeter",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_SmartMeter", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Metadata",
                schema: "domain",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    validFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    streetName = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    country = table.Column<string>(type: "text", nullable: true),
                    continent = table.Column<string>(type: "text", nullable: true),
                    householdSize = table.Column<int>(type: "integer", nullable: false),
                    smartMeterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_Metadata", x => x.id);
                    table.ForeignKey(
                        name: "fK_Metadata_SmartMeter_smartMeterId",
                        column: x => x.smartMeterId,
                        principalSchema: "domain",
                        principalTable: "SmartMeter",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "iX_Metadata_smartMeterId",
                schema: "domain",
                table: "Metadata",
                column: "smartMeterId");

            migrationBuilder.CreateIndex(
                name: "iX_Metadata_validFrom",
                schema: "domain",
                table: "Metadata",
                column: "validFrom",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measurement",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "Metadata",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "Policy",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "PolicyRequest",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "SmartMeter",
                schema: "domain");
        }
    }
}