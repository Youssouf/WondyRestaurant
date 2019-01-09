using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddMenuItemToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Spicyness = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    CategoriId = table.Column<int>(nullable: false),
                    SubcategoryId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItem_SubCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_CategoryId",
                table: "MenuItem",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItem");
        }
    }
}
