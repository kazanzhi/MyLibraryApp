using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using MyLibraryApp.Repositories;

namespace MyLibraryApp.Tests.UnitTests.Repositories
{
    public class UserBookRepositoryTests
    {
        private readonly DataContext _context;
        private readonly IUserBookRepository _userBookRepository;
        public UserBookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _userBookRepository = new UserBookRepository(_context);
        }

        [Fact]
        public async Task AddUserBookAsync_ShouldAddBookToUserLibrary()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdBook = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();

            var createdBookId = createdBook.Id;

            var userId = "test_user_Id";

            //act
            var result = await _userBookRepository.AddUserBookAsync(userId, createdBookId);

            //assert
            result.Should().Be(1);

            var userBooks = await _context.UserBooks.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == createdBookId);
            userBooks.Should().NotBeNull();
            userBooks.Book.Author.Should().Be("Gu Zhen Ren");
            userBooks.Book.Title.Should().Be("Reverend Insanity");
        }

        [Fact]
        public async Task AddUserBookAsync_ShouldReturnZero_WhenSameBookAlreadyInLibrary()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdBook = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();
            var createdBookId = createdBook.Id;

            var userId = "test_user_Id";

            var userBook = new UserBook
            {
                BookId = createdBookId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };
            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            //act
            var result = await _userBookRepository.AddUserBookAsync(userId, createdBookId);

            //assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task AddUserBookAsync_ShouldReturnZero_WhenSameBookNotFound()
        {
            //arrange
            var userId = "test_user_Id";
            var bookId = 999;

            //act
            var result = await _userBookRepository.AddUserBookAsync(userId, bookId);

            //assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetUserBooksAsync_ShouldReturnListOfUserBooks()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdBook = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();
            var createdBookId = createdBook.Id;

            var userId = "test_user_Id";

            var userBook = new UserBook
            {
                BookId = createdBookId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };
            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            //act
            var result = await _userBookRepository.GetUserBooksAsync(userId);

            //assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result.Should().BeOfType<List<UserBook>>();
            result[0].Book.Category.Name.Should().Be("Dark");
            result[0].Book.Title.Should().Be("Reverend Insanity");
        }

        [Fact]
        public async Task GetUserBooksAsync_ShouldReturnEmpty_WhenNoUserBooks()
        {
            //arrange
            var userId = "test_user_Id";

            //act
            var result = await _userBookRepository.GetUserBooksAsync(userId);

            //assert
            result.Should().BeEmpty();
            result.Should().BeOfType<List<UserBook>>();
        }

        [Fact]
        public async Task RemoveUserBookAsync_ShouldRemoveUserBookFromLibraryAndReturnOne()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var createdBook = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();

            var createdBookId = createdBook.Id;

            var userId = "test_user_Id";

            var userBook = new UserBook
            {
                BookId = createdBookId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };
            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();

            //act
            var result = await _userBookRepository.RemoveUserBookAsync(userId, createdBookId);

            //assert
            result.Should().Be(1);

            var userBookAfterDeletion = await _context.UserBooks.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == createdBookId);
            userBookAfterDeletion.Should().BeNull();
        }

        [Fact]
        public async Task RemoveUserBookAsync_ShouldReturnZeroWhenUserBookNotFound()
        {
            //arrange
            var userId = "test_user_Id";
            var bookId = 999;

            //act
            var result = await _userBookRepository.RemoveUserBookAsync(userId, bookId);

            //assert
            result.Should().Be(0);
        }
    }
}