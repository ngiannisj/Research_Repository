using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository_DataAccess.Migrations
{
    public partial class checks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Items",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
