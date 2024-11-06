using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Abstractions
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhoto(IFormFile file);
        Task<DeletionResult> DeletePhoto(string publicId);
        Task<VideoUploadResult> UploadVideoAsync(IFormFile file);

    }
}
