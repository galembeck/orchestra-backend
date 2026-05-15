using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyOnboardingConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ClosingHour",
                table: "TBCompany",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpeningHour",
                table: "TBCompany",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Schedule",
                table: "TBCompany",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceRadius",
                table: "TBCompany",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypes",
                table: "TBCompany",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamSize",
                table: "TBCompany",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingHour",
                table: "TBCompany");

            migrationBuilder.DropColumn(
                name: "OpeningHour",
                table: "TBCompany");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "TBCompany");

            migrationBuilder.DropColumn(
                name: "ServiceRadius",
                table: "TBCompany");

            migrationBuilder.DropColumn(
                name: "ServiceTypes",
                table: "TBCompany");

            migrationBuilder.DropColumn(
                name: "TeamSize",
                table: "TBCompany");
        }
    }
}
