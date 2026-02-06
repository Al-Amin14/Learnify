using Microsoft.AspNetCore.Identity;

namespace Learnify.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string RoleType { get; set; }
       
    }
}
