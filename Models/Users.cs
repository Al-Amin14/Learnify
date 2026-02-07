using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace Learnify.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }

        [Column("RoleType")]
        public string RoleType { get; set; }

    }
}