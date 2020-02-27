using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class TrueLuckyDialogPersonalDataAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrueLuckyPersonalData",
                schema: "TQB",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 16, nullable: true),
                    CompanyName = table.Column<string>(maxLength: 128, nullable: true),
                    Position = table.Column<string>(maxLength: 128, nullable: true),
                    Interests = table.Column<string>(maxLength: 4000, nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrueLuckyPersonalData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrueLuckyPersonalData_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "TQB",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrueLuckyPersonalData_UserId",
                schema: "TQB",
                table: "TrueLuckyPersonalData",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrueLuckyPersonalData",
                schema: "TQB");
        }
    }
}
