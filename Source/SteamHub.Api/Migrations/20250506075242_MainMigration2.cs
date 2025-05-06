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
            migrationBuilder.InsertData(
                table: "UsersGames",
                columns: new[] { "GameId", "UserId", "IsInCart", "IsInWishlist", "IsPurchased" },
                values: new object[] { 5, 5, false, false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersGames",
                keyColumns: new[] { "GameId", "UserId" },
                keyValues: new object[] { 5, 5 });
        }
    }
}
