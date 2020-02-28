using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class PersonalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonalData",
                schema: "TQB");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                schema: "TQB",
                table: "TrueLuckyPersonalData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailAddress",
                schema: "TQB",
                table: "TrueLuckyPersonalData");

            migrationBuilder.CreateTable(
                name: "PersonalData",
                schema: "TQB",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsAcceptedPersonalDataProcessing = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalData_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "TQB",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalData_UserId",
                schema: "TQB",
                table: "PersonalData",
                column: "UserId",
                unique: true);
        }
    }
}
