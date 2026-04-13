using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPropertiesToMarketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Clicks",
                schema: "Analytics",
                table: "MarketingExpenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Impressions",
                schema: "Analytics",
                table: "MarketingExpenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Leads",
                schema: "Analytics",
                table: "MarketingExpenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clicks",
                schema: "Analytics",
                table: "MarketingExpenses");

            migrationBuilder.DropColumn(
                name: "Impressions",
                schema: "Analytics",
                table: "MarketingExpenses");

            migrationBuilder.DropColumn(
                name: "Leads",
                schema: "Analytics",
                table: "MarketingExpenses");
        }
    }
}
