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

        // Foreign Key
        [Required]
        public int Teacher_Id { get; set; }

        [ForeignKey("Teacher_Id")]
        public Teacher Teacher { get; set; } = null!;

        // Foreign Key
        [Required]
        public int Classes_id { get; set; }

        [ForeignKey("Classes_id")]
        public Classes Classes { get; set; } = null!;
    }
}
