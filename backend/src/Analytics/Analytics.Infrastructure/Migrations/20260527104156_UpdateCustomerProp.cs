using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LifetimeValue",
                schema: "Analytics",
                table: "Customers",
                newName: "TotalRevenue");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Tenant_LifetimeValue",
                schema: "Analytics",
                table: "Customers",
                newName: "IX_Customers_Tenant_TotalRevenue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalRevenue",
                schema: "Analytics",
                table: "Customers",
                newName: "LifetimeValue");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Tenant_TotalRevenue",
                schema: "Analytics",
                table: "Customers",
                newName: "IX_Customers_Tenant_LifetimeValue");
        }
    }
}
