using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("{categoryId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Category>> GetCategory(int categoryId)
        {
            var category = await _categoryRepository.GetCategory(categoryId);
            if (category is null)
                return NotFound($"Category with Id: {categoryId} not found.");
            
            return Ok(category);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _categoryRepository.GetCategories();

            if (!categories.Any())
                return NoContent();
            
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            var createdCategory = await _categoryRepository.CreateCategory(categoryDto);

            if (createdCategory is null)
                return Conflict("Category with this name already exists.");

            return CreatedAtAction(nameof(GetCategory), new { categoryId = createdCategory.Id }, createdCategory);
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<bool>> DeleteCategory(int categoryId)
        {
            var deletedCategory = await _categoryRepository.DeleteCategory(categoryId);

            if(deletedCategory == 0)
                return NotFound($"Category with ID {categoryId} not found.");

            return NoContent();
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<bool>> UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            var updatedCategory = await _categoryRepository.UpdateCategory(categoryId, categoryDto);

            if (updatedCategory == 0)
                return NotFound($"Category with ID {categoryId} not found.");
            
            return NoContent();
        }
    }
}
