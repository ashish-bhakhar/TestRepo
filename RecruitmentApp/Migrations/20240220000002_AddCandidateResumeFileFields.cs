using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitmentApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateResumeFileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeFilePath",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeFilePath",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "Candidates");
        }
    }
}
