using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using MyLibraryApp.Repositories;

namespace MyLibraryApp.Tests.UnitTests.Repositories
{
    public class BookRepositoryTests
    {
        private readonly DataContext _context;
        private readonly IBookRepository _bookRepository;
        public BookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _bookRepository = new BookRepository(_context);
        }

        [Fact]
        public async Task CreateBook_ShouldCreateBook()
        {
            //arange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var bookDto = new BookDto
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = "Dark",
                PublishedYear = 2012
            };

            //act
            var result = await _bookRepository.CreateBook(bookDto);

            //assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Reverend Insanity");
            var books = await _context.Books.ToListAsync();
            books.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateBook_ShouldReturnNull_WhenBookWithSameAuthorAndTitleAlreadyExists()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var book = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookDto = new BookDto
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "...",
                Category = "Dark",
                PublishedYear = 2012
            };

            //Act
            var result = await _bookRepository.CreateBook(bookDto);

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateBook_ShouldReturnNull_WhenBookCategoryDoesNotExists()
        {
            //arrange
            var bookDto = new BookDto
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "...",
                Category = "Dark",
                PublishedYear = 2012
            };

            //act
            var result = await _bookRepository.CreateBook(bookDto);

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnOne()
        {
            //arrange
            var category = new Category
            {
                Name = "Fantasy"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var book = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookId = book.Id;

            //act
            var result = await _bookRepository.DeleteBook(bookId);

            //assert
            result.Should().Be(1);
            var books = await _context.Books.ToListAsync();
            books.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnZero_WhenBookDoesNotExists()
        {
            //arrange

            //act
            var result = await _bookRepository.DeleteBook(999);

            //assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnListOfBooks() 
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var book = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category,
                PublishedYear = 2012
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            //act
            var result = await _bookRepository.GetBooks();

            //assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<List<Book>>();

            var books = await _context.Books.ToListAsync();
            books.Should().HaveCount(1);
            books[0].Author.Should().Be("Gu Zhen Ren");
            books[0].Title.Should().Be("Reverend Insanity");
        }

        [Fact]
        public async Task GetBooks_ShouldReturnEmptyList_WhenNoBooks() 
        {
            //arrange

            //act
            var result = await _bookRepository.GetBooks();

            //assert
            result.Should().BeEmpty();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetBook_ShouldReturnBook()
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

            var bookId = createdBook.Id;

            //act
            var result = await _bookRepository.GetBook(bookId);

            //assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Book>();

            var book = await _context.Books.ToListAsync();
            book.Should().HaveCount(1);
            book[0].Author.Should().Be("Gu Zhen Ren");
            book[0].Title.Should().Be("Reverend Insanity");
        }

        [Fact]
        public async Task GetBook_ShouldReturnNull_WhenBookNotDoesNotExists()
        {
            //arrange

            //act
            var result = await _bookRepository.GetBook(999);

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnOne()
        {
            //arrange
            var category = new List<Category>
            {
                new Category
                {
                    Name = "Detectives"
                },
                new Category
                {
                    Name = "Dark"
                }
            };
            _context.Categories.AddRange(category);
            await _context.SaveChangesAsync();

            var createdBook = new Book
            {
                Title = "Reverend Insanity",
                Author = "Gu Zhen Ren",
                Content = "A lot of content...",
                Category = category[0],
                PublishedYear = 2012
            };
            _context.Books.Add(createdBook);
            await _context.SaveChangesAsync();

            var createdBookId = createdBook.Id;

            var bookDto = new BookDto
            {
                Title = "Lord of Mysteries",
                Author = "Cuttlefish That Loves Diving",
                Content = "...",
                Category = "Detectives",
                PublishedYear = 2017
            };

            //act
            var result = await _bookRepository.UpdateBook(createdBookId, bookDto);

            //assert
            result.Should().Be(1);

            var book = await _context.Books.ToListAsync();
            book.Should().HaveCount(1);
            book[0].Author.Should().Be("Cuttlefish That Loves Diving");
            book[0].Title.Should().Be("Lord of Mysteries");
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnZero_WhenBookNotFound()
        {
            //arrange
            var category = new Category
            {
                Name = "Detectives"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var bookDto = new BookDto
            {
                Title = "Lord of Mysteries",
                Author = "Cuttlefish That Loves Diving",
                Content = "...",
                Category = "Detectives",
                PublishedYear = 2017
            };

            //act
            var result = await _bookRepository.UpdateBook(999, bookDto);

            //assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnZero_WhenBookCategoryNotFound()
        {
            //arrange
            var category = new Category
            {
                Name = "Dark"
            };
            _context.Categories.AddRange(category);
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

            var bookDto = new BookDto
            {
                Title = "Lord of Mysteries",
                Author = "Cuttlefish That Loves Diving",
                Content = "...",
                Category = "Detectives",
                PublishedYear = 2017
            };

            //act
            var result = await _bookRepository.UpdateBook(createdBookId, bookDto);

            //assert
            result.Should().Be(0);
        }
    }
}
