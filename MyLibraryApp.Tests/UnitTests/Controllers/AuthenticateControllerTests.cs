using Castle.Core.Logging;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibraryApp.Tests.UnitTests.Controllers
{
    public class AuthenticateControllerTests
    {
        private readonly AuthenticateController _authenticateController;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<AuthenticateController>> _loggerMock;
        public AuthenticateControllerTests()
        {
            _userManagerMock = MockUserManager.CreateMockUserManager<AppUser>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<AuthenticateController>>();

            _authenticateController = new AuthenticateController
            (
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserSuccessfullyCreated()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@gmail.com",
                Username = "newuser",
                Password = "newuserpassword"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), UserRoles.User)).ReturnsAsync(IdentityResult.Success);

            //act
            var result = await _authenticateController.Register(registerModel);

            //assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be("User created successfully");
        }

        [Fact]
        public async Task Register_ShouldReturnConflictResult_WhenUserAlreadyExists()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@gmail.com",
                Username = "newuser",
                Password = "newuserpassword"
            };

            var existingUser = new AppUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Username
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync(existingUser);
            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync(existingUser);

            //act
            var result = await _authenticateController.Register(registerModel);

            //assert
            var objectResult = result as ConflictObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().Be("User with this username or email already exists");
        }

        [Fact]
        public async Task Register_ShouldReturnStatusCode400_WhenUserCreationFails()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@gmail.com",
                Username = "newuser",
                Password = "newuserpassword"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync((AppUser)null);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak." });

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password)).ReturnsAsync(identityResult);

            //act
            var result = await _authenticateController.Register(registerModel);

            //assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            objectResult.Value.Should().Be("User creation failed: Password too weak.");
        }

        [Fact]
        public async Task RegisterAdmin_ShouldReturnOk_WhenAdminSuccessfullyCreated()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@gmail.com",
                Username = "newuser",
                Password = "newuserpassword"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync((AppUser?)null);
            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((AppUser?)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<AppUser>(), UserRoles.Admin)).ReturnsAsync(IdentityResult.Success);

            //act
            var result = await _authenticateController.RegisterAdmin(registerModel);

            //assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.Should().BeOfType<OkObjectResult>();
            objectResult.Value.Should().Be("Admin created successfully");
        }

        [Fact]
        public async Task RegisterAdmin_ShouldReturnConflictResult_WhenAdminAlreadyExists()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newadmin@gmail.com",
                Username = "newadmin",
                Password = "newadminpassword"
            };

            var existingUser = new AppUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Username
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync(existingUser);
            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync(existingUser);

            //act
            var result = await _authenticateController.RegisterAdmin(registerModel);

            //assert
            var objectResult = result as ConflictObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().Be("User with this username or email already exists");
        }

        [Fact]
        public async Task RegisterAdmin_ShouldReturnStatusCode400_WhenAdminCreationFails()
        {
            //arrange
            var registerModel = new RegisterModel
            {
                Email = "newadmin@gmail.com",
                Username = "newadmin",
                Password = "newadminpassword"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((AppUser)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(registerModel.Email)).ReturnsAsync((AppUser)null);

            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Password too weak." });

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<AppUser>(), registerModel.Password)).ReturnsAsync(identityResult);

            //act
            var result = await _authenticateController.RegisterAdmin(registerModel);

            //assert
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            objectResult.Value.Should().Be("User creation failed: Password too weak.");
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenTokenCreated()
        {
            //arrange
            var loginModel = new LoginModel
            {
                Email = "testuser",
                Password = "userpassword"
            };

            var fakeUser = new AppUser
            {
                Id = "1",
                Email = loginModel.Email,
                UserName = "testuser"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email)).ReturnsAsync(fakeUser);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(fakeUser, loginModel.Password)).ReturnsAsync(true);

            var jwtToken = "supercooljwttoken";
            _tokenServiceMock.Setup(ts => ts.CreateToken(It.IsAny<AppUser>())).ReturnsAsync(jwtToken);

            //act
            var result = await _authenticateController.Login(loginModel);

            //assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(new { Token = jwtToken });
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserEmailNotFound()
        {
            //arrange
            var loginModel = new LoginModel
            {
                Email = "testuser",
                Password = "userpassword"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email)).ReturnsAsync((AppUser)null);

            //act
            var result = await _authenticateController.Login(loginModel);

            //assert
            var objectResult = result as UnauthorizedObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().Be("Invalid credentials.");
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIncorrect()
        {
            //arrange
            var loginModel = new LoginModel
            {
                Email = "testuser",
                Password = "userpassword"
            };
            var fakeUser = new AppUser
            {
                Id = "1",
                Email = loginModel.Email,
                UserName = "testuser"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(loginModel.Email)).ReturnsAsync(fakeUser);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(fakeUser, loginModel.Password)).ReturnsAsync(false);

            //act
            var result = await _authenticateController.Login(loginModel);

            //assert
            var objectResult = result as UnauthorizedObjectResult;
            objectResult.Should().NotBeNull();
            objectResult.Value.Should().Be("Invalid credentials.");
        }
    }
}
