using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class GameTagUserEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tag_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletBalance = table.Column<float>(type: "real", nullable: false),
                    PointsBalance = table.Column<float>(type: "real", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Identifier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrailerPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameplayPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TagScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PublisherIdentifier = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_Games_User_PublisherIdentifier",
                        column: x => x.PublisherIdentifier,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTag",
                columns: table => new
                {
                    GameIdentifier = table.Column<int>(type: "int", nullable: false),
                    TagsTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTag", x => new { x.GameIdentifier, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_GameTag_Games_GameIdentifier",
                        column: x => x.GameIdentifier,
                        principalTable: "Games",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTag_Tags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "Tag_name" },
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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "Email", "PointsBalance", "UserName", "UserRole", "WalletBalance" },
                values: new object[,]
                {
                    { 1, "roblox@gmail.com", 22f, "Roblox", 1, 11f },
                    { 2, "john_doe@example.com", 500f, "john_doe", 0, 100f },
                    { 3, "jane_smith@example.com", 300f, "jane_smith", 1, 150f },
                    { 4, "alex_brown@example.com", 150f, "alex_brown", 0, 50f },
                    { 5, "behaviour@example.com", 1000f, "Behaviour Interactive", 1, 200f },
                    { 6, "valve@example.com", 300f, "Valve Corporation", 1, 150f },
                    { 7, "nintendo@example.com", 800f, "Nintendo", 1, 250f },
                    { 8, "hempuli@example.com", 500f, "Hempuli Oy", 1, 100f },
                    { 9, "mobius@example.com", 600f, "Mobius Digital", 1, 120f },
                    { 10, "mojang@example.com", 900f, "Mojang Studios", 1, 300f },
                    { 11, "unknownworlds@example.com", 700f, "Unknown Worlds Entertainment", 1, 180f },
                    { 12, "mary_jones@example.com", 1000f, "mary_jones", 1, 200f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherIdentifier",
                table: "Games",
                column: "PublisherIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_GameTag_TagsTagId",
                table: "GameTag",
                column: "TagsTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTag");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
