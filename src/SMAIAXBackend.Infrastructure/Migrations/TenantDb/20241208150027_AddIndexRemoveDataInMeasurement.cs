using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMAIAXBackend.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class AddIndexRemoveDataInMeasurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pK_Measurement",
                schema: "domain",
                table: "Measurement");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "domain",
                table: "Measurement");

            migrationBuilder.DropColumn(
                name: "measurement_timestamp",
                schema: "domain",
                table: "Measurement");

            migrationBuilder.CreateIndex(
                name: "iX_Measurement_smartMeterId",
                schema: "domain",
                table: "Measurement",
                column: "smartMeterId");

            migrationBuilder.CreateIndex(
                name: "iX_Measurement_timestamp",
                schema: "domain",
                table: "Measurement",
                column: "timestamp",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "iX_Measurement_smartMeterId",
                schema: "domain",
                table: "Measurement");

            migrationBuilder.DropIndex(
                name: "iX_Measurement_timestamp",
                schema: "domain",
                table: "Measurement");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "domain",
                table: "Measurement",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "measurement_timestamp",
                schema: "domain",
                table: "Measurement",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "pK_Measurement",
                schema: "domain",
                table: "Measurement",
                column: "id");
        }
    }
}