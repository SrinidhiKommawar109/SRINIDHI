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
            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 500m, 5000m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 1200m, 15000m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 2000m, 25000m });

            migrationBuilder.UpdateData(
                table: "PropertyPlans",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AgentCommission", "BasePremium" },
                values: new object[] { 200m, 2000m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
