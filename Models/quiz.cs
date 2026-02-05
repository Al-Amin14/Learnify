using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace learnify.Models
{
    public class Quiz
    {
        [Key]
        public int Quiz_Id { get; set; }      // PK

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int Total_Marks { get; set; }

        // Foreign Key to Course
        [Required]
        public int Course_Id { get; set; }

        [ForeignKey("Course_Id")]
        public Course Course { get; set; } = null!;
    }

}
