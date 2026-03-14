using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learnify.Migrations
{
    /// <inheritdoc />
    public partial class quiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "Quizs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentID",
                table: "Quizs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question",
                table: "Quizs");

            migrationBuilder.DropColumn(
                name: "StudentID",
                table: "Quizs");
        }
    }
}
