using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CevatBiyik.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContactNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationAttempts",
                table: "ContactMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NotificationLeaseId",
                table: "ContactMessages",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationLeaseUntil",
                table: "ContactMessages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationNextAttemptAt",
                table: "ContactMessages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationPending",
                table: "ContactMessages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NotificationSentAt",
                table: "ContactMessages",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_IsDeleted_NotificationPending_NotificationNe~",
                table: "ContactMessages",
                columns: new[] { "IsDeleted", "NotificationPending", "NotificationNextAttemptAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_IsDeleted_NotificationPending_NotificationNe~",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationAttempts",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationLeaseId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationLeaseUntil",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationNextAttemptAt",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationPending",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "NotificationSentAt",
                table: "ContactMessages");
        }
    }
}
