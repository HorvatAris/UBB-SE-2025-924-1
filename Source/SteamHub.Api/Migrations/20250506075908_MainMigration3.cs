using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 9,
                column: "ImagePath",
                value: "https://static.posters.cz/image/1300/merch/replica-minecraft-diamond-pickaxe-i94007.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Items",
                keyColumn: "ItemId",
                keyValue: 9,
                column: "ImagePath",
                value: "https://cdn.example.com/minecraft/diamond-pickaxe.jphttps://static.wikia.nocookie.net/minecraft_gamepedia/images/4/4c/Diamond_Pickaxe_JE1_BE1.png/revision/latest?cb=20190518122739g");
        }
    }
}
