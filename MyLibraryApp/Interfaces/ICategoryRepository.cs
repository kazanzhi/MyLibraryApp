using MyLibraryApp.Dtos;
using MyLibraryApp.Models;

namespace MyLibraryApp.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories();
        Task<Category> GetCategory(int categoryId);
        Task<Category> CreateCategory(CategoryDto categoryDto);
        Task<int> DeleteCategory(int categoryId);
        Task<int> UpdateCategory(int categoryId, CategoryDto categoryDto);
    }
}
