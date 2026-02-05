using System.ComponentModel.DataAnnotations;

namespace learnify.Models
{
    public class Classes
    {
        [Key]
        public int Classes_id { get; set; }

        [Required]
        public string ClassesName { get; set; } = string.Empty;

        [Required]
        public string ClassesDescription { get; set;} = string.Empty;

    }
}
