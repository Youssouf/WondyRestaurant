using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddUpdateClassName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_SubcategoryId",
                table: "MenuItem");

            migrationBuilder.RenameColumn(
                name: "SubcategoryId",
                table: "MenuItem",
                newName: "SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItem_SubcategoryId",
                table: "MenuItem",
                newName: "IX_MenuItem_SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem",
                column: "SubCategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_SubCategoryId",
                table: "MenuItem");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "MenuItem",
                newName: "SubcategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItem_SubCategoryId",
                table: "MenuItem",
                newName: "IX_MenuItem_SubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_SubcategoryId",
                table: "MenuItem",
                column: "SubcategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
