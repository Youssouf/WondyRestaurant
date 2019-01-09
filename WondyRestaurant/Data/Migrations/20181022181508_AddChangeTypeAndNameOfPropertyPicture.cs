using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WondyRestaurant.Data.Migrations
{
    public partial class AddChangeTypeAndNameOfPropertyPicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pictures",
                table: "Coupons");

            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Coupons",
                nullable: false,
                defaultValue: new byte[] {  });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Coupons");

            migrationBuilder.AddColumn<byte>(
                name: "Pictures",
                table: "Coupons",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
