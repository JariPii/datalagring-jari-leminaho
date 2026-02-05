using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCourseCodeFromCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Courses_CourseCode",
                table: "CourseSessions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Courses_CourseCode",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseCode",
                table: "Courses");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "CourseSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_CourseId",
                table: "CourseSessions",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseSessions_Courses_CourseId",
                table: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessions_CourseId",
                table: "CourseSessions");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseSessions");

            migrationBuilder.AddColumn<string>(
                name: "CourseCode",
                table: "Courses",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSessions_Courses_CourseCode",
                table: "CourseSessions",
                column: "CourseCode",
                principalTable: "Courses",
                principalColumn: "CourseCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
