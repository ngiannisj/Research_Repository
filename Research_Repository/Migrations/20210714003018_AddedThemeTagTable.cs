using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository.Migrations
{
    public partial class AddedThemeTagTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThemeTag_Tags_TagId",
                table: "ThemeTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ThemeTag_Themes_ThemeId",
                table: "ThemeTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThemeTag",
                table: "ThemeTag");

            migrationBuilder.RenameTable(
                name: "ThemeTag",
                newName: "ThemeTags");

            migrationBuilder.RenameIndex(
                name: "IX_ThemeTag_TagId",
                table: "ThemeTags",
                newName: "IX_ThemeTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThemeTags",
                table: "ThemeTags",
                columns: new[] { "ThemeId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ThemeTags_Tags_TagId",
                table: "ThemeTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThemeTags_Themes_ThemeId",
                table: "ThemeTags",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ThemeTags_Tags_TagId",
                table: "ThemeTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ThemeTags_Themes_ThemeId",
                table: "ThemeTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThemeTags",
                table: "ThemeTags");

            migrationBuilder.RenameTable(
                name: "ThemeTags",
                newName: "ThemeTag");

            migrationBuilder.RenameIndex(
                name: "IX_ThemeTags_TagId",
                table: "ThemeTag",
                newName: "IX_ThemeTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThemeTag",
                table: "ThemeTag",
                columns: new[] { "ThemeId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ThemeTag_Tags_TagId",
                table: "ThemeTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThemeTag_Themes_ThemeId",
                table: "ThemeTag",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
