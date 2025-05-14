using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;
        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Book?> CreateBook(BookDto bookDto)
        {
            var bookExist = await _context.Books
                .FirstOrDefaultAsync(b => b.Title == bookDto.Title && b.Author == bookDto.Author);

            if (bookExist is not null)
                return null;

            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == bookDto.Category);

            if (existingCategory is null)
                return null;
            
            var createdBook = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                Content = bookDto.Content,
                Category = existingCategory,
                PublishedYear = bookDto.PublishedYear
            };

            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();
                
            return createdBook;
        }

        public async Task<int> DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book is null)
                return 0;

            _context.Books.Remove(book);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetBooks()
        {
            var books = await _context.Books
                .Include(b => b.Category)
                .ToListAsync();
            
            return books;
        }

        public async Task<Book?> GetBook(int bookId)
        {
            var book = await _context.Books
                .Include(c => c.Category)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            return book;
        }

        public async Task<int> UpdateBook(int bookId, BookDto bookDto)
        {
            var book = await _context.Books
                .Include(c => c.Category)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null)
            {
                Console.WriteLine($"Book with id: {bookId} not found.");
                return 0;
            }

            var categoryExist = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == bookDto.Category);

            if (categoryExist is null)
            {
                Console.WriteLine($"Category with name: {bookDto.Category} not found.");
                return 0;
            }

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.CategoryId = categoryExist.Id;
            book.Content = bookDto.Content;
            book.PublishedYear = bookDto.PublishedYear;

            return await _context.SaveChangesAsync();
        }
    }
}