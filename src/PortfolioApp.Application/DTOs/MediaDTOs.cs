namespace PortfolioApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new image.
/// This DTO contains all the required information for image creation.
/// </summary>
public class CreateImageDto
{
    /// <summary>
    /// ID of the project this image belongs to.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Display title for the image. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the image content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the image file from wwwroot.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the thumbnail version of the image.
    /// </summary>
    public string ThumbnailUrl { get; set; } = string.Empty;

    /// <summary>
    /// Alternative text for accessibility (screen readers).
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing image.
/// This DTO contains fields that can be updated for an image.
/// </summary>
public class UpdateImageDto
{
    /// <summary>
    /// Image's unique identifier. Required for updates.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display title for the image. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the image content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Alternative text for accessibility (screen readers).
    /// </summary>
    public string AltText { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for image information display.
/// This DTO contains all image information formatted for presentation.
/// </summary>
public class ImageDto
{
    /// <summary>
    /// Image's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the project this image belongs to.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Display title for the image.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the image content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the image file.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Alternative text for accessibility.
    /// </summary>
    public string AltText { get; set; } = string.Empty;

    /// <summary>
    /// Image width in pixels.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Image height in pixels.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Formatted file size string (e.g., "1.2 MB").
    /// </summary>
    public string FormattedFileSize { get; set; } = string.Empty;

    /// <summary>
    /// Image dimensions as a formatted string (e.g., "1920×1080").
    /// </summary>
    public string Dimensions { get; set; } = string.Empty;

    /// <summary>
    /// Image aspect ratio.
    /// </summary>
    public double AspectRatio { get; set; }

    /// <summary>
    /// Indicates if the image is in landscape orientation.
    /// </summary>
    public bool IsLandscape { get; set; }

    /// <summary>
    /// Indicates if the image is in portrait orientation.
    /// </summary>
    public bool IsPortrait { get; set; }

    /// <summary>
    /// Indicates if the image is square.
    /// </summary>
    public bool IsSquare { get; set; }

    /// <summary>
    /// File extension of the image.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the image was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for creating a new video.
/// This DTO contains all the required information for video creation.
/// </summary>
public class CreateVideoDto
{
    /// <summary>
    /// ID of the project this video belongs to.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Display title for the video. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the video content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video file from wwwroot.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video thumbnail image.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Video duration in seconds.
    /// </summary>
    public int DurationInSeconds { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing video.
/// This DTO contains fields that can be updated for a video.
/// </summary>
public class UpdateVideoDto
{
    /// <summary>
    /// Video's unique identifier. Required for updates.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display title for the video. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the video content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video thumbnail image.
    /// </summary>
    public string? ThumbnailUrl { get; set; }
}

/// <summary>
/// Data Transfer Object for video information display.
/// This DTO contains all video information formatted for presentation.
/// </summary>
public class VideoDto
{
    /// <summary>
    /// Video's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the project this video belongs to.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Display title for the video.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the video content.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video file.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video thumbnail image.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Video duration in seconds.
    /// </summary>
    public int DurationInSeconds { get; set; }

    /// <summary>
    /// Formatted duration string (e.g., "2:30", "1:05:30").
    /// </summary>
    public string FormattedDuration { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// Formatted file size string (e.g., "15.2 MB").
    /// </summary>
    public string FormattedFileSize { get; set; } = string.Empty;

    /// <summary>
    /// File extension of the video.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the video is considered short (less than 1 minute).
    /// </summary>
    public bool IsShortVideo { get; set; }

    /// <summary>
    /// Indicates if the video is considered long (more than 10 minutes).
    /// </summary>
    public bool IsLongVideo { get; set; }

    /// <summary>
    /// Approximate bitrate in bits per second.
    /// </summary>
    public long ApproximateBitrate { get; set; }

    /// <summary>
    /// Formatted bitrate string (e.g., "2.5 Mbps").
    /// </summary>
    public string FormattedBitrate { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the video has a thumbnail image.
    /// </summary>
    public bool HasThumbnail { get; set; }

    /// <summary>
    /// Timestamp when the video was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// Data Transfer Object for file upload operations.
/// This DTO contains information about uploaded files before they are processed.
/// </summary>
public class FileUploadDto
{
    /// <summary>
    /// Original filename of the uploaded file.
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Generated filename for storage.
    /// </summary>
    public string StorageFileName { get; set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the stored file.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long SizeInBytes { get; set; }

    /// <summary>
    /// MIME type of the file.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File extension.
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the file is an image.
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// Indicates if the file is a video.
    /// </summary>
    public bool IsVideo { get; set; }

    /// <summary>
    /// Image width in pixels (for images only).
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Image height in pixels (for images only).
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Video duration in seconds (for videos only).
    /// </summary>
    public int? DurationInSeconds { get; set; }

    /// <summary>
    /// URL to generated thumbnail (for videos).
    /// </summary>
    public string? ThumbnailUrl { get; set; }
}

/// <summary>
/// Data Transfer Object for media gallery display.
/// This DTO combines images and videos for gallery presentations.
/// </summary>
public class MediaGalleryDto
{
    /// <summary>
    /// Collection of images in the gallery.
    /// </summary>
    public IEnumerable<ImageDto> Images { get; set; } = new List<ImageDto>();

    /// <summary>
    /// Collection of videos in the gallery.
    /// </summary>
    public IEnumerable<VideoDto> Videos { get; set; } = new List<VideoDto>();

    /// <summary>
    /// Total number of media items.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of images.
    /// </summary>
    public int ImageCount { get; set; }

    /// <summary>
    /// Number of videos.
    /// </summary>
    public int VideoCount { get; set; }

    /// <summary>
    /// Total size of all media files in bytes.
    /// </summary>
    public long TotalSizeInBytes { get; set; }

    /// <summary>
    /// Formatted total size string.
    /// </summary>
    public string FormattedTotalSize { get; set; } = string.Empty;
}

