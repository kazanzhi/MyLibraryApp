using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Category?> CreateCategory(CategoryDto categoryDto)
        {
            var categoryExist = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryDto.Name);

            if (categoryExist is not null)
                return null;

            var createdCategory = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(createdCategory);
            await _context.SaveChangesAsync();

            return createdCategory;
        }

        public async Task<int> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);

            if (category is null)
                return 0;

            _context.Categories.Remove(category);

            return await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return categories;
        }

        public async Task<Category?> GetCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            
            return category;
        }

        public async Task<int> UpdateCategory(int categoryId, CategoryDto categoryDto)
        {
            var updatedCategory = await _context.Categories.FindAsync(categoryId);
            if(updatedCategory is null)
                return 0;

            updatedCategory.Name = categoryDto.Name;
            
            return await _context.SaveChangesAsync();
        }
    }
}
