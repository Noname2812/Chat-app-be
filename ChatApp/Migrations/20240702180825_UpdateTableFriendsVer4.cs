using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableFriendsVer4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Friends");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Friends",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friends");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Friends",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
