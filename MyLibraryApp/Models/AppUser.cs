using Microsoft.AspNetCore.Identity;

namespace MyLibraryApp.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>(); 
    }
}
