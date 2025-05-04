using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 1,
                column: "TradeDescription",
                value: "Trade 1: John Doe offers Game1 to Michael John");

            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 2,
                column: "TradeDescription",
                value: "Trade 2: Jane Doe offers Game2 to Maria Elena");

            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 3,
                column: "TradeDescription",
                value: "Trade 1: John Doe offers Game1 to Michael John");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "johndoe@gmail.com", "John Doe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "michaeljohn@gmail.com", "Michael John" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "janedoe@gmail.com", "Jane Doe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "mariaelena@gmail.com", "Maria Elena" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 1,
                column: "TradeDescription",
                value: "Trade 1: User1 offers Game1 to User2");

            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 2,
                column: "TradeDescription",
                value: "Trade 2: User3 offers Game2 to User4");

            migrationBuilder.UpdateData(
                table: "ItemTrades",
                keyColumn: "TradeId",
                keyValue: 3,
                column: "TradeDescription",
                value: "Trade 1: User1 offers Game1 to User2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "user1@gmail.com", "User1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "user2@gmail.com", "User2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "user3@gmail.com", "User3" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "Email", "UserName" },
                values: new object[] { "user4@gmail.com", "User4" });
        }
    }
}
