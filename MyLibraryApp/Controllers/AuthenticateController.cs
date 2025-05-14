using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticateController> _logger;
        public AuthenticateController(UserManager<AppUser> userManager, ITokenService tokenService, ILogger<AuthenticateController> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            var usernameExists = await _userManager.FindByNameAsync(registerModel.Username);
            var emailExists = await _userManager.FindByEmailAsync(registerModel.Email);
            
            if (usernameExists != null || emailExists != null)
                return Conflict("User with this username or email already exists");

            var user = new AppUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Username
                //EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, UserRoles.User);
            
            return Ok("User created successfully");
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = UserRoles.Admin)] //Added after creating first admin
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel registerModel)
        {
            var usernameExists = await _userManager.FindByNameAsync(registerModel.Username);
            var emailExists = await _userManager.FindByEmailAsync(registerModel.Email);

            if (usernameExists != null || emailExists != null)
                return Conflict("User with this username or email already exists");

            var user = new AppUser
            {
                Email = registerModel.Email,
                UserName = registerModel.Username
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            return Ok("Admin created successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            _logger.LogInformation($"Login attempt: {loginModel.Email}");

            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                _logger.LogInformation("Login failed, user not found.");
                return Unauthorized("Invalid credentials.");
            }
                
            if(!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                _logger.LogInformation("Login failed: wrong password");
                return Unauthorized("Invalid credentials.");
            }

            var token = await _tokenService.CreateToken(user);

            _logger.LogInformation("Login successful: token issued");

            return Ok(new { Token = token });
        }
    }
}
