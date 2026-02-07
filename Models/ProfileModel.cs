using System.ComponentModel.DataAnnotations;

namespace Learnify.Models
{
    public class ProfileModel
    {
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }
    }
}