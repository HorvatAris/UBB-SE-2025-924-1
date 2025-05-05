using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration : Migration
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
                name: "UserInventories",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AcquiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInventories", x => new { x.UserId, x.ItemId, x.GameId });
                    table.ForeignKey(
                        name: "FK_UserInventories_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInventories_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserInventories_Users_UserId",
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
                        onDelete: ReferentialAction.Cascade);
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
                    { 1, "Rogue-Like" },
                    { 2, "Third-Person Shooter" },
                    { 3, "Multiplayer" },
                    { 4, "Horror" },
                    { 5, "First-Person Shooter" },
                    { 6, "Action" },
                    { 7, "Platformer" },
                    { 8, "Adventure" },
                    { 9, "Puzzle" },
                    { 10, "Exploration" },
                    { 11, "Sandbox" },
                    { 12, "Survival" },
                    { 13, "Arcade" },
                    { 14, "RPG" },
                    { 15, "Racing" },
                    { 16, "Action RPG" },
                    { 17, "Battle Royale" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "PointsBalance", "RoleId", "UserName", "WalletBalance" },
                values: new object[,]
                {
                    { 1, "gabe.newell@valvestudio.com", 100f, 1, "GabeN", 500f },
                    { 2, "mathias.new@cdprojektred.com", 85f, 1, "MattN", 420f },
                    { 3, "john.chen@thatgamecompany.com", 92f, 1, "JohnC", 390f },
                    { 4, "alice.johnson@example.com", 45f, 0, "AliceJ", 78f },
                    { 5, "liam.garcia@example.com", 67f, 0, "LiamG", 55f },
                    { 6, "sophie.williams@example.com", 120f, 0, "SophieW", 95f },
                    { 7, "noah.smith@example.com", 38f, 0, "NoahS", 33f },
                    { 8, "emily.brown@example.com", 84f, 0, "EmilyB", 110f }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "Description", "Discount", "GameplayPath", "ImagePath", "MinimumRequirements", "Name", "NumberOfRecentPurchases", "Price", "PublisherUserId", "Rating", "RecommendedRequirements", "RejectMessage", "StatusId", "TrailerPath" },
                values: new object[,]
                {
                    { 1, "A rogue-like third-person shooter where players fight through hordes of monsters to escape an alien planet.", 0.20m, "https://www.youtube.com/watch?v=Cwk3qmD28CE", "https://upload.wikimedia.org/wikipedia/en/c/c1/Risk_of_Rain_2.jpg", "4GB RAM, 2.5GHz Processor, GTX 580", "Risk of Rain 2", 0, 24.99m, 1, 0m, "8GB RAM, 3.0GHz Processor, GTX 680", "Minimum requirements are too high", 2, "https://www.youtube.com/watch?v=pJ-aR--gScM" },
                    { 2, "A multiplayer horror game where survivors must evade a killer.", 0.40m, "https://www.youtube.com/watch?v=3wUHKO0ieyY", "https://pbs.twimg.com/media/FOEzJiXX0AcxBTi.jpg", "8GB RAM, i3-4170, GTX 760", "Dead by Daylight", 0, 19.99m, 1, 0m, "16GB RAM, i5-6500, GTX 1060", null, 0, "https://www.youtube.com/watch?v=JGhIXLO3ul8" },
                    { 3, "A tactical first-person shooter featuring team-based gameplay.", 0.50m, "https://www.youtube.com/watch?v=P22HqM9w500", "https://sm.ign.com/ign_nordic/cover/c/counter-st/counter-strike-2_jc2d.jpg", "8GB RAM, i5-2500K, GTX 660", "Counter-Strike 2", 0, 20.99m, 1, 0m, "16GB RAM, i7-7700K, GTX 1060", null, 1, "https://www.youtube.com/watch?v=c80dVYcL69E" },
                    { 4, "A story-driven first-person shooter that revolutionized the genre.", 0.60m, "https://www.youtube.com/watch?v=jElU1mD8JnI", "https://media.moddb.com/images/mods/1/47/46951/d1jhx20-dc797b78-5feb-4005-b206-.1.jpg", "512MB RAM, 1.7GHz Processor, DirectX 8 GPU", "Half-Life 2", 0, 9.99m, 1, 0m, "1GB RAM, 3.0GHz Processor, DirectX 9 GPU", null, 1, "https://www.youtube.com/watch?v=UKA7JkV51Jw" },
                    { 5, "A classic platformer adventure with iconic characters and worlds.", 0.70m, "https://www.youtube.com/watch?v=rLl9XBg7wSs", "https://play-lh.googleusercontent.com/3ZKfMRp_QrdN-LzsZTbXdXBH-LS1iykSg9ikNq_8T2ppc92ltNbFxS-tORxw2-6kGA", "N/A", "Mario", 0, 59.99m, 1, 0m, "N/A", null, 1, "https://www.youtube.com/watch?v=TnGl01FkMMo" },
                    { 6, "An epic adventure game where heroes save the kingdom of Hyrule.", 0.30m, "https://www.youtube.com/watch?v=wW7jkBJ_yK0", "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg", "N/A", "The Legend of Zelda", 0, 59.99m, 1, 0m, "N/A", null, 1, "https://www.youtube.com/watch?v=_X2h3SF7gd4" },
                    { 7, "A puzzle game where you change the rules to solve challenges.", 0.20m, "https://www.youtube.com/watch?v=dAiX8s-Eu7w", "https://is5-ssl.mzstatic.com/image/thumb/Purple113/v4/9e/30/61/9e3061a5-b2f0-87ad-9e90-563f37729be5/source/256x256bb.jpg", "2GB RAM, 1.0GHz Processor", "Baba Is You", 0, 14.99m, 2, 0m, "4GB RAM, 2.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=z3_yA4HTJfs" },
                    { 8, "A mind-bending puzzle-platformer with a dark sense of humor.", 0.10m, "https://www.youtube.com/watch?v=ts-j0nFf2e0", "https://cdn2.steamgriddb.com/icon_thumb/0994c8d1d6bc62cc56e9112d2303266b.png", "2GB RAM, 1.7GHz Processor, DirectX 9 GPU", "Portal 2", 0, 9.99m, 2, 0m, "4GB RAM, 3.0GHz Processor, GTX 760", null, 0, "https://www.youtube.com/watch?v=tax4e4hBBZc" },
                    { 9, "An exploration-based game where you unravel cosmic mysteries.", 0.15m, "https://www.youtube.com/watch?v=huL_TawYrMs", "https://images.nintendolife.com/62a79995ed766/outer-wilds-echoes-of-the-eye-cover.cover_large.jpg", "6GB RAM, i5-2300, GTX 560", "Outer Wilds", 0, 24.99m, 2, 0m, "8GB RAM, i7-6700, GTX 970", null, 0, "https://www.youtube.com/watch?v=d9u6KYVq5kw" },
                    { 10, "A rogue-like dungeon crawler where you defy the god of the dead.", 0.20m, "https://www.youtube.com/watch?v=4fVO0qUBe4E", "https://image.api.playstation.com/vulcan/ap/rnd/202104/0517/9AcM3vy5t77zPiJyKHwRfnNT.png", "4GB RAM, Dual Core 2.4GHz, Intel HD 5000", "Hades", 0, 24.99m, 2, 0m, "8GB RAM, Dual Core 3.0GHz, GTX 760", null, 0, "https://www.youtube.com/watch?v=91sW0DMkZzI" },
                    { 11, "A deck-building rogue-like where strategy is key to survival.", 0.25m, "https://www.youtube.com/watch?v=JO3EIPtw-4I", "https://image.api.playstation.com/cdn/EP3717/CUSA15338_00/Sn5xbNutqfQdWYIjbeCIN0bwTJOV7UPG.png", "2GB RAM, 2.0GHz Processor", "Slay the Spire", 0, 19.99m, 2, 0m, "4GB RAM, 3.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=75qT5KOs-Ew" },
                    { 12, "A platformer about climbing a mountain and facing inner demons.", 0.30m, "https://www.youtube.com/watch?v=FfRjHZWSYqY", "https://images.nintendolife.com/ef02c2e24c59e/celeste-cover.cover_large.jpg", "2GB RAM, 2.0GHz Processor", "Celeste", 0, 19.99m, 2, 0m, "4GB RAM, 2.4GHz Processor", null, 0, "https://www.youtube.com/watch?v=iofYDsP2vhQ" },
                    { 13, "An action-adventure game set in a beautifully animated underground world.", 0.35m, "https://www.youtube.com/watch?v=UAO2urG23S4", "https://image.api.playstation.com/cdn/EP1805/CUSA13285_00/DmwPWlU0468FbsjrtI92FhQz1xBYMoog.png", "4GB RAM, 2.0GHz Processor", "Hollow Knight", 0, 14.99m, 2, 0m, "8GB RAM, 3.2GHz Processor", null, 0, "https://www.youtube.com/watch?v=UAO2urG23S4" },
                    { 14, "A farming simulator RPG where you build a life in the countryside.", 0.20m, "https://www.youtube.com/watch?v=ot7uXNQskhs", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQHWjybGuWhdyJqjmtziGvtHvCnQf23yY0R6g&s", "2GB RAM, 2.0GHz Processor", "Stardew Valley", 0, 14.99m, 2, 0m, "4GB RAM, 3.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=ot7uXNQskhs" },
                    { 15, "A sandbox game that lets you build and explore infinite worlds.", 0.14m, "https://www.youtube.com/watch?v=ANgI2o_Jinc", "https://cdn2.steamgriddb.com/icon/f0b57183da91a7972b2b3c06b0db5542/32/512x512.png", "4GB RAM, Intel HD 4000", "Minecraft", 1420, 29.99m, 2, 4.8m, "8GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=MmB9b5njVbA" },
                    { 16, "A survival game in a dark and whimsical world filled with strange creatures.", 0.25m, "https://www.youtube.com/watch?v=htXgxyLpPMg", "https://image.api.playstation.com/cdn/EP2107/CUSA00327_00/i5qwqMWJj33IIr2m9TM29GQNnFCi4ZqI.png?w=440", "1.7GHz Processor, 1GB RAM", "Don't Starve", 0, 9.99m, 2, 0m, "2.0GHz Processor, 2GB RAM", null, 0, "https://www.youtube.com/watch?v=ochPlhMFk84" },
                    { 17, "A classic run and gun game with hand-drawn animations and tough bosses.", 0.40m, "https://www.youtube.com/watch?v=DNIMD8ZpMSQ", "https://upload.wikimedia.org/wikipedia/en/thumb/e/eb/Cuphead_%28artwork%29.png/250px-Cuphead_%28artwork%29.png", "3GB RAM, Intel Core2 Duo E8400", "Cuphead", 0, 19.99m, 2, 0m, "4GB RAM, i3-3240", null, 0, "https://www.youtube.com/watch?v=NN-9SQXoi50" },
                    { 18, "A black-and-white puzzle platformer with a haunting atmosphere.", 0.30m, "https://www.youtube.com/watch?v=dYeuLZY7fZk", "https://image.api.playstation.com/cdn/EP2054/CUSA01369_00/W45kellY9yrwSDpmQEL9tFqZQW7N4FEz.png?w=440", "512MB RAM, 1.5GHz Processor", "Limbo", 0, 9.99m, 2, 0m, "2GB RAM, 2.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=Y4HSyVXKYz8" },
                    { 19, "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.", 0.25m, "https://www.youtube.com/watch?v=8X2kIfS6fb8", "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg", "Intel i5-3570K, 8GB RAM, GTX 780", "Cyberstrike 2077", 950, 59.99m, 3, 4.2m, "Intel i7-4790, 12GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=FknHjl7eQ6o" },
                    { 20, "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.", 0.10m, "https://www.youtube.com/watch?v=gncB1_e9n8E", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDtZKyDW9Jrnh8ix-Y38qG5fddbUgYEW7yxA&s", "Intel i5-4460, 8GB RAM, GTX 960", "Shadow of Valhalla", 780, 44.99m, 3, 4.5m, "Intel i7-6700K, 16GB RAM, GTX 1080", null, 1, "https://www.youtube.com/watch?v=ssrNcwxALS4" }
                });

            migrationBuilder.InsertData(
                table: "UserPointShopInventories",
                columns: new[] { "PointShopItemId", "UserId", "IsActive", "PurchaseDate" },
                values: new object[,]
                {
                    { 1, 4, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 4, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 4, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 5, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 6, false, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 7, true, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "GameTag",
                columns: new[] { "GamesGameId", "TagsTagId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 3 },
                    { 3, 5 },
                    { 3, 6 },
                    { 4, 5 },
                    { 4, 8 },
                    { 5, 7 },
                    { 5, 8 },
                    { 6, 8 },
                    { 6, 14 },
                    { 7, 9 },
                    { 8, 8 },
                    { 8, 9 },
                    { 9, 8 },
                    { 9, 10 },
                    { 10, 1 },
                    { 10, 6 },
                    { 11, 1 },
                    { 11, 9 },
                    { 12, 7 },
                    { 13, 6 },
                    { 13, 7 },
                    { 14, 8 },
                    { 14, 12 },
                    { 15, 11 },
                    { 15, 12 },
                    { 16, 8 },
                    { 16, 12 },
                    { 17, 6 },
                    { 17, 7 },
                    { 18, 7 },
                    { 18, 9 },
                    { 19, 6 },
                    { 19, 14 },
                    { 19, 16 },
                    { 20, 16 }
                });

            migrationBuilder.InsertData(
                table: "ItemTrades",
                columns: new[] { "TradeId", "AcceptedByDestinationUser", "AcceptedBySourceUser", "DestinationUserId", "GameOfTradeId", "SourceUserId", "TradeDate", "TradeDescription", "TradeStatus" },
                values: new object[,]
                {
                    { 1, false, false, 8, 6, 4, new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trade 1: AliceJ offers Legend of Zelda to EmilyB", 0 },
                    { 2, false, true, 4, 19, 5, new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trade 2: LiamG offers Cyberstrike 2077 to AliceJ", 0 },
                    { 3, true, true, 6, 20, 7, new DateTime(2025, 4, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trade 3: NoahS offers Shadow of Valhalla to SophieW", 1 }
                });

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "ItemId", "CorrespondingGameId", "Description", "ImagePath", "IsListed", "ItemName", "Price" },
                values: new object[,]
                {
                    { 1, 3, "A sleek and aggressive finish for your AK-47.", "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_ak47_cu_ak47_cobra_light_large.7494bfdf4855fd4e6a2dbd983ed0a243c80ef830.png", false, "AK-47 | Redline Skin", 29.99f },
                    { 2, 3, "Legendary pistol skin with a fiery design.", "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_deagle_aa_flames_light_large.dd140c3b359c16ccd8e918ca6ad0b2628151fe1c.png", false, "Desert Eagle | Blaze Skin", 34.99f },
                    { 3, 4, "Iconic weapon that manipulates objects with physics.", "https://www.toyark.com/wp-content/uploads/2013/05/Half-Life-2-Gravity-Gun-007.jpg", false, "Gravity Gun Replica", 49.99f },
                    { 4, 4, "Protective gloves from the HEV suit worn by Gordon Freeman.", "https://preview.redd.it/hl2-revision-update-the-grabbity-gloves-v0-ftz143vjmqcb1.jpg?width=640&crop=smart&auto=webp&s=9b3738a0f4bce98cc6a38b34e6ec319d03c05dd0", false, "HEV Suit Gloves", 19.99f },
                    { 5, 5, "A soft collectible version of the iconic power-up.", "https://mario.wiki.gallery/images/thumb/7/7e/New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png/1200px-New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png", false, "Fire Flower", 14.99f },
                    { 6, 5, "The classic red cap worn by Mario himself.", "https://static.wikia.nocookie.net/mario/images/c/cd/Mario_Cap.png/revision/latest?cb=20180310022043", false, "Mario Cap", 24.99f },
                    { 7, 6, "Faithful replica of Link's legendary blade.", "https://upload.wikimedia.org/wikipedia/en/f/f9/Master_Sword_Lead.png", false, "Master Sword Replica", 69.99f },
                    { 8, 6, "Sturdy shield bearing the crest of Hyrule.", "https://theswordstall.co.uk/cdn/shop/files/Legend-Of-Zelda-Deluxe-Hylian-Shield-Full-Metal-3.jpg?v=1723552799&width=750", false, "Hylian Shield", 59.99f },
                    { 9, 15, "Miniature version of the famous mining tool.", "https://cdn.example.com/minecraft/diamond-pickaxe.jphttps://static.wikia.nocookie.net/minecraft_gamepedia/images/4/4c/Diamond_Pickaxe_JE1_BE1.png/revision/latest?cb=20190518122739g", false, "Diamond Pickaxe", 9.99f },
                    { 10, 15, "Soft plush of the infamous explosive mob.", "https://feltright.com/cdn/shop/files/minecraft-creeper.jpg?v=1720033057&width=800", false, "Creeper Plush", 19.99f },
                    { 11, 19, "A high-tech gauntlet to hack and crush foes in Cyberstrike 2077.", "https://static.wikia.nocookie.net/shop-heroes/images/4/4a/Gauntlets_Cybernetic_Gauntlets_Blueprint.png/revision/latest?cb=20200724020856", false, "Cybernetic Gauntlet", 34.99f },
                    { 12, 19, "A visor that enhances your vision in the neon-lit battles of Cyberstrike 2077.", "https://www.motocentral.co.uk/cdn/shop/files/Ruroc-EOX-Cyberstrike_-From-Moto-Central-_-Fast-Free-UK-Delivery-257043288_1024x.jpg?v=1744036882", false, "Neon Visor", 24.99f },
                    { 13, 20, "A mighty axe for the warriors of Shadow of Valhalla.", "https://valhalla-vikings.co.uk/cdn/shop/products/il_fullxfull.3370240260_td4v.jpg?v=1679150085&width=1080", false, "Viking Axe", 44.99f },
                    { 14, 20, "A robust shield forged for the bravest of fighters in Shadow of Valhalla.", "https://www.vikingsroar.com/cdn/shop/products/d7f00df1f2c5a9059ec5dd319139da24.webp?v=1652049514", false, "Valhalla Shield", 34.99f }
                });

            migrationBuilder.InsertData(
                table: "StoreTransactions",
                columns: new[] { "StoreTransactionId", "Amount", "Date", "GameId", "UserId", "WithMoney" },
                values: new object[,]
                {
                    { 1, 14.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 5, 4, true },
                    { 2, 34.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 20, 7, false },
                    { 3, 29.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 15, 4, true }
                });

            migrationBuilder.InsertData(
                table: "UsersGames",
                columns: new[] { "GameId", "UserId", "IsInCart", "IsInWishlist", "IsPurchased" },
                values: new object[,]
                {
                    { 3, 4, false, true, false },
                    { 5, 4, false, false, true },
                    { 6, 4, false, false, true },
                    { 15, 4, false, false, true },
                    { 6, 5, true, false, false },
                    { 19, 5, false, false, true },
                    { 20, 7, false, false, true },
                    { 15, 8, true, false, false }
                });

            migrationBuilder.InsertData(
                table: "ItemTradeDetails",
                columns: new[] { "ItemId", "TradeId", "IsSourceUserItem" },
                values: new object[,]
                {
                    { 7, 1, true },
                    { 12, 2, false }
                });

            migrationBuilder.InsertData(
                table: "UserInventories",
                columns: new[] { "GameId", "ItemId", "UserId", "AcquiredDate", "IsActive" },
                values: new object[,]
                {
                    { 5, 5, 4, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 5, 6, 4, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 15, 9, 4, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 15, 10, 4, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 6, 7, 5, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 6, 8, 5, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 19, 12, 5, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false },
                    { 20, 13, 7, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), false }
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
                name: "IX_UserInventories_GameId",
                table: "UserInventories",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInventories_ItemId",
                table: "UserInventories",
                column: "ItemId");

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
                name: "UserInventories");

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
