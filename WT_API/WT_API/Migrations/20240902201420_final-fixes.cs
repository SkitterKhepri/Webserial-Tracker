using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
  public partial class finalfixes : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      // Step 1: Convert existing nvarchar values to integer values
      migrationBuilder.Sql(@"
                UPDATE Serials
                SET status = CASE
                    WHEN status = 'Completed' THEN 1
                    WHEN status = 'Ongoing' THEN 2
                    WHEN status = 'Hiatus' THEN 3
                    WHEN status = 'Abandoned' THEN 4
                    ELSE 0  -- Handle any unexpected values if necessary
                END;
            ");

      // Step 2: Alter column type from nvarchar to int
      migrationBuilder.AlterColumn<int>(
          name: "status",
          table: "Serials",
          type: "int",
          nullable: false,
          oldClrType: typeof(string),
          oldType: "nvarchar(max)");

      // Adding new columns
      migrationBuilder.AddColumn<bool>(
          name: "isLastChapter",
          table: "Chapters",
          type: "bit",
          nullable: false,
          defaultValue: false);

      migrationBuilder.AddColumn<bool>(
          name: "reviewStatus",
          table: "Chapters",
          type: "bit",
          nullable: false,
          defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      // Revert the changes if needed
      migrationBuilder.DropColumn(
          name: "isLastChapter",
          table: "Chapters");

      migrationBuilder.DropColumn(
          name: "reviewStatus",
          table: "Chapters");

      // Revert column type change
      migrationBuilder.AlterColumn<string>(
          name: "status",
          table: "Serials",
          type: "nvarchar(max)",
          nullable: false,
          oldClrType: typeof(int),
          oldType: "int");
    }
  }
}
