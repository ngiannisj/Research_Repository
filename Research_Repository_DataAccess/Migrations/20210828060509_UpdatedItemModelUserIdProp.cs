using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository_DataAccess.Migrations
{
    public partial class UpdatedItemModelUserIdProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uploader",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "UploaderId",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UploaderId",
                table: "Items",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UploaderId",
                table: "Items",
                column: "UploaderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_UploaderId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UploaderId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UploaderId",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Uploader",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
