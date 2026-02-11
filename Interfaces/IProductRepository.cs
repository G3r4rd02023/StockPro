using StockPro.Data.Entities;

namespace StockPro.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string? search, Guid? categoryId, bool? lowStock);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<bool> ExistsBySkuAsync(string sku, Guid? exceptId = null);
        Task<int> CountAsync();
        Task<decimal> GetTotalStockValueAsync();
        Task<int> GetLowStockCountAsync();
    }
}
