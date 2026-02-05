


using System.ComponentModel.DataAnnotations;

namespace learnify.Models
{
    public class Teacher
    {
        [Key]
        public int Teacher_Id { get; set; }   // PK

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
