using MyLibraryApp.Dtos;
using MyLibraryApp.Models;

namespace MyLibraryApp.Interfaces
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooks();
        Task<Book> GetBook(int bookId);

        Task<Book> CreateBook(BookDto bookDto);
        Task<int> DeleteBook(int bookId);
        Task<int> UpdateBook(int bookId, BookDto bookDto);
    }
}
