using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Timespace.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoveEmployeePropertiesToTenantUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantUser_AspNetUsers_UserId",
                table: "TenantUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantUser_Tenants_TenantId",
                table: "TenantUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantUser",
                table: "TenantUser");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "TenantUser",
                newName: "TenantUsers");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUser_UserId",
                table: "TenantUsers",
                newName: "IX_TenantUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUser_TenantId",
                table: "TenantUsers",
                newName: "IX_TenantUsers_TenantId");

            migrationBuilder.AddColumn<Instant>(
                name: "CreatedAt",
                table: "TenantUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "TenantUsers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "LastLogin",
                table: "TenantUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Permissions",
                table: "TenantUsers",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<Instant>(
                name: "UpdatedAt",
                table: "TenantUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantUsers",
                table: "TenantUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUsers_AspNetUsers_UserId",
                table: "TenantUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUsers_Tenants_TenantId",
                table: "TenantUsers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantUsers_AspNetUsers_UserId",
                table: "TenantUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantUsers_Tenants_TenantId",
                table: "TenantUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TenantUsers",
                table: "TenantUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TenantUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "TenantUsers");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "TenantUsers");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "TenantUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TenantUsers");

            migrationBuilder.RenameTable(
                name: "TenantUsers",
                newName: "TenantUser");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUsers_UserId",
                table: "TenantUser",
                newName: "IX_TenantUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantUsers_TenantId",
                table: "TenantUser",
                newName: "IX_TenantUser_TenantId");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "LastLogin",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Permissions",
                table: "AspNetUsers",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TenantUser",
                table: "TenantUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUser_AspNetUsers_UserId",
                table: "TenantUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantUser_Tenants_TenantId",
                table: "TenantUser",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
