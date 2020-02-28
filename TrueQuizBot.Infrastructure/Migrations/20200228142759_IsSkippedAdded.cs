using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class IsSkippedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsScipped",
                schema: "TQB",
                table: "AnswerStatistic",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsScipped",
                schema: "TQB",
                table: "AnswerStatistic");
        }
    }
}
