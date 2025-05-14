using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserBookController : ControllerBase
    {
        private readonly IUserBookRepository _userBookRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserBookController> _logger;
        public UserBookController(IUserBookRepository userBookRepository, UserManager<AppUser> userManager, ILogger<UserBookController> logger)
        {
            _userBookRepository = userBookRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult<List<UserBook>>> GetAllUserBooks()
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
                return Unauthorized();

            var userBooks = await _userBookRepository.GetUserBooksAsync(userId);

            if (!userBooks.Any())
                return NoContent();

            return Ok(userBooks);
        }

        [HttpPost("{bookId}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult> AddBookToUser(int bookId)
        {
            _logger.LogInformation("Start finding user Id.");

            var user = await _userManager.GetUserAsync(User);

            if(user is null)
            {
                _logger.LogError("User not found.");
                return Unauthorized();
            }

            var result = await _userBookRepository.AddUserBookAsync(user.Id, bookId);
            if (result == 0)
                return Conflict("Book already in your library or does not exist.");

            return Ok();
        }

        [HttpDelete("{bookId}")]
        [Authorize(Roles = UserRoles.User)]
        public async Task<ActionResult> DeleteBookFromUser(int bookId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
                return Unauthorized();

            var result = await _userBookRepository.RemoveUserBookAsync(userId, bookId);
            if (result == 0)
                return NotFound("Book not found in your library");

            return Ok();
        }
    }
}
