using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyLibraryApp.Controllers;
using MyLibraryApp.Dtos;
using MyLibraryApp.Interfaces;
using MyLibraryApp.Models;

namespace MyLibraryApp.Tests.UnitTests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly CategoryController _categoryController;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        public CategoryControllerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();

            _categoryController = new CategoryController
            (
                _categoryRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetCategory_ShouldReturnOk()
        {
            //arrange
            var category = new Category
            {
                Id = 1,
                Name = "Science"
            };

            _categoryRepositoryMock.Setup(repo => repo.GetCategory(category.Id)).ReturnsAsync(category);

            //act
            var result = await _categoryController.GetCategory(category.Id);

            //assert
            var okResult = result.Result as OkObjectResult;
            result.Result.Should().NotBeNull();
            okResult.Value.Should().Be(category);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnNotFound_WhenCurrentCategoryDoesNotExists()
        {
            //arrange
            var categoryId = 1;
            _categoryRepositoryMock.Setup(repo => repo.GetCategory(categoryId)).ReturnsAsync((Category?)null);

            //act
            var result = await _categoryController.GetCategory(categoryId);

            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult!.Value.Should().Be($"Category with Id: {categoryId} not found.");
        }

        [Fact]
        public async Task GetCategories_ShouldReturnOk()
        {
            //arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Science"},
                new Category { Id = 2, Name = "Technology"}
            };

            _categoryRepositoryMock.Setup(repo => repo.GetCategories()).ReturnsAsync(categories);

            //act
            var result = await _categoryController.GetCategories();

            //assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(categories);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnCreatedAtAction()
        {
            //arrange
            var categoryDto = new CategoryDto
            {
                Name = "Science"
            };

            _categoryRepositoryMock.Setup(repo => repo.CreateCategory(categoryDto)).ReturnsAsync(new Category());

            //act
            var result = await _categoryController.CreateCategory(categoryDto);

            //assert
            result.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.ActionName.Should().Be(nameof(_categoryController.GetCategory));

            result.Result.Should().BeOfType<CreatedAtActionResult>()
                .Which.Value.Should().BeOfType<Category>();
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnConflict_WhenCategoryAlreadyExists()
        {
            //arrange
            var categoryDto = new CategoryDto
            {
                Name = "Science"
            };

            _categoryRepositoryMock.Setup(repo => repo.CreateCategory(categoryDto)).ReturnsAsync((Category?)null);

            //act
            var result = await _categoryController.CreateCategory(categoryDto);

            //assert
            result.Result.Should().BeOfType<ConflictObjectResult>()
                .Which.Value.Should().Be("Category with this name already exists.");
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnNoContent()
        {
            //arrange
            var categoryId = 1;

            _categoryRepositoryMock.Setup(repo => repo.DeleteCategory(categoryId)).ReturnsAsync(1);

            //act
            var result = await _categoryController.DeleteCategory(categoryId);

            //assert
            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnNotFound_WhenCategoryNotFound()
        {
            //arrange
            var categoryId = 1;

            _categoryRepositoryMock.Setup(repo => repo.DeleteCategory(categoryId)).ReturnsAsync(0);

            //act
            var result = await _categoryController.DeleteCategory(categoryId);

            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Category with ID {categoryId} not found.");
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNoContent()
        {
            //arrange
            var categoryId = 1;
            var categoryDto = new CategoryDto
            {
                Name = "Science"
            };

            _categoryRepositoryMock.Setup(repo => repo.UpdateCategory(categoryId, categoryDto)).ReturnsAsync(1);

            //act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDto);

            //assert
            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNotFound_WhenCategoryDoesNotExists()
        {
            //arrange
            var categoryId = 1;
            var categoryDto = new CategoryDto
            {
                Name = "Science"
            };

            _categoryRepositoryMock.Setup(repo => repo.UpdateCategory(categoryId, categoryDto)).ReturnsAsync(0);

            //act
            var result = await _categoryController.UpdateCategory(categoryId, categoryDto);

            //assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Category with ID {categoryId} not found.");
        }
    }
}
