using Microsoft.AspNetCore.Identity;

namespace VerdonFileManager.Models
{
    public class AppUser:IdentityUser
    {
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
