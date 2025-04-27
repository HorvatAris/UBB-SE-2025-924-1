using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserPointShopItemInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "GameStatus",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GameStatus", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Role",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Role", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Tags",
            //    columns: table => new
            //    {
            //        TagId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        TagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tags", x => x.TagId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        UserId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        WalletBalance = table.Column<float>(type: "real", nullable: false),
            //        PointsBalance = table.Column<float>(type: "real", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.UserId);
            //        table.ForeignKey(
            //            name: "FK_Users_Role_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Role",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Games",
            //    columns: table => new
            //    {
            //        GameId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        MinimumRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RecommendedRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        StatusId = table.Column<int>(type: "int", nullable: false),
            //        RejectMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        NumberOfRecentPurchases = table.Column<int>(type: "int", nullable: false),
            //        TrailerPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        GameplayPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        PublisherUserId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Games", x => x.GameId);
            //        table.ForeignKey(
            //            name: "FK_Games_GameStatus_StatusId",
            //            column: x => x.StatusId,
            //            principalTable: "GameStatus",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Games_Users_PublisherUserId",
            //            column: x => x.PublisherUserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "UserPointShopInventories",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PointShopItemId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPointShopInventories", x => new { x.UserId, x.PointShopItemId });
                    table.ForeignKey(
                        name: "FK_UserPointShopInventories_PointShopItems_PointShopItemId",
                        column: x => x.PointShopItemId,
                        principalTable: "PointShopItems",
                        principalColumn: "PointShopItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPointShopInventories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "GameTag",
            //    columns: table => new
            //    {
            //        GamesGameId = table.Column<int>(type: "int", nullable: false),
            //        TagsTagId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GameTag", x => new { x.GamesGameId, x.TagsTagId });
            //        table.ForeignKey(
            //            name: "FK_GameTag_Games_GamesGameId",
            //            column: x => x.GamesGameId,
            //            principalTable: "Games",
            //            principalColumn: "GameId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_GameTag_Tags_TagsTagId",
            //            column: x => x.TagsTagId,
            //            principalTable: "Tags",
            //            principalColumn: "TagId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.InsertData(
            //    table: "GameStatus",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[,]
            //    {
            //        { 0, "Pending" },
            //        { 1, "Approved" },
            //        { 2, "Rejected" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Role",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[,]
            //    {
            //        { 0, "User" },
            //        { 1, "Developer" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Tags",
            //    columns: new[] { "TagId", "TagName" },
            //    values: new object[,]
            //    {
            //        { 1, "Tag1" },
            //        { 2, "Rogue-Like" },
            //        { 3, "Third-Person Shooter" },
            //        { 4, "Multiplayer" },
            //        { 5, "Horror" },
            //        { 6, "First-Person Shooter" },
            //        { 7, "Action" },
            //        { 8, "Platformer" },
            //        { 9, "Adventure" },
            //        { 10, "Puzzle" },
            //        { 11, "Exploration" },
            //        { 12, "Sandbox" },
            //        { 13, "Survival" },
            //        { 14, "Arcade" },
            //        { 15, "RPG" },
            //        { 16, "Racing" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Users",
            //    columns: new[] { "UserId", "Email", "PointsBalance", "RoleId", "UserName", "WalletBalance" },
            //    values: new object[,]
            //    {
            //        { 1, "user1@gmail.com", 100f, 0, "User1", 56f },
            //        { 2, "user2@gmail.com", 45f, 0, "User2", 78f },
            //        { 3, "user3@gmail.com", 234f, 1, "User3", 21f },
            //        { 4, "user4@gmail.com", 34f, 1, "User4", 455f }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Games",
            //    columns: new[] { "GameId", "Description", "Discount", "GameplayPath", "ImagePath", "MinimumRequirements", "Name", "NumberOfRecentPurchases", "Price", "PublisherUserId", "Rating", "RecommendedRequirements", "RejectMessage", "StatusId", "TrailerPath" },
            //    values: new object[,]
            //    {
            //        { 1, "An epic open-world RPG set in a mystical realm full of adventure, magic, and danger.", 0.15m, "https://www.youtube.com/watch?v=AKXiKBnzpBQ", "https://upload.wikimedia.org/wikipedia/en/5/5e/Elden_Ring_Box_art.jpg", "Intel i5-8400, 8GB RAM, GTX 1060", "Legends of Etheria", 1200, 49.99m, 3, 4.7m, "Intel i7-8700K, 16GB RAM, RTX 2070", "rejected game", 2, "https://www.youtube.com/watch?v=E3Huy2cdih0" },
            //        { 2, "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.", 0.25m, "https://www.youtube.com/watch?v=8X2kIfS6fb8", "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg", "Intel i5-3570K, 8GB RAM, GTX 780", "Cyberstrike 2077", 950, 59.99m, 4, 4.2m, "Intel i7-4790, 12GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=FknHjl7eQ6o" },
            //        { 3, "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.", 0.10m, "https://www.youtube.com/watch?v=gncB1_e9n8E", "https://upload.wikimedia.org/wikipedia/en/6/6d/Assassin%27s_Creed_Valhalla_cover.jpg", "Intel i5-4460, 8GB RAM, GTX 960", "Shadow of Valhalla", 780, 44.99m, 3, 4.5m, "Intel i7-6700K, 16GB RAM, GTX 1080", null, 1, "https://www.youtube.com/watch?v=ssrNcwxALS4" }
            //    });

            migrationBuilder.InsertData(
                table: "UserPointShopInventories",
                columns: new[] { "PointShopItemId", "UserId", "IsActive", "PurchaseDate" },
                values: new object[,]
                {
                    { 1, 1, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 1, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 2, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 3, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            //migrationBuilder.InsertData(
            //    table: "GameTag",
            //    columns: new[] { "GamesGameId", "TagsTagId" },
            //    values: new object[,]
            //    {
            //        { 1, 1 },
            //        { 1, 2 },
            //        { 1, 3 },
            //        { 2, 4 },
            //        { 2, 5 },
            //        { 2, 6 },
            //        { 3, 7 },
            //        { 3, 8 },
            //        { 3, 9 }
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Games_PublisherUserId",
            //    table: "Games",
            //    column: "PublisherUserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Games_StatusId",
            //    table: "Games",
            //    column: "StatusId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_GameTag_TagsTagId",
            //    table: "GameTag",
            //    column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPointShopInventories_PointShopItemId",
                table: "UserPointShopInventories",
                column: "PointShopItemId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_RoleId",
            //    table: "Users",
            //    column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "GameTag");

            migrationBuilder.DropTable(
                name: "UserPointShopInventories");

            //migrationBuilder.DropTable(
            //    name: "Games");

            //migrationBuilder.DropTable(
            //    name: "Tags");

            //migrationBuilder.DropTable(
            //    name: "GameStatus");

            //migrationBuilder.DropTable(
            //    name: "Users");

            //migrationBuilder.DropTable(
            //    name: "Role");
        }
    }
}
