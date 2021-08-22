using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Research_Repository_DataAccess.Migrations
{
    public partial class UpdatedItemModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Items",
                newName: "SuggestedTheme");

            migrationBuilder.AddColumn<string>(
                name: "Abstract",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedUse",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Collaborator",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "KeyInsights",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Limitations",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Methodology",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Sensitivity",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SuggestedProject",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedTags",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedTeam",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abstract",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ApprovedUse",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Collaborator",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "KeyInsights",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Limitations",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Methodology",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Sensitivity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SuggestedProject",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SuggestedTags",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SuggestedTeam",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "SuggestedTheme",
                table: "Items",
                newName: "Description");
        }
    }
}
