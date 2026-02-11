using StockPro.DTOs;

namespace StockPro.Interfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<AlertDto>> GetAlertsAsync(bool? unreadOnly);
        Task<int> GetUnreadCountAsync();
        Task MarkAsReadAsync(Guid id);
        Task MarkAllAsReadAsync();
        Task CheckAndGenerateAlertsAsync();
    }
}
