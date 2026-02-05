using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace learnify.Models
{
    public class Question
    {

        [Key]
        public int Question_Id { get; set; }    // PK

        [Required]
        public string Question_Text { get; set; } = string.Empty;

        // Optional: Model can later include max marks per question
        [Required]
        public int Marks { get; set; }

        // Foreign Key to Quiz
        [Required]
        public int Quiz_Id { get; set; }

        [ForeignKey("Quiz_Id")]
        public Quiz Quiz { get; set; } = null!;
    }
}
