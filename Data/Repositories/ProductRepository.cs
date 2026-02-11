using Microsoft.EntityFrameworkCore;
using StockPro.Data.Entities;
using StockPro.Interfaces;

namespace StockPro.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(string? search, Guid? categoryId, bool? lowStock)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.SKU.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (lowStock.HasValue && lowStock.Value)
            {
                query = query.Where(p => p.CurrentStock <= p.MinStockThreshold);
            }

            return await query.ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsBySkuAsync(string sku, Guid? exceptId = null)
        {
            var query = _context.Products.AsQueryable();
            
            if (exceptId.HasValue)
            {
                query = query.Where(p => p.Id != exceptId.Value);
            }

            return await query.AnyAsync(p => p.SKU.ToLower() == sku.ToLower());
        }

        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<decimal> GetTotalStockValueAsync()
        {
            return await _context.Products.SumAsync(p => p.CurrentStock * p.Price);
        }

        public async Task<int> GetLowStockCountAsync()
        {
            return await _context.Products.CountAsync(p => p.CurrentStock <= p.MinStockThreshold);
        }
    }
}
