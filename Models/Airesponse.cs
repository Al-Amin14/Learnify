using System.ComponentModel.DataAnnotations;

namespace Learnify.Models
{
    public class ApiInteraction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string UserMessage { get; set; } = string.Empty;

        [Required]
        public string ApiResponse { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}