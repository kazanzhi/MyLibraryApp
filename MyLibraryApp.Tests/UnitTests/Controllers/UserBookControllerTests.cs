using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MyLibraryApp.Controllers;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using MyLibraryApp.Tests.UnitTests.Helpers;
using System.Security.Claims;

namespace MyLibraryApp.Tests.UnitTests.Controllers
{
    public class UserBookControllerTests
    {
        private readonly UserBookController _userBookController;
        private readonly Mock<IUserBookRepository> _userBookRepositoryMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ILogger<UserBookController>> _loggerMock;
        public UserBookControllerTests()
        {
            _userBookRepositoryMock = new Mock<IUserBookRepository>();
            _userManagerMock = MockUserManager.CreateMockUserManager<AppUser>();
            _loggerMock = new Mock<ILogger<UserBookController>>();

            _userBookController = new UserBookController(
                _userBookRepositoryMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAllUserBooks_ShouldReturnOk_WithBooks()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@gmail.com"
            };

            var testUserBooks = new List<UserBook>
            {
                new UserBook{ Id = 0, Book = new Book { Id = 0, Title = "Book1" }},
                new UserBook{ Id = 1, Book = new Book { Id = 0, Title = "Book2" }}
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user}
            };

            _userManagerMock.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(testUser.Id);

            _userBookRepositoryMock.Setup(repo => repo.GetUserBooksAsync(testUser.Id)).ReturnsAsync(testUserBooks);

            //act
            var result = await _userBookController.GetAllUserBooks();

            //assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.Value.Should().BeEquivalentTo(testUserBooks);
        }

        [Fact]    
        public async Task GetAllUserBooks_ShouldReturnNoContent_WhenNoBooks()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@gmail.com"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(testUser.Id);

            _userBookRepositoryMock.Setup(repo => repo.GetUserBooksAsync(testUser.Id)).ReturnsAsync(new List<UserBook>());

            //act
            var result = await _userBookController.GetAllUserBooks();

            //assert
            result.Result.Should().BeOfType<NoContentResult>();
            result.Value.Should().BeNull();
        }

        [Fact]
        public async Task AddBookToUser_ShouldReturnOk()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@gmail.com"
            };

            int testBookId = 0;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUser.Id)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext 
            { 
                HttpContext = new DefaultHttpContext { User = user}

            };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser);

            _userBookRepositoryMock.Setup(repo => repo.AddUserBookAsync(testUser.Id,testBookId)).ReturnsAsync(1);

            //act
            var result = await _userBookController.AddBookToUser(testBookId);

            //assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task AddBookToUser_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            //arrange
            int testBookId = 0;

            //act
            var result = await _userBookController.AddBookToUser(testBookId);

            //assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task AddBookToUser_ShouldReturnConflict_WhenBookAlreadyExists()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@gmail.com"
            };

            var testUserId = testUser.Id;

            var testBookId = 0;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(testUser);

            _userBookRepositoryMock.Setup(repo => repo.AddUserBookAsync(testUserId, testBookId)).ReturnsAsync(0);

            //act
            var result = await _userBookController.AddBookToUser(testBookId);

            //assert
            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task DeleteBookFromUser_ShouldReturnOk()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test@gmail.com",
                UserName = "testuser"
            };

            var testUserId = testUser.Id;

            int testBookId = 0;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(testUserId);

            _userBookRepositoryMock.Setup(repo => repo.RemoveUserBookAsync(testUserId, testBookId)).ReturnsAsync(1);

            //act
            var result = await _userBookController.DeleteBookFromUser(testBookId);

            //assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteBookFromUser_ShouldReturnUnauthorized_WhenUserNotFound()
        {
            //arrange
            var testBookId = 0;

            //act
            var result = await _userBookController.DeleteBookFromUser(0);

            //assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task DeleteBookFromUser_ShhouldReturnNotFound_WhenNoBookInUserLibrary()
        {
            //arrange
            var testUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser",
                Email = "test@gmail.com"
            };

            var testUserId = testUser.Id;

            var testBookId = 0;

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, testUserId)
            }, "mock"));

            _userBookController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _userManagerMock.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(testUserId);

            _userBookRepositoryMock.Setup(repo => repo.RemoveUserBookAsync(testUserId, testBookId)).ReturnsAsync(0);

            //act
            var result = await _userBookController.DeleteBookFromUser(testBookId);

            //assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    } 
}
