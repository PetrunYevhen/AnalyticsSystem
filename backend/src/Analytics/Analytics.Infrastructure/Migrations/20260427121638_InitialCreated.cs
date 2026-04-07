using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Analytics");

            migrationBuilder.CreateTable(
                name: "Campaigns",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Channel = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ActivePeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    ActivePeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    BudgetAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BudgetCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstOrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastOrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AcquisitionChannel = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    AcquisitionCampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingExpenses",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Impressions = table.Column<int>(type: "integer", nullable: false),
                    Clicks = table.Column<int>(type: "integer", nullable: false),
                    Leads = table.Column<int>(type: "integer", nullable: false),
                    MetricsRecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingExpenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalOrderId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ApiKeyHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ApiKeyPrefix = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Plan = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Analytics",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSources",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSyncError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TenantId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSources_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "Analytics",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DataSources_Tenants_TenantId1",
                        column: x => x.TenantId1,
                        principalSchema: "Analytics",
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Tenant_Channel",
                schema: "Analytics",
                table: "Campaigns",
                columns: new[] { "TenantId", "Channel" });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Tenant_Name",
                schema: "Analytics",
                table: "Campaigns",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Tenant_Status",
                schema: "Analytics",
                table: "Campaigns",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_Channel",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "AcquisitionChannel" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_External",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_LastOrderDate",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "LastOrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_RegistrationDate",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "RegistrationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_Status",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_Tenant_Name",
                schema: "Analytics",
                table: "DataSources",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_Tenant_Status",
                schema: "Analytics",
                table: "DataSources",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_Tenant_Type",
                schema: "Analytics",
                table: "DataSources",
                columns: new[] { "TenantId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_TenantId1",
                schema: "Analytics",
                table: "DataSources",
                column: "TenantId1");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingExpenses_Tenant_Campaign",
                schema: "Analytics",
                table: "MarketingExpenses",
                columns: new[] { "TenantId", "CampaignId" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingExpenses_Tenant_Date",
                schema: "Analytics",
                table: "MarketingExpenses",
                columns: new[] { "TenantId", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingExpenses_Tenant_Source_Date",
                schema: "Analytics",
                table: "MarketingExpenses",
                columns: new[] { "TenantId", "AdSource", "ExpenseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                schema: "Analytics",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductExternalId",
                schema: "Analytics",
                table: "OrderItem",
                column: "ProductExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Tenant_Customer_Date",
                schema: "Analytics",
                table: "Orders",
                columns: new[] { "TenantId", "CustomerId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Tenant_ExternalId",
                schema: "Analytics",
                table: "Orders",
                columns: new[] { "TenantId", "ExternalOrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Tenant_OrderDate",
                schema: "Analytics",
                table: "Orders",
                columns: new[] { "TenantId", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Tenant_Status_Date",
                schema: "Analytics",
                table: "Orders",
                columns: new[] { "TenantId", "Status", "OrderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ApiKeyHash",
                schema: "Analytics",
                table: "Tenants",
                column: "ApiKeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ApiKeyPrefix",
                schema: "Analytics",
                table: "Tenants",
                column: "ApiKeyPrefix",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CompanyName",
                schema: "Analytics",
                table: "Tenants",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Plan",
                schema: "Analytics",
                table: "Tenants",
                column: "Plan");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Analytics",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                schema: "Analytics",
                table: "Users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Campaigns",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "DataSources",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "MarketingExpenses",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "Tenants",
                schema: "Analytics");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Analytics");
        }
    }
}
