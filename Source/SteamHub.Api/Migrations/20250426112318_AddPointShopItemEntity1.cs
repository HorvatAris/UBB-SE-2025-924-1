using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPointShopItemEntity1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointShopItems",
                columns: table => new
                {
                    ItemIdentifier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointPrice = table.Column<double>(type: "float", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointShopItems", x => x.ItemIdentifier);
                });

            migrationBuilder.CreateTable(
                name: "UserInventoryItems",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ItemIdentifier = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInventoryItems", x => new { x.UserId, x.ItemIdentifier });
                    table.ForeignKey(
                        name: "FK_UserInventoryItems_PointShopItems_ItemIdentifier",
                        column: x => x.ItemIdentifier,
                        principalTable: "PointShopItems",
                        principalColumn: "ItemIdentifier",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInventoryItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PointShopItems",
                columns: new[] { "ItemIdentifier", "Description", "ImagePath", "IsActive", "ItemType", "Name", "PointPrice" },
                values: new object[,]
                {
                    { 1, "Description1", "https://www.teeshood.com/cdn/shop/products/IMG_2627_1800x.png?v=1627292451", true, "Sticker", "Item1", 10.0 },
                    { 2, "Description2", "https://store.playstation.com/store/api/chihiro/00_09_000/container/US/en/19/UP1415-CUSA03724_00-AV00000000000160/image?w=320&h=320&bg_color=000000&opacity=100&_version=00_09_000", false, "Avatar", "Item2", 20.0 },
                    { 3, "Description3", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRPOga6RK9EhRyqpH7wO2HyZcjxyiryNk6vIw&s", true, "Emoji", "Item3", 30.0 },
                    { 4, "Description4", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRIaA9WSGUAWBQ2oVRwsPWoC7CrnhuYX5mMXA&s", true, "Sticker", "Item4", 40.0 }
                });

            migrationBuilder.InsertData(
                table: "UserInventoryItems",
                columns: new[] { "ItemIdentifier", "UserId", "PurchaseDate", "isActive" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 4, 26, 14, 23, 16, 756, DateTimeKind.Local).AddTicks(1002), true },
                    { 4, 1, new DateTime(2025, 4, 26, 14, 23, 16, 758, DateTimeKind.Local).AddTicks(3762), true },
                    { 2, 2, new DateTime(2025, 4, 26, 14, 23, 16, 758, DateTimeKind.Local).AddTicks(3736), false },
                    { 3, 3, new DateTime(2025, 4, 26, 14, 23, 16, 758, DateTimeKind.Local).AddTicks(3759), true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInventoryItems_ItemIdentifier",
                table: "UserInventoryItems",
                column: "ItemIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInventoryItems");

            migrationBuilder.DropTable(
                name: "PointShopItems");
        }
    }
}
