using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class AddBrandModelModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    BrandID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "BrandModels",
                columns: table => new
                {
                    ModelID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BrandID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandModels", x => x.ModelID);
                    table.ForeignKey(
                        name: "FK_BrandModels_Brand_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brand",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModelYears",
                columns: table => new
                {
                    ModelYearID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<string>(type: "text", nullable: true),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModelID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelYears", x => x.ModelYearID);
                    table.ForeignKey(
                        name: "FK_ModelYears_BrandModels_ModelID",
                        column: x => x.ModelID,
                        principalTable: "BrandModels",
                        principalColumn: "ModelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandModels_BrandID",
                table: "BrandModels",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_ModelYears_ModelID",
                table: "ModelYears",
                column: "ModelID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelYears");

            migrationBuilder.DropTable(
                name: "BrandModels");

            migrationBuilder.DropTable(
                name: "Brand");
        }
    }
}
