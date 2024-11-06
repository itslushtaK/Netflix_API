using Microsoft.AspNetCore.Http;

namespace Application.Common.Abstractions
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string filePath);
    }
}
