using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CritiQuest2.Server.Migrations
{
    /// <inheritdoc />
    public partial class Interaktywne2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InteractionProgress",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LessonId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalInteractiveSections = table.Column<int>(type: "int", nullable: false),
                    CompletedSections = table.Column<int>(type: "int", nullable: false),
                    TotalTimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteractionProgress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InteractionProgress_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InteractionTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TemplateConfigJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InteractiveSections",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LessonId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OrderInLesson = table.Column<int>(type: "int", nullable: false),
                    ConfigurationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractiveSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InteractiveSections_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LessonInteractionResponses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    LessonId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InteractionType = table.Column<int>(type: "int", nullable: false),
                    ResponseDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonInteractionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonInteractionResponses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonInteractionResponses_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserInteractionResponses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InteractiveSectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResponseDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletionPercentage = table.Column<int>(type: "int", nullable: false),
                    QualityScore = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInteractionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserInteractionResponses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInteractionResponses_InteractiveSections_InteractiveSectionId",
                        column: x => x.InteractiveSectionId,
                        principalTable: "InteractiveSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InteractionProgress_LessonId",
                table: "InteractionProgress",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionProgress_UserId_LessonId",
                table: "InteractionProgress",
                columns: new[] { "UserId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InteractionTemplates_Type_Category",
                table: "InteractionTemplates",
                columns: new[] { "Type", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_InteractiveSections_LessonId_OrderInLesson",
                table: "InteractiveSections",
                columns: new[] { "LessonId", "OrderInLesson" });

            migrationBuilder.CreateIndex(
                name: "IX_LessonInteractionResponses_LessonId",
                table: "LessonInteractionResponses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonInteractionResponses_UserId",
                table: "LessonInteractionResponses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractionResponses_InteractiveSectionId",
                table: "UserInteractionResponses",
                column: "InteractiveSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInteractionResponses_UserId_InteractiveSectionId",
                table: "UserInteractionResponses",
                columns: new[] { "UserId", "InteractiveSectionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InteractionProgress");

            migrationBuilder.DropTable(
                name: "InteractionTemplates");

            migrationBuilder.DropTable(
                name: "LessonInteractionResponses");

            migrationBuilder.DropTable(
                name: "UserInteractionResponses");

            migrationBuilder.DropTable(
                name: "InteractiveSections");
        }
    }
}
