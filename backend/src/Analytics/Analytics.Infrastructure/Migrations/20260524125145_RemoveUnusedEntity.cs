using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSources",
                schema: "Analytics");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Plan",
                schema: "Analytics",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Plan",
                schema: "Analytics",
                table: "Tenants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Plan",
                schema: "Analytics",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DataSources",
                schema: "Analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSyncError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
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
                name: "IX_Tenants_Plan",
                schema: "Analytics",
                table: "Tenants",
                column: "Plan");

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
        }
    }
}
