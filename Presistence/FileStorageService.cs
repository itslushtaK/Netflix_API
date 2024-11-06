using Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql.BackendMessages;

namespace Presistence
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _uploadFolderPath;

        public FileStorageService(IConfiguration configuration)
        {
            _uploadFolderPath = @"C:\Users\itslushtaK\Desktop\my-app\src\assets\images";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file '{filePath}': {ex.Message}");
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty.");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_uploadFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }
    }

}
