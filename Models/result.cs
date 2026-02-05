using learnify.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practice.Models
{
    public class Result
    {
        [Key]
        public int Result_Id { get; set; }    // PK

        // Foreign Key to Student
        [Required]
        public int Student_Id { get; set; }

        [ForeignKey("Student_Id")]
        public Student Student { get; set; } = null!;

        // Foreign Key to Quiz
        [Required]
        public int Quiz_Id { get; set; }

        [ForeignKey("Quiz_Id")]
        public Quiz Quiz { get; set; } = null!;

        [Required]
        public string Answer_Text { get; set; } = string.Empty;

        [Required]
        public int Marks_Obtained { get; set; }
    }
}
