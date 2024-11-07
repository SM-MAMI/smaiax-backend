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
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PositiveActivePower = table.Column<double>(type: "double precision", nullable: false),
                    PositiveActiveEnergyTotal = table.Column<double>(type: "double precision", nullable: false),
                    NegativeActivePower = table.Column<double>(type: "double precision", nullable: false),
                    NegativeActiveEnergyTotal = table.Column<double>(type: "double precision", nullable: false),
                    ReactiveEnergyQuadrant1Total = table.Column<double>(type: "double precision", nullable: false),
                    ReactiveEnergyQuadrant3Total = table.Column<double>(type: "double precision", nullable: false),
                    TotalPower = table.Column<double>(type: "double precision", nullable: false),
                    CurrentPhase1 = table.Column<double>(type: "double precision", nullable: false),
                    VoltagePhase1 = table.Column<double>(type: "double precision", nullable: false),
                    CurrentPhase2 = table.Column<double>(type: "double precision", nullable: false),
                    VoltagePhase2 = table.Column<double>(type: "double precision", nullable: false),
                    CurrentPhase3 = table.Column<double>(type: "double precision", nullable: false),
                    VoltagePhase3 = table.Column<double>(type: "double precision", nullable: false),
                    Uptime = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    householdSize = table.Column<int>(type: "integer", nullable: false),
                    streetName = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    country = table.Column<string>(type: "text", nullable: true),
                    continent = table.Column<string>(type: "text", nullable: true),
                    locationResolution = table.Column<string>(type: "text", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    policy_state = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    MeasurementResolution = table.Column<string>(type: "text", nullable: false),
                    MinHouseHoldSize = table.Column<int>(type: "integer", nullable: false),
                    MaxHouseHoldSize = table.Column<int>(type: "integer", nullable: false),
                    policyFilter_Locations = table.Column<string>(type: "text", nullable: false),
                    LocationResolution = table.Column<string>(type: "text", nullable: false),
                    MaxPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    StreetName = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Continent = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "PolicySmartMeter",
                schema: "domain",
                columns: table => new
                {
                    policyId = table.Column<Guid>(type: "uuid", nullable: false),
                    smartMeterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pK_PolicySmartMeter", x => new { x.policyId, x.smartMeterId });
                    table.ForeignKey(
                        name: "fK_PolicySmartMeter_Policy_policyId",
                        column: x => x.policyId,
                        principalSchema: "domain",
                        principalTable: "Policy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fK_PolicySmartMeter_SmartMeter_smartMeterId",
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
                name: "iX_PolicySmartMeter_smartMeterId",
                schema: "domain",
                table: "PolicySmartMeter",
                column: "smartMeterId");
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
                name: "PolicyRequest",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "PolicySmartMeter",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "Policy",
                schema: "domain");

            migrationBuilder.DropTable(
                name: "SmartMeter",
                schema: "domain");
        }
    }
}