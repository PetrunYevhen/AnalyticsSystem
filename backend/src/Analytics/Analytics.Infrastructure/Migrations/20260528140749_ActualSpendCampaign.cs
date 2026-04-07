using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualSpendCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualSpendAmount",
                schema: "Analytics",
                table: "Campaigns",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ActualSpendCurrency",
                schema: "Analytics",
                table: "Campaigns",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualSpendAmount",
                schema: "Analytics",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "ActualSpendCurrency",
                schema: "Analytics",
                table: "Campaigns");
        }
    }
}
