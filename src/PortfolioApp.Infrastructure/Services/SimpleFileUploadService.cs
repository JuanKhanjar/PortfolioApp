using PortfolioApp.Application.DTOs;

namespace PortfolioApp.Infrastructure.Services;

/// <summary>
/// Interface for file upload service operations.
/// </summary>
public interface IFileUploadService
{
    Task<FileUploadDto> UploadImageAsync(object file, CancellationToken cancellationToken = default);
    Task<FileUploadDto> UploadVideoAsync(object file, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default);
    bool ValidateImageFile(object file);
    bool ValidateVideoFile(object file);
    IEnumerable<string> GetAllowedImageExtensions();
    IEnumerable<string> GetAllowedVideoExtensions();
    long GetMaxImageSize();
    long GetMaxVideoSize();
}

/// <summary>
/// Simplified file upload service for demonstration purposes.
/// In a real application, this would handle actual file uploads with validation.
/// </summary>
public class SimpleFileUploadService : IFileUploadService
{
    private readonly string _uploadsPath;

    public SimpleFileUploadService()
    {
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(_uploadsPath);
        Directory.CreateDirectory(Path.Combine(_uploadsPath, "images"));
        Directory.CreateDirectory(Path.Combine(_uploadsPath, "videos"));
        Directory.CreateDirectory(Path.Combine(_uploadsPath, "thumbnails"));
    }

    public Task<FileUploadDto> UploadImageAsync(object file, CancellationToken cancellationToken = default)
    {
        // Simplified implementation for demonstration
        var result = new FileUploadDto
        {
            OriginalFileName = "sample-image.jpg",
            StorageFileName = $"{Guid.NewGuid()}.jpg",
            Url = "/uploads/images/sample-image.jpg",
            SizeInBytes = 1024000,
            ContentType = "image/jpeg",
            FileExtension = ".jpg",
            IsImage = true,
            IsVideo = false,
            Width = 800,
            Height = 600
        };

        return Task.FromResult(result);
    }

    public Task<FileUploadDto> UploadVideoAsync(object file, CancellationToken cancellationToken = default)
    {
        // Simplified implementation for demonstration
        var result = new FileUploadDto
        {
            OriginalFileName = "sample-video.mp4",
            StorageFileName = $"{Guid.NewGuid()}.mp4",
            Url = "/uploads/videos/sample-video.mp4",
            SizeInBytes = 10240000,
            ContentType = "video/mp4",
            FileExtension = ".mp4",
            IsImage = false,
            IsVideo = true,
            DurationInSeconds = 120
        };

        return Task.FromResult(result);
    }

    public Task<bool> DeleteFileAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public bool ValidateImageFile(object file)
    {
        return true; // Simplified validation
    }

    public bool ValidateVideoFile(object file)
    {
        return true; // Simplified validation
    }

    public IEnumerable<string> GetAllowedImageExtensions()
    {
        return new[] { ".jpg", ".jpeg", ".png", ".gif" };
    }

    public IEnumerable<string> GetAllowedVideoExtensions()
    {
        return new[] { ".mp4", ".avi", ".mov" };
    }

    public long GetMaxImageSize()
    {
        return 50 * 1024 * 1024; // 50 MB
    }

    public long GetMaxVideoSize()
    {
        return 2L * 1024 * 1024 * 1024; // 2 GB
    }
}

