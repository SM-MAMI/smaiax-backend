using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMAIAXBackend.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class MeasurementToHypertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "SELECT CREATE_HYPERTABLE('domain.\"Measurement\"', 'timestamp');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE TABLE domain.""Measurement_New"" AS SELECT * FROM domain.""Measurement"";
                  DROP TABLE domain.""Measurement"";
                  ALTER TABLE domain.""Measurement_New"" rename to domain.""Measurement"";");
        }
    }
}