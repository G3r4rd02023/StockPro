using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPro.Interfaces;

namespace StockPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QrCodeController : ControllerBase
    {
        private readonly IQrCodeService _qrCodeService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<QrCodeController> _logger;

        public QrCodeController(
            IQrCodeService qrCodeService,
            IProductRepository productRepository,
            ILogger<QrCodeController> logger)
        {
            _qrCodeService = qrCodeService;
            _productRepository = productRepository;
            _logger = logger;
        }

        /// <summary>
        /// HU-017: Generar código QR para un producto (devuelve imagen PNG)
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GenerateQrForProduct(Guid productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return NotFound("Producto no encontrado");
                }

                // El QR contiene un JSON con la información esencial del producto
                var qrContent = $"{{\"id\":\"{product.Id}\",\"sku\":\"{product.SKU}\",\"name\":\"{product.Name}\"}}";
                var qrBytes = _qrCodeService.GenerateQrCode(qrContent);

                // Guardar referencia del QR en el producto si no tiene
                if (string.IsNullOrEmpty(product.QrCode))
                {
                    product.QrCode = _qrCodeService.GenerateQrCodeBase64(qrContent);
                    product.UpdatedAt = DateTime.UtcNow;
                    await _productRepository.UpdateAsync(product);
                }

                return File(qrBytes, "image/png", $"QR_{product.SKU}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando QR para producto {ProductId}", productId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-017: Generar código QR en base64 (para mostrar en pantalla)
        /// </summary>
        [HttpGet("product/{productId}/base64")]
        public async Task<ActionResult> GenerateQrBase64ForProduct(Guid productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return NotFound("Producto no encontrado");
                }

                var qrContent = $"{{\"id\":\"{product.Id}\",\"sku\":\"{product.SKU}\",\"name\":\"{product.Name}\"}}";
                var base64 = _qrCodeService.GenerateQrCodeBase64(qrContent);

                return Ok(new { qrBase64 = base64, productId = product.Id, sku = product.SKU });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando QR base64 para producto {ProductId}", productId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// HU-018: Buscar producto por contenido escaneado del QR (por SKU o ID)
        /// </summary>
        [HttpGet("scan")]
        public async Task<ActionResult> ScanQrCode([FromQuery] string? sku, [FromQuery] Guid? productId)
        {
            try
            {
                if (productId.HasValue)
                {
                    var product = await _productRepository.GetByIdAsync(productId.Value);
                    if (product == null)
                    {
                        return NotFound("Producto no encontrado con ese ID");
                    }
                    return Ok(new
                    {
                        product.Id,
                        product.Name,
                        product.SKU,
                        product.Price,
                        product.CurrentStock,
                        product.MinStockThreshold,
                        product.ImageUrl,
                        CategoryName = product.Category?.Name ?? "Sin categoría",
                        product.IsLowStock,
                        product.IsOutOfStock
                    });
                }

                if (!string.IsNullOrEmpty(sku))
                {
                    var products = await _productRepository.GetAllAsync(sku, null, null);
                    var product = products.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));
                    if (product == null)
                    {
                        return NotFound("Producto no encontrado con ese SKU");
                    }
                    return Ok(new
                    {
                        product.Id,
                        product.Name,
                        product.SKU,
                        product.Price,
                        product.CurrentStock,
                        product.MinStockThreshold,
                        product.ImageUrl,
                        CategoryName = product.Category?.Name ?? "Sin categoría",
                        product.IsLowStock,
                        product.IsOutOfStock
                    });
                }

                return BadRequest("Debe proporcionar un SKU o un ID de producto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando escaneo de QR");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
