using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddPropertyToClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory");

            migrationBuilder.AlterColumn<int>(
                name: "CateboryId",
                table: "SubCategory",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory",
                column: "CateboryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory");

            migrationBuilder.AlterColumn<int>(
                name: "CateboryId",
                table: "SubCategory",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CateboryId",
                table: "SubCategory",
                column: "CateboryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
