using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class TrueLuckyPersonalDataIsRulesAcceptedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAcceptedPersonalDataProcessing",
                schema: "TQB",
                table: "TrueLuckyPersonalData",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAcceptedPersonalDataProcessing",
                schema: "TQB",
                table: "TrueLuckyPersonalData");
        }
    }
}
