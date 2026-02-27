using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "PropertyPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "PolicyRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InstallmentAmount",
                table: "PolicyRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InstallmentCount",
                table: "PolicyRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPremium",
                table: "PolicyRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "PolicyRequests",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Frequency", "InstallmentAmount", "InstallmentCount", "TotalPremium" },
                values: new object[] { 0, 0m, 0, 0m });

            migrationBuilder.UpdateData(
                table: "PolicyRequests",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Frequency", "InstallmentAmount", "InstallmentCount", "TotalPremium" },
                values: new object[] { 0, 0m, 0, 0m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Frequency",
                value: 3);

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Frequency",
                value: 1);

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "Frequency",
                value: 2);

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 4,
                column: "Frequency",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "PropertyPlans");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "PolicyRequests");

            migrationBuilder.DropColumn(
                name: "InstallmentAmount",
                table: "PolicyRequests");

            migrationBuilder.DropColumn(
                name: "InstallmentCount",
                table: "PolicyRequests");

            migrationBuilder.DropColumn(
                name: "TotalPremium",
                table: "PolicyRequests");
        }
    }
}
