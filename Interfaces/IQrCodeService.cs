namespace StockPro.Interfaces
{
    public interface IQrCodeService
    {
        byte[] GenerateQrCode(string content);
        string GenerateQrCodeBase64(string content);
    }
}
