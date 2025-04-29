using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StoreTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "StoreTransactions",
                columns: new[] { "StoreTransactionId", "Amount", "Date", "GameId", "UserId", "WithMoney" },
                values: new object[,]
                {
                    { 1, 49.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 1, 1, true },
                    { 2, 59.99f, new DateTime(2025, 4, 27, 14, 30, 0, 0, DateTimeKind.Unspecified), 2, 2, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_GameId",
                table: "StoreTransactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_UserId",
                table: "StoreTransactions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreTransactions");
        }
    }
}
