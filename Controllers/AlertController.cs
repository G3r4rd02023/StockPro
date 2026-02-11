using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertController> _logger;

        public AlertController(IAlertService alertService, ILogger<AlertController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las alertas (opcionalmente solo no leídas)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAlerts([FromQuery] bool? unreadOnly)
        {
            try
            {
                var alerts = await _alertService.GetAlertsAsync(unreadOnly);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo alertas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtener cantidad de alertas no leídas (para sidebar/badge)
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var count = await _alertService.GetUnreadCountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo conteo de alertas no leídas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Marcar una alerta como leída
        /// </summary>
        [HttpPatch("{id}/read")]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _alertService.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marcando alerta {Id} como leída", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Marcar todas las alertas como leídas
        /// </summary>
        [HttpPatch("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                await _alertService.MarkAllAsReadAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marcando todas las alertas como leídas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Ejecutar detección manual de alertas de bajo stock
        /// </summary>
        [HttpPost("check")]
        public async Task<ActionResult> CheckAlerts()
        {
            try
            {
                await _alertService.CheckAndGenerateAlertsAsync();
                return Ok(new { message = "Verificación de alertas completada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando alertas");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
