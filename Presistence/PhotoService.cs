using Application.Common.Abstractions;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Domain;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(IOptions<CloudinarySettings> config, ILogger<PhotoService> logger)
    {
        _logger = logger;

        // Ensure that the CloudinarySettings configuration is bound correctly
        if (config?.Value == null || string.IsNullOrWhiteSpace(config.Value.CloudName) || string.IsNullOrWhiteSpace(config.Value.ApiKey) || string.IsNullOrWhiteSpace(config.Value.ApiSecret))
        {
            throw new ArgumentNullException(nameof(config), "Cloudinary settings cannot be null or empty");
        }

        var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);

    }
    public async Task<ImageUploadResult> AddPhoto(IFormFile file)
    {
        if (file.Length == 0)
        {
            return null;
        }

        var uploadResult = new ImageUploadResult();

        using (var stream = file.OpenReadStream())
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public Task<DeletionResult> DeletePhoto(string publicId)
    {
        throw new NotImplementedException();
    }

    public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentNullException(nameof(file), "Video file is null or empty");
        }

        VideoUploadResult uploadResult;

        using (var stream = file.OpenReadStream())
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }


    //public async Task<DeletionResult> DeletePhoto(string publicId)
    //{
    //    var deleteParams = new DeletionParams(publicId);
    //    var result = await _cloudinary.DestroyAsync(deleteParams);

    //    return result;
    //} 
}


