using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CevatBiyik.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeoRedirects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "PracticeAreas",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "PracticeAreas",
                type: "varchar(320)",
                maxLength: 320,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "PracticeAreas",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OpenGraphDescription",
                table: "PracticeAreas",
                type: "varchar(320)",
                maxLength: 320,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "OpenGraphMediaId",
                table: "PracticeAreas",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenGraphTitle",
                table: "PracticeAreas",
                type: "varchar(120)",
                maxLength: 120,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RedirectRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourcePath = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinationPath = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedirectRules", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_unicode_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAreas_OpenGraphMediaId",
                table: "PracticeAreas",
                column: "OpenGraphMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_RedirectRules_SourcePath",
                table: "RedirectRules",
                column: "SourcePath",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PracticeAreas_MediaAssets_OpenGraphMediaId",
                table: "PracticeAreas",
                column: "OpenGraphMediaId",
                principalTable: "MediaAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PracticeAreas_MediaAssets_OpenGraphMediaId",
                table: "PracticeAreas");

            migrationBuilder.DropTable(
                name: "RedirectRules");

            migrationBuilder.DropIndex(
                name: "IX_PracticeAreas_OpenGraphMediaId",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "OpenGraphDescription",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "OpenGraphMediaId",
                table: "PracticeAreas");

            migrationBuilder.DropColumn(
                name: "OpenGraphTitle",
                table: "PracticeAreas");
        }
    }
}
