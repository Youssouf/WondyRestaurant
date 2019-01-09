using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddChangesToPropertyCategoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CateboryId",
                table: "SubCategory");

            migrationBuilder.DropColumn(
                name: "CateboryId",
                table: "SubCategory");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory");

            migrationBuilder.AddColumn<int>(
                name: "CateboryId",
                table: "SubCategory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CateboryId",
                table: "SubCategory",
                column: "CateboryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory",
                column: "CateboryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
