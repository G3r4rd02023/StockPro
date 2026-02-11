using StockPro.DTOs;

namespace StockPro.Interfaces
{
    public interface IReportService
    {
        Task<InventoryReportDto> GetInventoryReportAsync();
        Task<MovementReportDto> GetMovementReportAsync(DateTime? startDate, DateTime? endDate);
        Task<byte[]> ExportInventoryToCsvAsync();
        Task<byte[]> ExportMovementsToCsvAsync(DateTime? startDate, DateTime? endDate);
    }
}
