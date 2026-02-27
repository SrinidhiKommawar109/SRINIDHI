using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertySubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertySubCategories_PropertyCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PropertyCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseCoverageAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoverageRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BasePremium = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgentCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyPlans_PropertySubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "PropertySubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PropertyAge = table.Column<int>(type: "int", nullable: true),
                    RiskScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PremiumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AgentCommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyRequests_PropertyPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "PropertyPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyRequests_Users_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PolicyRequests_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyRequestId = table.Column<int>(type: "int", nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PropertyAge = table.Column<int>(type: "int", nullable: false),
                    ClaimAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_PolicyRequests_PolicyRequestId",
                        column: x => x.PolicyRequestId,
                        principalTable: "PolicyRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PropertyCategories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Property Insurance" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "IsActive", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "admin@gmail.com", "Admin", true, "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", 1 },
                    { 2, "claims@gmail.com", "Claims Officer", true, "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", 4 },
                    { 3, "customer@gmail.com", "Customer", true, "$2a$11$kkF9EKe7KJxAijZ374He4edBGTSujLGRA48MkMwN9g6PK77IM2H..", 3 }
                });

            migrationBuilder.InsertData(
                table: "PropertySubCategories",
                columns: new[] { "Id", "CategoryId", "Code", "Name" },
                values: new object[,]
                {
                    { 1, 1, "SUB_RES_01", "Residential Property" },
                    { 2, 1, "SUB_COM_02", "Commercial Property" },
                    { 3, 1, "SUB_IND_03", "Industrial & Special Use" },
                    { 4, 1, "SUB_CON_04", "Property Contents" }
                });

            migrationBuilder.InsertData(
                table: "PropertyPlans",
                columns: new[] { "Id", "AgentCommission", "BaseCoverageAmount", "BasePremium", "CoverageRate", "PlanName", "SubCategoryId" },
                values: new object[,]
                {
                    { 1, 500m, 1000000m, 5000m, 0.02m, "Basic Residential Plan", 1 },
                    { 2, 1200m, 5000000m, 15000m, 0.03m, "Basic Commercial Plan", 2 },
                    { 3, 2000m, 10000000m, 25000m, 0.04m, "Basic Industrial Plan", 3 },
                    { 4, 200m, 300000m, 2000m, 0.015m, "Basic Contents Plan", 4 }
                });

            migrationBuilder.InsertData(
                table: "PolicyRequests",
                columns: new[] { "Id", "AgentCommissionAmount", "AgentId", "CustomerId", "PlanId", "PremiumAmount", "PropertyAddress", "PropertyAge", "PropertyValue", "RiskScore", "Status" },
                values: new object[,]
                {
                    { 1, 0m, null, 3, 1, 0m, null, null, null, null, 6 },
                    { 2, 0m, null, 3, 2, 0m, null, null, null, null, 6 }
                });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimAmount", "PolicyRequestId", "PropertyAddress", "PropertyAge", "PropertyValue", "Remarks", "Status" },
                values: new object[,]
                {
                    { 1, 5000m, 1, "123 Main Street", 10, 1000000m, "", 0 },
                    { 2, 15000m, 2, "456 Commerce Road", 5, 5000000m, "", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyRequestId",
                table: "Claims",
                column: "PolicyRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRequests_AgentId",
                table: "PolicyRequests",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRequests_CustomerId",
                table: "PolicyRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyRequests_PlanId",
                table: "PolicyRequests",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPlans_SubCategoryId",
                table: "PropertyPlans",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertySubCategories_CategoryId",
                table: "PropertySubCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "PolicyRequests");

            migrationBuilder.DropTable(
                name: "PropertyPlans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PropertySubCategories");

            migrationBuilder.DropTable(
                name: "PropertyCategories");
        }
    }
}
