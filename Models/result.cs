using learnify.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace practice.Models
{
    public class Result
    {
        [Key]
        public int Result_Id { get; set; }    // PK

        // Foreign Key to Student
        [ValidateNever]
        public string Student_Id { get; set; }

        
        // Foreign Key to Quiz
        [Required]
        public int Quiz_Id { get; set; }

        [ForeignKey("Quiz_Id")]
        [ValidateNever]
        public Quiz Quiz { get; set; } = null!;

        [Required]
        public string Answer_Text { get; set; } = string.Empty;

        [Required]
        public int Marks_Obtained { get; set; }
    }
}
