using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsAndRemoveTestGamesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestGames");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "TagName" },
                values: new object[,]
                {
                    { 1, "Tag1" },
                    { 2, "Rogue-Like" },
                    { 3, "Third-Person Shooter" },
                    { 4, "Multiplayer" },
                    { 5, "Horror" },
                    { 6, "First-Person Shooter" },
                    { 7, "Action" },
                    { 8, "Platformer" },
                    { 9, "Adventure" },
                    { 10, "Puzzle" },
                    { 11, "Exploration" },
                    { 12, "Sandbox" },
                    { 13, "Survival" },
                    { 14, "Arcade" },
                    { 15, "RPG" },
                    { 16, "Racing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.CreateTable(
                name: "TestGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGames", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TestGames",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Roblox" },
                    { 2, "Minecraft" },
                    { 3, "Metin2" }
                });
        }
    }
}
