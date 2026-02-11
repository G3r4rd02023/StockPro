using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// HU-019: Reporte de inventario con métricas y detalle de productos
        /// </summary>
        [HttpGet("inventory")]
        public async Task<ActionResult<InventoryReportDto>> GetInventoryReport()
        {
            try
            {
                var report = await _reportService.GetInventoryReportAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de inventario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-019: Reporte de movimientos con filtros de fecha
        /// </summary>
        [HttpGet("movements")]
        public async Task<ActionResult<MovementReportDto>> GetMovementReport(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var report = await _reportService.GetMovementReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de movimientos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-020: Exportar inventario a CSV
        /// </summary>
        [HttpGet("inventory/csv")]
        public async Task<IActionResult> ExportInventoryCsv()
        {
            try
            {
                var csvBytes = await _reportService.ExportInventoryToCsvAsync();
                return File(csvBytes, "text/csv", $"Inventario_{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando inventario a CSV");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-020: Exportar movimientos a CSV
        /// </summary>
        [HttpGet("movements/csv")]
        public async Task<IActionResult> ExportMovementsCsv(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var csvBytes = await _reportService.ExportMovementsToCsvAsync(startDate, endDate);
                return File(csvBytes, "text/csv", $"Movimientos_{DateTime.UtcNow:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exportando movimientos a CSV");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
