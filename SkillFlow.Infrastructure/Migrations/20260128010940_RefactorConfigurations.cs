using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Locations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Enrollments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Competences",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Attendees",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseSessionId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseSessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_EndDate",
                table: "CourseSessions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSessions_StartDate",
                table: "CourseSessions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Competences_Name",
                table: "Competences",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseSessionId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessions_EndDate",
                table: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_CourseSessions_StartDate",
                table: "CourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_Competences_Name",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Attendees");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");
        }
    }
}
