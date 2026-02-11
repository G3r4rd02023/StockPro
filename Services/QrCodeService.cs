using QRCoder;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class QrCodeService : IQrCodeService
    {
        public byte[] GenerateQrCode(string content)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(10);
        }

        public string GenerateQrCodeBase64(string content)
        {
            var qrBytes = GenerateQrCode(content);
            return Convert.ToBase64String(qrBytes);
        }
    }
}
