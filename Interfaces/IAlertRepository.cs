using StockPro.Data.Entities;

namespace StockPro.Interfaces
{
    public interface IAlertRepository
    {
        Task<IEnumerable<Alert>> GetAllAsync(bool? unreadOnly);
        Task<Alert?> GetByIdAsync(Guid id);
        Task<Alert> AddAsync(Alert alert);
        Task UpdateAsync(Alert alert);
        Task<int> GetUnreadCountAsync();
        Task<bool> ExistsActiveAlertForProductAsync(Guid productId);
    }
}
