using MyLibraryApp.Models;

namespace MyLibraryApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
