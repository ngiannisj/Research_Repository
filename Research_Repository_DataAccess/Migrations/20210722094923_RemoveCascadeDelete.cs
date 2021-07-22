using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository_DataAccess.Migrations
{
    public partial class RemoveCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Teams_TeamId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Themes_ThemeId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_TeamId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Items",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Themes_ThemeId",
                table: "Items",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Projects_ProjectId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Themes_ThemeId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "ThemeId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                name: "FK_Items_Projects_ProjectId",
                table: "Items",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Teams_TeamId",
                table: "Items",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Themes_ThemeId",
                table: "Items",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
