using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class CurrentQuestionIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionIndex",
                schema: "TQB",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuestionIndex",
                schema: "TQB",
                table: "User");
        }
    }
}
