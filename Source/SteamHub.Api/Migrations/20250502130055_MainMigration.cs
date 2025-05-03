using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MyMainMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PointShopItems",
                columns: table => new
                {
                    PointShopItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointPrice = table.Column<double>(type: "float", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointShopItems", x => x.PointShopItemId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletBalance = table.Column<float>(type: "real", nullable: false),
                    PointsBalance = table.Column<float>(type: "real", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    RejectMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfRecentPurchases = table.Column<int>(type: "int", nullable: false),
                    TrailerPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameplayPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PublisherUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_GameStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "GameStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Users_PublisherUserId",
                        column: x => x.PublisherUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "GameTag",
                columns: table => new
                {
                    GamesGameId = table.Column<int>(type: "int", nullable: false),
                    TagsTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTag", x => new { x.GamesGameId, x.TagsTagId });
                    table.ForeignKey(
                        name: "FK_GameTag_Games_GamesGameId",
                        column: x => x.GamesGameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTag_Tags_TagsTagId",
                        column: x => x.TagsTagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrespondingGameId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsListed = table.Column<bool>(type: "bit", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Games_CorrespondingGameId",
                        column: x => x.CorrespondingGameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemTrades",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceUserId = table.Column<int>(type: "int", nullable: false),
                    DestinationUserId = table.Column<int>(type: "int", nullable: false),
                    GameOfTradeId = table.Column<int>(type: "int", nullable: false),
                    TradeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TradeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradeStatus = table.Column<int>(type: "int", nullable: false),
                    AcceptedBySourceUser = table.Column<bool>(type: "bit", nullable: false),
                    AcceptedByDestinationUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTrades", x => x.TradeId);
                    table.ForeignKey(
                        name: "FK_ItemTrades_Games_GameOfTradeId",
                        column: x => x.GameOfTradeId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemTrades_Users_DestinationUserId",
                        column: x => x.DestinationUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ItemTrades_Users_SourceUserId",
                        column: x => x.SourceUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "StoreTransactions",
                columns: table => new
                {
                    StoreTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    WithMoney = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactions", x => x.StoreTransactionId);
                    table.ForeignKey(
                        name: "FK_StoreTransactions_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersGames",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    IsInWishlist = table.Column<bool>(type: "bit", nullable: false),
                    IsPurchased = table.Column<bool>(type: "bit", nullable: false),
                    IsInCart = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersGames", x => new { x.UserId, x.GameId });
                    table.ForeignKey(
                        name: "FK_UsersGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersGames_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemTradeDetails",
                columns: table => new
                {
                    TradeId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    IsSourceUserItem = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTradeDetails", x => new { x.TradeId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_ItemTradeDetails_ItemTrades_TradeId",
                        column: x => x.TradeId,
                        principalTable: "ItemTrades",
                        principalColumn: "TradeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTradeDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "GameStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Pending" },
                    { 1, "Approved" },
                    { 2, "Rejected" }
                });

            migrationBuilder.InsertData(
                table: "PointShopItems",
                columns: new[] { "PointShopItemId", "Description", "ImagePath", "ItemType", "Name", "PointPrice" },
                values: new object[,]
                {
                    { 1, "A cool blue background for your profile", "https://picsum.photos/id/1/200/200", "ProfileBackground", "Blue Profile Background", 1000.0 },
                    { 2, "A vibrant red background for your profile", "https://picsum.photos/id/20/200/200", "ProfileBackground", "Red Profile Background", 1000.0 },
                    { 3, "A golden frame for your avatar image", "https://picsum.photos/id/30/200/200", "AvatarFrame", "Golden Avatar Frame", 2000.0 },
                    { 4, "A silver frame for your avatar image", "https://picsum.photos/id/40/200/200", "AvatarFrame", "Silver Avatar Frame", 1500.0 },
                    { 5, "Express yourself with this happy emoticon", "https://picsum.photos/id/50/200/200", "Emoticon", "Happy Emoticon", 500.0 },
                    { 6, "Express yourself with this sad emoticon", "https://picsum.photos/id/60/200/200", "Emoticon", "Sad Emoticon", 500.0 },
                    { 7, "Cool gamer avatar for your profile", "https://picsum.photos/id/70/200/200", "Avatar", "Gamer Avatar", 1200.0 },
                    { 8, "Stealthy ninja avatar for your profile", "https://picsum.photos/id/80/200/200", "Avatar", "Ninja Avatar", 1200.0 },
                    { 9, "Space-themed mini profile", "https://picsum.photos/id/90/200/200", "MiniProfile", "Space Mini-Profile", 3000.0 },
                    { 10, "Fantasy-themed mini profile", "https://picsum.photos/id/100/200/200", "MiniProfile", "Fantasy Mini-Profile", 3000.0 }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "User" },
                    { 1, "Developer" }
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "PointsBalance", "RoleId", "UserName", "WalletBalance" },
                values: new object[,]
                {
                    { 1, "user1@gmail.com", 100f, 0, "User1", 56f },
                    { 2, "user2@gmail.com", 45f, 0, "User2", 78f },
                    { 3, "user3@gmail.com", 234f, 1, "User3", 21f },
                    { 4, "user4@gmail.com", 34f, 1, "User4", 455f }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "Description", "Discount", "GameplayPath", "ImagePath", "MinimumRequirements", "Name", "NumberOfRecentPurchases", "Price", "PublisherUserId", "Rating", "RecommendedRequirements", "RejectMessage", "StatusId", "TrailerPath" },
                values: new object[,]
                {
                    { 1, "An epic open-world RPG set in a mystical realm full of adventure, magic, and danger.", 0.15m, "https://www.youtube.com/watch?v=AKXiKBnzpBQ", "https://upload.wikimedia.org/wikipedia/en/5/5e/Elden_Ring_Box_art.jpg", "Intel i5-8400, 8GB RAM, GTX 1060", "Legends of Etheria", 1200, 49.99m, 3, 4.7m, "Intel i7-8700K, 16GB RAM, RTX 2070", "rejected game", 2, "https://www.youtube.com/watch?v=E3Huy2cdih0" },
                    { 2, "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.", 0.25m, "https://www.youtube.com/watch?v=8X2kIfS6fb8", "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg", "Intel i5-3570K, 8GB RAM, GTX 780", "Cyberstrike 2077", 950, 59.99m, 4, 4.2m, "Intel i7-4790, 12GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=FknHjl7eQ6o" },
                    { 3, "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.", 0.10m, "https://www.youtube.com/watch?v=gncB1_e9n8E", "https://upload.wikimedia.org/wikipedia/en/6/6d/Assassin%27s_Creed_Valhalla_cover.jpg", "Intel i5-4460, 8GB RAM, GTX 960", "Shadow of Valhalla", 780, 44.99m, 3, 4.5m, "Intel i7-6700K, 16GB RAM, GTX 1080", null, 1, "https://www.youtube.com/watch?v=ssrNcwxALS4" },
                    { 4, "An action-adventure game set in the fantasy land of Hyrule, where players control Link to rescue Princess Zelda.", 0.20m, "https://www.youtube.com/watch?v=0u8g1c2v4xE", "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg", "Intel Core i5, 8GB RAM, GTX 960", "The Legend of Zelda", 1500, 59.99m, 3, 4.8m, "Intel Core i7, 16GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=0u8g1c2v4xE" }
                });

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

            migrationBuilder.InsertData(
                table: "GameTag",
                columns: new[] { "GamesGameId", "TagsTagId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 4 },
                    { 2, 5 },
                    { 2, 6 },
                    { 3, 7 },
                    { 3, 8 },
                    { 3, 9 }
                });

            migrationBuilder.InsertData(
                table: "ItemTrades",
                columns: new[] { "TradeId", "AcceptedByDestinationUser", "AcceptedBySourceUser", "DestinationUserId", "GameOfTradeId", "SourceUserId", "TradeDate", "TradeDescription", "TradeStatus" },
                values: new object[,]
                {
                    { 1, false, false, 2, 1, 1, new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trade 1: User1 offers Game1 to User2", 0 },
                    { 2, false, true, 4, 2, 3, new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trade 2: User3 offers Game2 to User4", 0 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "CorrespondingGameId", "Description", "ImagePath", "IsListed", "ItemName", "Price" },
                values: new object[,]
                {
                    { 1, 1, "A mystical blade imbued with ancient magic from Legends of Etheria.", "https://cdn.example.com/etheria/ethereal-blade.jpg", true, "Ethereal Blade", 29.99f },
                    { 2, 1, "An enchanted armour that protects the bearer in Legends of Etheria.", "https://cdn.example.com/etheria/mystic-armour.jpg", true, "Mystic Armour", 39.99f },
                    { 3, 2, "A high-tech gauntlet to hack and crush foes in Cyberstrike 2077.", "https://cdn.example.com/cyberstrike/gauntlet.jpg", true, "Cybernetic Gauntlet", 34.99f },
                    { 4, 2, "A visor that enhances your vision in the neon-lit battles of Cyberstrike 2077.", "https://cdn.example.com/cyberstrike/neon-visor.jpg", true, "Neon Visor", 24.99f },
                    { 5, 3, "A mighty axe for the warriors of Shadow of Valhalla.", "https://cdn.example.com/valhalla/viking-axe.jpg", true, "Viking Axe", 44.99f },
                    { 6, 3, "A robust shield forged for the bravest of fighters in Shadow of Valhalla.", "https://cdn.example.com/valhalla/shield.jpg", true, "Valhalla Shield", 34.99f }
                });

            migrationBuilder.InsertData(
                table: "StoreTransactions",
                columns: new[] { "StoreTransactionId", "Amount", "Date", "GameId", "UserId", "WithMoney" },
                values: new object[,]
                {
                    { 1, 49.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 1, 1, true },
                    { 2, 59.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 2, 2, false }
                });

            migrationBuilder.InsertData(
                table: "UsersGames",
                columns: new[] { "GameId", "UserId", "IsInCart", "IsInWishlist", "IsPurchased" },
                values: new object[,]
                {
                    { 1, 1, false, true, false },
                    { 2, 1, false, false, true },
                    { 3, 1, true, false, false },
                    { 1, 2, false, false, true },
                    { 3, 2, true, false, false }
                });

            migrationBuilder.InsertData(
                table: "ItemTradeDetails",
                columns: new[] { "ItemId", "TradeId", "IsSourceUserItem" },
                values: new object[,]
                {
                    { 1, 1, true },
                    { 2, 2, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherUserId",
                table: "Games",
                column: "PublisherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_StatusId",
                table: "Games",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTag_TagsTagId",
                table: "GameTag",
                column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CorrespondingGameId",
                table: "Items",
                column: "CorrespondingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTradeDetails_ItemId",
                table: "ItemTradeDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTrades_DestinationUserId",
                table: "ItemTrades",
                column: "DestinationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTrades_GameOfTradeId",
                table: "ItemTrades",
                column: "GameOfTradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTrades_SourceUserId",
                table: "ItemTrades",
                column: "SourceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_GameId",
                table: "StoreTransactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_UserId",
                table: "StoreTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPointShopInventories_PointShopItemId",
                table: "UserPointShopInventories",
                column: "PointShopItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersGames_GameId",
                table: "UsersGames",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTag");

            migrationBuilder.DropTable(
                name: "ItemTradeDetails");

            migrationBuilder.DropTable(
                name: "StoreTransactions");

            migrationBuilder.DropTable(
                name: "UserPointShopInventories");

            migrationBuilder.DropTable(
                name: "UsersGames");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ItemTrades");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "PointShopItems");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameStatus");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
