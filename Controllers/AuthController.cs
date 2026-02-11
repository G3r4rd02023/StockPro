using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPro.DTOs;
using StockPro.Interfaces;

namespace StockPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// HU-001: Registrar un nuevo usuario con email y contraseña
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("Usuario registrado exitosamente: {Email}", registerDto.Email);
                
                return Ok(new
                {
                    success = true,
                    message = "Usuario registrado exitosamente",
                    data = response
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error al registrar usuario: {Message}", ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al registrar usuario");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// HU-002: Iniciar sesión con credenciales
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("Usuario inició sesión exitosamente: {Email}", loginDto.Email);
                
                return Ok(new
                {
                    success = true,
                    message = "Inicio de sesión exitoso",
                    data = response
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Intento de inicio de sesión fallido: {Email}", loginDto.Email);
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al iniciar sesión");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// HU-003: Cerrar sesión (client-side token removal)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                // En una implementación con JWT, el logout se maneja principalmente del lado del cliente
                // eliminando el token. Aquí simplemente confirmamos la acción.
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                _logger.LogInformation("Usuario cerró sesión: {Email}", userEmail);
                
                return Ok(new
                {
                    success = true,
                    message = "Sesión cerrada exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al cerrar sesión");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}
