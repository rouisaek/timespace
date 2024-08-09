using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Timespace.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdToTimesheetEntryIdInTimesheetEntriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TimesheetEntries",
                newName: "TimesheetEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimesheetEntryId",
                table: "TimesheetEntries",
                newName: "Id");
        }
    }
}
