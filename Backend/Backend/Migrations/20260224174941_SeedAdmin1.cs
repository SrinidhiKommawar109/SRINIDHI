using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AQAAAAIAAYagAAAAEGc41NIa7ewK0Z3m2djNGY/t3PlG3PE9CSpjqWtiHDL51FJobm+bmw24N0WMXfRo4w==", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "Role" },
                values: new object[] { new DateTime(2026, 2, 24, 17, 11, 37, 814, DateTimeKind.Utc).AddTicks(8212), "AQAAAAIAAYagAAAAEMD0BevMoGinwCu4gV8p+RyfHOZAIp3EgEnkaPXqacboU8wEzJVE/kAVQq2N9+fV2Q==", 1 });
        }
    }
}
