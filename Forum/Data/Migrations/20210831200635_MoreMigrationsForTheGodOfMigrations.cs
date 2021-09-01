using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class MoreMigrationsForTheGodOfMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ForumUserViewModel_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_ForumUserViewModel_AuthorId",
                table: "Threads");

            migrationBuilder.DropTable(
                name: "ForumUserViewModel");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId",
                table: "Threads",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_AuthorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Threads_AspNetUsers_AuthorId",
                table: "Threads");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ForumUserViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumUserViewModel", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ForumUserViewModel_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "ForumUserViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Threads_ForumUserViewModel_AuthorId",
                table: "Threads",
                column: "AuthorId",
                principalTable: "ForumUserViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
