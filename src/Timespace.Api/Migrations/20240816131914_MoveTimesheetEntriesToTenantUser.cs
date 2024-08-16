using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timespace.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveTimesheetEntriesToTenantUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_AspNetUsers_UserId",
                table: "TimesheetEntries");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TimesheetEntries",
                newName: "TenantUserId");

            migrationBuilder.RenameIndex(
                name: "IX_TimesheetEntries_UserId",
                table: "TimesheetEntries",
                newName: "IX_TimesheetEntries_TenantUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_TenantUsers_TenantUserId",
                table: "TimesheetEntries",
                column: "TenantUserId",
                principalTable: "TenantUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimesheetEntries_TenantUsers_TenantUserId",
                table: "TimesheetEntries");

            migrationBuilder.RenameColumn(
                name: "TenantUserId",
                table: "TimesheetEntries",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TimesheetEntries_TenantUserId",
                table: "TimesheetEntries",
                newName: "IX_TimesheetEntries_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TimesheetEntries_AspNetUsers_UserId",
                table: "TimesheetEntries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
