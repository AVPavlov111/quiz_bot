using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrueQuizBot.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TQB");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "TQB",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AnswerStatistic",
                schema: "TQB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionIndex = table.Column<int>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    Answer = table.Column<string>(nullable: true),
                    PointsNumber = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerStatistic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerStatistic_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "TQB",
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalData",
                schema: "TQB",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 32, nullable: true),
                    LastName = table.Column<string>(maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 16, nullable: true),
                    Email = table.Column<string>(maxLength: 64, nullable: true),
                    IsAcceptedPersonalDataProcessing = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
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
                name: "IX_AnswerStatistic_UserId",
                schema: "TQB",
                table: "AnswerStatistic",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalData_UserId",
                schema: "TQB",
                table: "PersonalData",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerStatistic",
                schema: "TQB");

            migrationBuilder.DropTable(
                name: "PersonalData",
                schema: "TQB");

            migrationBuilder.DropTable(
                name: "User",
                schema: "TQB");
        }
    }
}
