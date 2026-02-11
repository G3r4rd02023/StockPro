using StockPro.Data.Entities;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;

        public ProductService(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository,
            IImageService imageService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string? search, Guid? categoryId, bool? lowStock)
        {
            var products = await _productRepository.GetAllAsync(search, categoryId, lowStock);
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            // Validar SKU único
            if (await _productRepository.ExistsBySkuAsync(createProductDto.SKU))
            {
                throw new InvalidOperationException($"El SKU '{createProductDto.SKU}' ya está registrado.");
            }

            // Validar Categoría
            var category = await _categoryRepository.GetByIdAsync(createProductDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("La categoría especificada no existe.");
            }

            string? imageUrl = null;
            if (createProductDto.ImageFile != null)
            {
                imageUrl = await _imageService.UploadImageAsync(createProductDto.ImageFile);
            }

            var product = new Product
            {
                Name = createProductDto.Name,
                SKU = createProductDto.SKU,
                CategoryId = createProductDto.CategoryId,
                Price = createProductDto.Price,
                CurrentStock = createProductDto.CurrentStock,
                MinStockThreshold = createProductDto.MinStockThreshold,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            
            // Recargar para incluir la entidad Category para el DTO
            var createdProduct = await _productRepository.GetByIdAsync(product.Id);
            return MapToDto(createdProduct!);
        }

        public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontró el producto con ID {id}.");
            }

            // Validar SKU único (excepto para sí mismo)
            if (await _productRepository.ExistsBySkuAsync(updateProductDto.SKU, id))
            {
                throw new InvalidOperationException($"El SKU '{updateProductDto.SKU}' ya está registrado por otro producto.");
            }

            // Validar Categoría
            var category = await _categoryRepository.GetByIdAsync(updateProductDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException("La categoría especificada no existe.");
            }

            if (updateProductDto.ImageFile != null)
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.ImageUrl);
                }
                
                // Subir nueva imagen
                product.ImageUrl = await _imageService.UploadImageAsync(updateProductDto.ImageFile);
            }

            product.Name = updateProductDto.Name;
            product.SKU = updateProductDto.SKU;
            product.CategoryId = updateProductDto.CategoryId;
            product.Price = updateProductDto.Price;
            product.CurrentStock = updateProductDto.CurrentStock;
            product.MinStockThreshold = updateProductDto.MinStockThreshold;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            
            return MapToDto(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"No se encontró el producto con ID {id}.");
            }

            // Eliminar imagen de Cloudinary
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                await _imageService.DeleteImageAsync(product.ImageUrl);
            }

            await _productRepository.DeleteAsync(product);
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                CategoryName = product?.Category?.Name ?? "Sin categoría",
                Price = product.Price,
                CurrentStock = product.CurrentStock,
                MinStockThreshold = product.MinStockThreshold,
                ImageUrl = product.ImageUrl,
                IsLowStock = product.IsLowStock,
                IsOutOfStock = product.IsOutOfStock
            };
        }
    }
}
