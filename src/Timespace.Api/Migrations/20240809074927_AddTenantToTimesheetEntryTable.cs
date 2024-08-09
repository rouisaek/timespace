using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timespace.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantToTimesheetEntryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TimesheetEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetEntries_TenantId",
                table: "TimesheetEntries",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_Tenants_TenantId",
                table: "TimesheetEntries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_Tenants_TenantId",
                table: "TimesheetEntries");

            migrationBuilder.DropIndex(
                name: "IX_TimesheetEntries_TenantId",
                table: "TimesheetEntries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TimesheetEntries");
        }
    }
}
