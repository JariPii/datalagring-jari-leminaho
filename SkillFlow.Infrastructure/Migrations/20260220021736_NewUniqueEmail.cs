using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewUniqueEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendees_Email",
                table: "Attendees");

            migrationBuilder.AddColumn<string>(
                name: "EmailUnique",
                table: "Attendees",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_EmailUnique",
                table: "Attendees",
                column: "EmailUnique",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendees_EmailUnique",
                table: "Attendees");

            migrationBuilder.DropColumn(
                name: "EmailUnique",
                table: "Attendees");

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_Email",
                table: "Attendees",
                column: "Email",
                unique: true);
        }
    }
}
