using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBService",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ReviewsCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBServiceCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBServiceCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBService_CategoryId",
                table: "TBService",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TBService_CompanyId",
                table: "TBService",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TBServiceCategory_Slug",
                table: "TBServiceCategory",
                column: "Slug",
                unique: true);

            // Seed default service categories.
            var now = DateTimeOffset.UtcNow;
            var seed = new (string Id, string Name, string Slug, string Icon)[]
            {
                ("c1f0a2e8-0001-4000-8000-000000000001", "Hidráulica",        "hidraulica",      "Wrench"),
                ("c1f0a2e8-0001-4000-8000-000000000002", "Elétrica",          "eletrica",        "Zap"),
                ("c1f0a2e8-0001-4000-8000-000000000003", "Reparos gerais",    "reparos-gerais",  "Hammer"),
                ("c1f0a2e8-0001-4000-8000-000000000004", "Instalação",        "instalacao",      "Drill"),
                ("c1f0a2e8-0001-4000-8000-000000000005", "Mudanças",          "mudancas",        "Truck"),
            };

            foreach (var (id, name, slug, icon) in seed)
            {
                migrationBuilder.InsertData(
                    table: "TBServiceCategory",
                    columns: new[] { "Id", "Name", "Slug", "Icon", "IsActive", "CreatedBy", "UpdatedBy", "CreatedAt", "UpdatedAt", "DeletedAt" },
                    values: new object[] { id, name, slug, icon, true, "system", "system", now, now, null });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBService");

            migrationBuilder.DropTable(
                name: "TBServiceCategory");
        }
    }
}
