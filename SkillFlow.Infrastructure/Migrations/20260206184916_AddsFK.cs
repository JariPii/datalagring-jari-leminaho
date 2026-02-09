using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddsFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
