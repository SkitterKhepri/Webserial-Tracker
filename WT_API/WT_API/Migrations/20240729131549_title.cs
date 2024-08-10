using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
    public partial class title : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "titleXPath",
                table: "Serials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "titleXPath",
                table: "Serials");
        }
    }
}
