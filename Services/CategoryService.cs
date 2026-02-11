using StockPro.Data.Entities;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ColorHex = c.ColorHex
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ColorHex = category.ColorHex
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            if (await _categoryRepository.ExistsByNameAsync(createCategoryDto.Name))
            {
                throw new InvalidOperationException($"La categoría '{createCategoryDto.Name}' ya existe.");
            }

            var category = new Category
            {
                Name = createCategoryDto.Name,
                ColorHex = createCategoryDto.ColorHex,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ColorHex = category.ColorHex
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría con ID {id}.");
            }

            if (category.Name.ToLower() != updateCategoryDto.Name.ToLower() && 
                await _categoryRepository.ExistsByNameAsync(updateCategoryDto.Name))
            {
                throw new InvalidOperationException($"La categoría '{updateCategoryDto.Name}' ya existe.");
            }

            category.Name = updateCategoryDto.Name;
            category.ColorHex = updateCategoryDto.ColorHex;
            category.UpdatedAt = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ColorHex = category.ColorHex
            };
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría con ID {id}.");
            }

            await _categoryRepository.DeleteAsync(category);
        }
    }
}
