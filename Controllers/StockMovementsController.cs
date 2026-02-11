using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPro.Data.Enums;
using StockPro.DTOs;
using StockPro.Interfaces;
using System.Security.Claims;

namespace StockPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockMovementsController : ControllerBase
    {
        private readonly IStockMovementService _movementService;
        private readonly ILogger<StockMovementsController> _logger;

        public StockMovementsController(IStockMovementService movementService, ILogger<StockMovementsController> logger)
        {
            _movementService = movementService;
            _logger = logger;
        }

        /// <summary>
        /// HU-015: Consultar historial de movimientos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetMovements(
            [FromQuery] Guid? productId,
            [FromQuery] Guid? userId,
            [FromQuery] MovementType? type)
        {
            try
            {
                var movements = await _movementService.GetMovementsAsync(productId, userId, type);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo movimientos de stock");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockMovementDto>> GetMovement(Guid id)
        {
            try
            {
                var movement = await _movementService.GetMovementByIdAsync(id);
                if (movement == null)
                {
                    return NotFound($"Movimiento con ID {id} no encontrado");
                }
                return Ok(movement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo movimiento {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-013 y HU-014: Registrar entradas y salidas de stock
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StockMovementDto>> CreateMovement([FromBody] CreateStockMovementDto createMovementDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                // Preferir la claim cuyo valor sea un GUID
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var currentUserId))
                {
                    _logger.LogWarning("No se pudo identificar al usuario. Claims: {Claims}", string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
                    return Unauthorized("No se pudo identificar al usuario");
                }

                var movement = await _movementService.CreateMovementAsync(createMovementDto, currentUserId);

                _logger.LogInformation("Movimiento de stock creado: {Type} de {Quantity} unidades para producto {ProductId}",
                    createMovementDto.MovementType, createMovementDto.Quantity, createMovementDto.ProductId);

                return CreatedAtAction(nameof(GetMovement), new { id = movement.Id }, movement);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando movimiento de stock");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
