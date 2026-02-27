using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgentCommission",
                table: "PropertyPlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePremium",
                table: "PropertyPlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AgentCommissionAmount",
                table: "PolicyRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PremiumAmount",
                table: "PolicyRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 0m, 0m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentCommission",
                table: "PropertyPlans");

            migrationBuilder.DropColumn(
                name: "BasePremium",
                table: "PropertyPlans");

            migrationBuilder.DropColumn(
                name: "AgentCommissionAmount",
                table: "PolicyRequests");

            migrationBuilder.DropColumn(
                name: "PremiumAmount",
                table: "PolicyRequests");
        }
    }
}
