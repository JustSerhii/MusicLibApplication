using Microsoft.AspNetCore.Identity;

namespace WebAppLab.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}