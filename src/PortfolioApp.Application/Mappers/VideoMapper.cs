using PortfolioApp.Application.DTOs;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Application.Mappers;

/// <summary>
/// Provides manual mapping functionalities between Video entities and Video DTOs.
/// This mapper ensures a clear separation of concerns and avoids direct exposure
/// of domain entities to the presentation layer, adhering to Clean Architecture principles.
/// </summary>
public static class VideoMapper
{
    /// <summary>
    /// Maps a Video entity to a VideoDto.
    /// This method transforms the domain model into a presentation-friendly DTO.
    /// </summary>
    /// <param name="video">The Video entity to map.</param>
    /// <returns>A new VideoDto instance.</returns>
    public static VideoDto ToDto(Video video)
    {
        if (video == null)
        {
            throw new ArgumentNullException(nameof(video));
        }

        return new VideoDto
        {
            Id = video.Id,
            ProjectId = video.ProjectId,
            Title = video.Title,
            Description = video.Description,
            Url = video.Url,
            ThumbnailUrl = video.ThumbnailUrl,
            DurationInSeconds = video.DurationInSeconds,
            FormattedDuration = FormatDuration(video.DurationInSeconds),
            SizeInBytes = video.SizeInBytes,
            FormattedFileSize = FormatFileSize(video.SizeInBytes),
            FileExtension = GetFileExtension(video.Url),
            IsShortVideo = video.DurationInSeconds < 60,
            IsLongVideo = video.DurationInSeconds > 600, // More than 10 minutes
            ApproximateBitrate = CalculateApproximateBitrate(video.SizeInBytes, video.DurationInSeconds),
            FormattedBitrate = FormatBitrate(CalculateApproximateBitrate(video.SizeInBytes, video.DurationInSeconds)),
            HasThumbnail = !string.IsNullOrWhiteSpace(video.ThumbnailUrl),
            UploadedAt = video.UploadedAt
        };
    }

    /// <summary>
    /// Maps a CreateVideoDto to a Video entity.
    /// This method transforms a DTO from the presentation layer into a domain entity.
    /// </summary>
    /// <param name="dto">The CreateVideoDto to map.</param>
    /// <returns>A new Video entity instance.</returns>
    public static Video ToEntity(CreateVideoDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        return new Video(
            dto.ProjectId,
            dto.Title,
            dto.Description,
            dto.Url,
            dto.ThumbnailUrl,
            dto.DurationInSeconds,
            dto.SizeInBytes
        );
    }

    /// <summary>
    /// Updates an existing Video entity from an UpdateVideoDto.
    /// This method modifies an existing domain entity based on the DTO.
    /// </summary>
    /// <param name="dto">The UpdateVideoDto containing updated information.</param>
    /// <param name="video">The existing Video entity to update.</param>
    public static void ToEntity(UpdateVideoDto dto, Video video)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        if (video == null)
        {
            throw new ArgumentNullException(nameof(video));
        }

        video.UpdateMetadata(
            dto.Title,
            dto.Description
        );
        
        if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
        {
            video.UpdateThumbnail(dto.ThumbnailUrl);
        }
    }

    /// <summary>
    /// Maps a collection of Video entities to VideoDto collection.
    /// </summary>
    /// <param name="videos">Collection of Video entities to map.</param>
    /// <returns>Collection of VideoDto instances.</returns>
    public static IEnumerable<VideoDto> ToDtoCollection(IEnumerable<Video> videos)
    {
        if (videos == null)
        {
            throw new ArgumentNullException(nameof(videos));
        }

        return videos.Select(ToDto).ToList();
    }

    /// <summary>
    /// Maps a FileUploadDto to a CreateVideoDto.
    /// This method transforms upload information into a creation DTO.
    /// </summary>
    /// <param name="fileUpload">The FileUploadDto containing upload information.</param>
    /// <param name="projectId">The ID of the project this video belongs to.</param>
    /// <param name="title">The title for the video.</param>
    /// <param name="description">The description for the video.</param>
    /// <returns>A new CreateVideoDto instance.</returns>
    public static CreateVideoDto FromFileUpload(FileUploadDto fileUpload, int projectId, string title, string description = "")
    {
        if (fileUpload == null)
        {
            throw new ArgumentNullException(nameof(fileUpload));
        }

        if (!fileUpload.IsVideo)
        {
            throw new ArgumentException("FileUploadDto must represent a video file.", nameof(fileUpload));
        }

        return new CreateVideoDto
        {
            ProjectId = projectId,
            Title = title,
            Description = description,
            Url = fileUpload.Url,
            ThumbnailUrl = fileUpload.ThumbnailUrl,
            DurationInSeconds = fileUpload.DurationInSeconds ?? 0,
            SizeInBytes = fileUpload.SizeInBytes
        };
    }

    /// <summary>
    /// Formats a duration in seconds to a human-readable string (e.g., "2:30", "1:05:30").
    /// </summary>
    /// <param name="durationInSeconds">The duration in seconds.</param>
    /// <returns>A formatted duration string.</returns>
    private static string FormatDuration(int durationInSeconds)
    {
        if (durationInSeconds < 0)
        {
            return "N/A";
        }

        TimeSpan time = TimeSpan.FromSeconds(durationInSeconds);
        if (time.TotalHours >= 1)
        {
            return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }

    /// <summary>
    /// Formats a file size in bytes to a human-readable string.
    /// </summary>
    /// <param name="sizeInBytes">The file size in bytes.</param>
    /// <returns>A formatted file size string (e.g., "1.2 MB").</returns>
    private static string FormatFileSize(long sizeInBytes)
    {
        if (sizeInBytes == 0)
        {
            return "0 B";
        }

        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = sizeInBytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Extracts the file extension from a URL.
    /// </summary>
    /// <param name="url">The URL to extract the extension from.</param>
    /// <returns>The file extension (e.g., ".mp4").</returns>
    private static string GetFileExtension(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        try
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            var path = uri.IsAbsoluteUri ? uri.LocalPath : url;
            return Path.GetExtension(path).ToLowerInvariant();
        }
        catch
        {
            // If URL parsing fails, try to extract extension from the string directly
            var lastDotIndex = url.LastIndexOf(".");
            if (lastDotIndex >= 0 && lastDotIndex < url.Length - 1)
            {
                var extension = url.Substring(lastDotIndex);
                // Remove query parameters if present
                var queryIndex = extension.IndexOf("?");
                if (queryIndex >= 0)
                {
                    extension = extension.Substring(0, queryIndex);
                }
                return extension.ToLowerInvariant();
            }
            return string.Empty;
        }
    }

    /// <summary>
    /// Calculates approximate bitrate in bits per second.
    /// </summary>
    /// <param name="sizeInBytes">File size in bytes.</param>
    /// <param name="durationInSeconds">Duration in seconds.</param>
    /// <returns>Approximate bitrate in bps.</returns>
    private static long CalculateApproximateBitrate(long sizeInBytes, int durationInSeconds)
    {
        if (durationInSeconds <= 0)
        {
            return 0;
        }
        // Convert bytes to bits (1 byte = 8 bits)
        long sizeInBits = sizeInBytes * 8;
        return sizeInBits / durationInSeconds;
    }

    /// <summary>
    /// Formats bitrate to a human-readable string (e.g., "2.5 Mbps").
    /// </summary>
    /// <param name="bitrateBps">Bitrate in bits per second.</param>
    /// <returns>Formatted bitrate string.</returns>
    private static string FormatBitrate(long bitrateBps)
    {
        if (bitrateBps == 0)
        {
            return "0 bps";
        }

        string[] units = { "bps", "Kbps", "Mbps", "Gbps" };
        int unitIndex = 0;
        double bitrate = bitrateBps;

        while (bitrate >= 1000 && unitIndex < units.Length - 1)
        {
            bitrate /= 1000;
            unitIndex++;
        }

        return $"{bitrate:0.##} {units[unitIndex]}";
    }
}

