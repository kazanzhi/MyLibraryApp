using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using MyLibraryApp.Services;
using MyLibraryApp.Tests.UnitTests.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyLibraryApp.Tests.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly ITokenService _tokenService;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<ILogger<TokenService>> _loggerMock;
        public TokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _userManagerMock = MockUserManager.CreateMockUserManager<AppUser>();
            _loggerMock = new Mock<ILogger<TokenService>>();

            _configurationMock.Setup(c => c["JWT:Secret"]).Returns("SuperSecretKey123SuperSuperSecure");
            _configurationMock.Setup(c => c["JWT:ValidIssuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["JWT:ValidAudience"]).Returns("TestAudience");

            _tokenService = new TokenService(
                _configurationMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateToken_ShouldGenerateValidJwtToken()
        {
            //arrange
            var user = new AppUser
            {
                Id = "123",
                Email = "example@gmail.com",
                UserName = "testUser"
            };

            var roles = new List<string> { "User" };

            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(roles);

            //act
            var tokenString = await _tokenService.CreateToken(user);

            //assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            token.Issuer.Should().Be("TestIssuer");
            token.Audiences.Should().ContainSingle().Which.Should().Be("TestAudience");
            token.ValidTo.Should().BeAfter(DateTime.UtcNow);
            token.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            token.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            token.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
        }

        [Fact]
        public async Task CreateToken_ShouldNotContainRoleClaims_WhenUserNoRoles()
        {
            //arrange
            var user = new AppUser
            {
                Id = "123",
                Email = "example@gmail.com",
                UserName = "testUser"
            };

            var emptyRoles = new List<string>();

            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(emptyRoles);

            //act
            var tokenString = await _tokenService.CreateToken(user);

            //assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            token.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
        }
    }
}
