using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace learnify.Models
{
    public class Course
    {
        [Key]
        public int Course_Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Foreign key to AspNetUsers
        public string Teacher_Id { get; set; } = string.Empty; // GUID of user with role Teacher

        // No navigation property needed for Teacher because it's in AspNetUsers

        // Foreign Key for Classes
        [Required]
        public int Classes_id { get; set; }

        [ForeignKey("Classes_id")]
        [ValidateNever]
        public Classes Classes { get; set; } = null!;
    }
}