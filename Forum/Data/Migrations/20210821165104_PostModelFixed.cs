using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.Data.Migrations
{
    public partial class PostModelFixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Posts");

            migrationBuilder.AddColumn<int>(
                name: "QuoteId",
                table: "Posts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_QuoteId",
                table: "Posts",
                column: "QuoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_QuoteId",
                table: "Posts",
                column: "QuoteId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_QuoteId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_QuoteId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }
    }
}
