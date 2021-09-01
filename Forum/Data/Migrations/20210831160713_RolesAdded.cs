using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class RolesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "AspNetUsers",
                newName: "ThreadsStarted");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ThreadsStarted",
                table: "AspNetUsers",
                newName: "Points");
        }
    }
}
