using MyLibraryApp.Models;

namespace MyLibraryApp.Interfaces
{
    public interface IUserBookRepository
    {
        Task<List<UserBook>> GetUserBooksAsync(string userId);
        Task<int> AddUserBookAsync(string userId, int bookId);
        Task<int> RemoveUserBookAsync(string userId, int bookId);
    }
}
