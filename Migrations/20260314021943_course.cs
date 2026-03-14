using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace learnify.Migrations
{
    /// <inheritdoc />
    public partial class course : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Teachers_Teacher_Id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enroll_Students_Student_Id",
                table: "Enroll");

            migrationBuilder.DropForeignKey(
                name: "FK_Result_Students_Student_Id",
                table: "Result");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Result_Student_Id",
                table: "Result");

            migrationBuilder.DropIndex(
                name: "IX_Enroll_Student_Id",
                table: "Enroll");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Teacher_Id",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Student_Id",
                table: "Enroll",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Teacher_Id",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Student_Id",
                table: "Enroll",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Teacher_Id",
                table: "Courses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Student_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Student_Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Teacher_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Teacher_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Result_Student_Id",
                table: "Result",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Enroll_Student_Id",
                table: "Enroll",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Teacher_Id",
                table: "Courses",
                column: "Teacher_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Teachers_Teacher_Id",
                table: "Courses",
                column: "Teacher_Id",
                principalTable: "Teachers",
                principalColumn: "Teacher_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enroll_Students_Student_Id",
                table: "Enroll",
                column: "Student_Id",
                principalTable: "Students",
                principalColumn: "Student_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Result_Students_Student_Id",
                table: "Result",
                column: "Student_Id",
                principalTable: "Students",
                principalColumn: "Student_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
