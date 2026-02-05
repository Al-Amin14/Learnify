using learnify.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practice.Models
{
    public class Enrollment
    {
        [Key]
        public int Enrollment_Id { get; set; }   // PK

        // Foreign Key to Student
        [Required]
        public int Student_Id { get; set; }

        [ForeignKey("Student_Id")]
        public Student Student { get; set; } = null!;

        // Foreign Key to Course
        [Required]
        public int Course_Id { get; set; }

        [ForeignKey("Course_Id")]
        public Course Course { get; set; } = null!;

        // Optional: Enrollment date
        [Required]
        public DateTime Enrolled_On { get; set; } = DateTime.Now;
    }
}
