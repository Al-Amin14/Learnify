using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learnify.Migrations
{
    /// <inheritdoc />
    public partial class quizupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "Quizs",
                newName: "TeacherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "Quizs",
                newName: "StudentID");
        }
    }
}
