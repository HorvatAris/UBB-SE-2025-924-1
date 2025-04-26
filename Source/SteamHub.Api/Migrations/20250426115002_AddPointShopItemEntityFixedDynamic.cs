using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPointShopItemEntityFixedDynamic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 4, 1 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 3, 3 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 1, 1 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 26, 14, 45, 20, 141, DateTimeKind.Local).AddTicks(7878));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 4, 1 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 26, 14, 45, 20, 141, DateTimeKind.Local).AddTicks(7878));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 2, 2 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 26, 14, 45, 20, 141, DateTimeKind.Local).AddTicks(7878));

            migrationBuilder.UpdateData(
                table: "UserInventoryItems",
                keyColumns: new[] { "ItemIdentifier", "UserId" },
                keyValues: new object[] { 3, 3 },
                column: "PurchaseDate",
                value: new DateTime(2025, 4, 26, 14, 45, 20, 141, DateTimeKind.Local).AddTicks(7878));
        }
    }
}
