using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyLibraryApp.Controllers;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using System.Net;

namespace MyLibraryApp.Tests.UnitTests.Controllers
{
    public class BookControllerTests
    {
        private readonly BookController _bookController;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        public BookControllerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();

            _bookController = new BookController
            (
                _bookRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetBook_ShouldReturnOk_WithBook()
        {
            //arrange
            var testBook = new Book
            {
                Id = 1,
                Author = "testAuthor",
                Title = "testTitle"
            };

            _bookRepositoryMock.Setup(repo => repo.GetBook(testBook.Id)).ReturnsAsync(testBook);

            //act
            var result = await _bookController.GetBook(testBook.Id);

            //assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(testBook);
        }

        [Fact]
        public async Task GetBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            //arrange
            var testBookId = 1;

            _bookRepositoryMock.Setup(repo => repo.GetBook(testBookId)).ReturnsAsync((Book?)null);

            //act
            var result = await _bookController.GetBook(testBookId);

            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Book with Id: {testBookId} not found.");
        }

        [Fact]
        public async Task GetBooks_ShouldReturnOk_WithListOfBooks()
        {
            //arrange
            var testBooks = new List<Book>
            {
                new Book { Id = 1, Title = "Title 1" },
                new Book { Id = 2, Title = "Title 2" }
            };

            _bookRepositoryMock.Setup(repo => repo.GetBooks()).ReturnsAsync(testBooks);

            //act
            var result = await _bookController.GetBooks();

            //assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(testBooks);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnNoContent_WithEmptyListOfBooks_WhenNoBooks()
        {
            //arrange
            var testBooks = new List<Book>();

            _bookRepositoryMock.Setup(repo => repo.GetBooks()).ReturnsAsync(testBooks);

            //act
            var result = await _bookController.GetBooks();

            //assert
            result.Result.Should().BeOfType<NoContentResult>();
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task CreateBook_ShouldReturnCreatedAtAction()
        {
            //arrange
            var testBookDto = new BookDto
            {
                Author = "test author",
                Title = "test title"
            };

            var testBook = new Book
            {
                Author = "test author",
                Title = "test title"
            };

            _bookRepositoryMock.Setup(repo => repo.CreateBook(testBookDto)).ReturnsAsync(testBook);

            //act
            var result = await _bookController.CreateBook(testBookDto);

            //assert
            result.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.ActionName.Should().Be(nameof(_bookController.GetBook));

            result.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeEquivalentTo(testBook);
        }

        [Fact]
        public async Task CreateBook_ShouldReturnConflict_WhenBookAlreadyExistsOrCategoryDoesNotExists()
        {
            //arrange
            var testBookDto = new BookDto
            {
                Author = "test author",
                Title = "test title"
            };

            _bookRepositoryMock.Setup(repo => repo.CreateBook(testBookDto)).ReturnsAsync((Book?)null);

            //act
            var result = await _bookController.CreateBook(testBookDto);

            //assert
            result.Result.Should().BeOfType<ConflictObjectResult>()
                .Which.Value.Should().Be("This book already exist or current category does not exist.");
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnNoContent()
        {
            //arrange
            var testBookDto = new BookDto
            {
                Author = "test author",
                Title = "test title"
            };

            var testBookId = 1;

            _bookRepositoryMock.Setup(repo => repo.UpdateBook(testBookId, testBookDto)).ReturnsAsync(1);

            //act
            var result = await _bookController.UpdateBook(testBookId, testBookDto);

            //assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnNotFound_WhenBookIdNotFoundOrCategoryDoesNotExists()
        {
            //arrange
            var testBookDto = new BookDto
            {
                Author = "test author",
                Title = "test title"
            };

            var testBookId = 1;

            _bookRepositoryMock.Setup(repo => repo.UpdateBook(testBookId, testBookDto)).ReturnsAsync(0);

            //act
            var result = await _bookController.UpdateBook(testBookId, testBookDto);

            //assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Book with Id {testBookId} not found or current category does not exists.");
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNoContent()
        {
            //arrange
            var testBookId = 1;

            _bookRepositoryMock.Setup(repo => repo.DeleteBook(testBookId)).ReturnsAsync(1);

            //act
            var result = await _bookController.DeleteBook(testBookId);

            //assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNotFound_WhenBookIdNotFound()
        {
            //arrange
            var testBookId = 1;

            _bookRepositoryMock.Setup(repo => repo.DeleteBook(testBookId)).ReturnsAsync(0);

            //act
            var result = await _bookController.DeleteBook(testBookId);

            //assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Book with Id {testBookId} not found.");
        }
    }
}
