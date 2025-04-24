using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialSkeleton : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGames", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TestGames",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Roblox" },
                    { 2, "Minecraft" },
                    { 3, "Metin2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestGames");
        }
    }
}
