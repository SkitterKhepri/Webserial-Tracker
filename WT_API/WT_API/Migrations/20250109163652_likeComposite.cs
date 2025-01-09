using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
    public partial class likeComposite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LikedSerials",
                table: "LikedSerials");

            migrationBuilder.DropColumn(
                name: "id",
                table: "LikedSerials");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "LikedSerials",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikedSerials",
                table: "LikedSerials",
                columns: new[] { "userId", "serialId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LikedSerials",
                table: "LikedSerials");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "LikedSerials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "LikedSerials",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikedSerials",
                table: "LikedSerials",
                column: "id");
        }
    }
}
