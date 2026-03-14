using learnify.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practice.Models
{
    public class Enrollment
    {
        [Key]
        public int Enrollment_Id { get; set; }   // PK

        // Foreign Key to Student
        [ValidateNever]
        public string Student_Id { get; set; }


        // Foreign Key to Course
        [Required]
        public int Course_Id { get; set; }

        [ForeignKey("Course_Id")]
        [ValidateNever]
        public Course Course { get; set; } = null!;

        // Optional: Enrollment date
        [Required]
        public DateTime Enrolled_On { get; set; } = DateTime.Now;
    }
}
