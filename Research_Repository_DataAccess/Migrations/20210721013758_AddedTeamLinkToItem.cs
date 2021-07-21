using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository_DataAccess.Migrations
{
    public partial class AddedTeamLinkToItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Items_TeamId",
                table: "Items",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Teams_TeamId",
                table: "Items",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Teams_TeamId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_TeamId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Items");
        }
    }
}
