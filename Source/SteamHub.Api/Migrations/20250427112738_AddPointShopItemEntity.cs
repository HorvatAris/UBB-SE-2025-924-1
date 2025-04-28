using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPointShopItemEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "TestGames");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointShopItems");

            //migrationBuilder.CreateTable(
            //    name: "TestGames",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TestGames", x => x.Id);
            //    });

            //migrationBuilder.InsertData(
            //    table: "TestGames",
            //    columns: new[] { "Id", "Name" },
            //    values: new object[,]
            //    {
            //        { 1, "Roblox" },
            //        { 2, "Minecraft" },
            //        { 3, "Metin2" }
            //    });
        }
    }
}
