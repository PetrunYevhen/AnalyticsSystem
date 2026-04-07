using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLtvToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LifetimeValue",
                schema: "Analytics",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                schema: "Analytics",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Tenant_LifetimeValue",
                schema: "Analytics",
                table: "Customers",
                columns: new[] { "TenantId", "LifetimeValue" });

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "Analytics",
                table: "Orders",
                column: "CustomerId",
                principalSchema: "Analytics",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "Analytics",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                schema: "Analytics",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Tenant_LifetimeValue",
                schema: "Analytics",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LifetimeValue",
                schema: "Analytics",
                table: "Customers");
        }
    }
}
