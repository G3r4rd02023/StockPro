using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using StockPro.Interfaces;

namespace StockPro.Services
{
    public class CloudinaryService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var acc = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "stockpro_products",
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUrl.ToString();
            }

            throw new ArgumentException("Archivo vacío");
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return false;

            // Extraer publicId de la URL
            var uri = new Uri(imageUrl);
            var publicId = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
            
            // Cloudinary suele incluir carpetas en el publicId si se subió a una
            // Para simplificar, buscamos si tiene el prefijo de la carpeta
            if (imageUrl.Contains("stockpro_products"))
            {
                publicId = "stockpro_products/" + publicId;
            }

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            return result.Result == "ok";
        }
    }
}
