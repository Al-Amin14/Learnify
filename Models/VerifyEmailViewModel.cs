using System.ComponentModel.DataAnnotations;

namespace Learnify.Models
{
    public class VerifyEmailModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

    }
}