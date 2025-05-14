using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyLibraryApp.Data;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;
using MyLibraryApp.Repositories;

namespace MyLibraryApp.Tests.UnitTests.Repositories
{
    public class CategoryRepositoryTests
    {
        private readonly DataContext _context;
        private readonly ICategoryRepository _categoryRepository;
        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _categoryRepository = new CategoryRepository(_context);
        }

        [Fact]
        public async Task CreateCategory_ShouldCreateCategory()
        {
            //arange
            var category = new CategoryDto
            {
                Name = "Science"
            };

            //act
            var result = await _categoryRepository.CreateCategory(category);

            //assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Category>();
            result.Name.Should().Be("Science");

            var categories = await _context.Categories.ToListAsync();

            categories.Should().HaveCount(1);   
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnNull_WhenCategoryAlreadyExists()
        {
            //arrange
            var category = new Category
            {
                Name = "Science"
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var newCategoryDto = new CategoryDto
            {
                Name = "Science"
            };

            //act
            var result = await _categoryRepository.CreateCategory(newCategoryDto);

            //assert
            result.Should().BeNull();
            var categories = await _context.Categories.ToListAsync();

            categories.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnOne()
        {
            //arrange
            var category = new Category
            {
                Name = "Science"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            var categoryId = category.Id;

            //act
            var result = await _categoryRepository.DeleteCategory(categoryId);

            //assert
            result.Should().Be(1);
            var categories = await _context.Categories.ToListAsync();
            categories.Should().BeEmpty();

        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnZero_WhenCategoryDoesNotExist()
        {
            //arrange

            //act
            var result = await _categoryRepository.DeleteCategory(1);

            //assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnListOfCategories()
        {
            //arrange
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Science"
                },
                new Category
                {
                    Name = "Fantasy"
                },
                new Category
                {
                    Name = "History"
                }
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            //act
            var result = await _categoryRepository.GetCategories();

            //assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeOfType<List<Category>>();
            result.Select(c => c.Name).Should().Contain(new[] { "Science", "Fantasy", "History" });
        }

        [Fact]
        public async Task GetCategories_ShouldReturnEmpty_WhenNoCategories()
        {
            //arrange

            //act
            var result = await _categoryRepository.GetCategories();

            //assert
            result.Should().BeEmpty();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCategory_ShouldReturnCategory()
        {
            //arrange
            var category = new Category
            {
                Name = "Science"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            var categoryId = category.Id;

            //act
            var result = await _categoryRepository.GetCategory(categoryId);

            //assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Science");
        }

        [Fact]
        public async Task GetCategory_ShouldReturnNull_WhenCategoryNotFound()
        {
            //arrange

            //act
            var result = await _categoryRepository.GetCategory(999);

            //assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnOne()
        {
            //arrange
            var category = new Category
            {
                Name = "science"
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            var categoryId = category.Id;

            var categoryDto = new CategoryDto
            {
                Name = "Technology"
            };

            //act
            var result = await _categoryRepository.UpdateCategory(categoryId, categoryDto);

            //assert
            result.Should().Be(1);
            var categories = await _context.Categories.ToListAsync();
            categories.Should().HaveCount(1);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnZero_WhenCategoryDoesNotExists()
        {
            //arrange
            var categoryDto = new CategoryDto
            {
                Name = "Science"
            };

            //act
            var result = await _categoryRepository.UpdateCategory(1, categoryDto);

            //assert
            result.Should().Be(0);
        }
    }
}