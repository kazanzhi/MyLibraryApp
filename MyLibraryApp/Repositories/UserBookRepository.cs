using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Repositories
{
    public class UserBookRepository : IUserBookRepository
    {
        private readonly DataContext _context;
        public UserBookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<int> AddUserBookAsync(string userId, int bookId)
        {
            bool alreadyAdded = await _context.UserBooks.AnyAsync(ub => ub.BookId == bookId && ub.UserId == userId);
            if (alreadyAdded)
                return 0;

            var book = await _context.Books.FindAsync(bookId);
            if (book is null)
                return 0;

            var userBook = new UserBook
            {
                BookId = bookId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };

            _context.UserBooks.Add(userBook);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<UserBook>> GetUserBooksAsync(string userId)
        {
            var books = await _context.UserBooks
                .Where(u => u.UserId == userId)
                .Include(b => b.Book.Category)
                .ToListAsync();

            
            return books;
        }

        public async Task<int> RemoveUserBookAsync(string userId, int bookId)
        {
            var userBook = await _context.UserBooks.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == bookId);
            if (userBook is null)
                return 0;

            _context.UserBooks.Remove(userBook);

            return await _context.SaveChangesAsync();
        }
    }
}
