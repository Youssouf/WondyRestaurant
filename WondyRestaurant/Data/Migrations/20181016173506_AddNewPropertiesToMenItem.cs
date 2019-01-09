using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddNewPropertiesToMenItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_CategoryId",
                table: "MenuItem");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "MenuItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_SubcategoryId",
                table: "MenuItem",
                column: "SubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_SubcategoryId",
                table: "MenuItem",
                column: "SubcategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_SubCategory_SubcategoryId",
                table: "MenuItem");

            migrationBuilder.DropIndex(
                name: "IX_MenuItem_SubcategoryId",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "MenuItem");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_SubCategory_CategoryId",
                table: "MenuItem",
                column: "CategoryId",
                principalTable: "SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
