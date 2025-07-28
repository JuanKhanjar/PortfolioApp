namespace PortfolioApp.Domain.Entities;

/// <summary>
/// Represents a video file associated with a project.
/// This entity manages video metadata and maintains relationships with projects.
/// The actual video file is stored in the file system, while this entity stores metadata.
/// </summary>
public class Video
{
    /// <summary>
    /// Unique identifier for the video. Primary key.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Foreign key reference to the Project this video belongs to.
    /// </summary>
    public int ProjectId { get; private set; }

    /// <summary>
    /// Display title for the video. Used in galleries and captions.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Detailed description of the video content.
    /// Used for context and accessibility.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video file from wwwroot.
    /// Example: "/videos/projects/project1/demo.mp4"
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the video thumbnail image.
    /// Used for video preview before playback.
    /// </summary>
    public string? ThumbnailUrl { get; private set; }

    /// <summary>
    /// Video duration in seconds. Used for display and user experience.
    /// </summary>
    public int DurationInSeconds { get; private set; }

    /// <summary>
    /// File size in bytes. Used for storage management and display.
    /// </summary>
    public long SizeInBytes { get; private set; }

    /// <summary>
    /// Timestamp when the video was uploaded. Audit field.
    /// </summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Navigation property to the Project this video belongs to.
    /// </summary>
    public virtual Project Project { get; private set; } = null!;

    /// <summary>
    /// Private constructor for Entity Framework.
    /// </summary>
    private Video() { }

    /// <summary>
    /// Creates a new Video instance with required fields.
    /// This constructor enforces business rules and validates input.
    /// </summary>
    /// <param name="projectId">ID of the project this video belongs to</param>
    /// <param name="title">Display title for the video</param>
    /// <param name="description">Description of the video</param>
    /// <param name="url">Relative URL to the video file</param>
    /// <param name="thumbnailUrl">Relative URL to the thumbnail image (optional)</param>
    /// <param name="durationInSeconds">Video duration in seconds</param>
    /// <param name="sizeInBytes">File size in bytes</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid</exception>
    public Video(int projectId, string title, string description, string url, 
                string? thumbnailUrl, int durationInSeconds, long sizeInBytes)
    {
        if (projectId <= 0)
            throw new ArgumentException("Project ID must be positive", nameof(projectId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Video title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Video URL cannot be empty", nameof(url));

        if (durationInSeconds < 0)
            throw new ArgumentException("Video duration cannot be negative", nameof(durationInSeconds));

        if (sizeInBytes <= 0)
            throw new ArgumentException("Video size must be positive", nameof(sizeInBytes));

        ProjectId = projectId;
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Url = url.Trim();
        ThumbnailUrl = string.IsNullOrWhiteSpace(thumbnailUrl) ? null : thumbnailUrl.Trim();
        DurationInSeconds = durationInSeconds;
        SizeInBytes = sizeInBytes;
        UploadedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the video metadata.
    /// This method allows updating display information without changing the file.
    /// </summary>
    /// <param name="title">New video title</param>
    /// <param name="description">New video description</param>
    public void UpdateMetadata(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Video title cannot be empty", nameof(title));

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Updates the video file information.
    /// This method is used when the physical file is replaced.
    /// </summary>
    /// <param name="url">New URL to the video file</param>
    /// <param name="durationInSeconds">New video duration</param>
    /// <param name="sizeInBytes">New file size</param>
    public void UpdateFileInfo(string url, int durationInSeconds, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Video URL cannot be empty", nameof(url));

        if (durationInSeconds < 0)
            throw new ArgumentException("Video duration cannot be negative", nameof(durationInSeconds));

        if (sizeInBytes <= 0)
            throw new ArgumentException("Video size must be positive", nameof(sizeInBytes));

        Url = url.Trim();
        DurationInSeconds = durationInSeconds;
        SizeInBytes = sizeInBytes;
        UploadedAt = DateTime.UtcNow; // Update timestamp when file changes
    }

    /// <summary>
    /// Updates the video thumbnail URL.
    /// </summary>
    /// <param name="thumbnailUrl">New thumbnail URL</param>
    public void UpdateThumbnail(string? thumbnailUrl)
    {
        ThumbnailUrl = string.IsNullOrWhiteSpace(thumbnailUrl) ? null : thumbnailUrl.Trim();
    }

    /// <summary>
    /// Gets the video duration in a human-readable format.
    /// </summary>
    /// <returns>Formatted duration string (e.g., "2:30", "1:05:30")</returns>
    public string GetFormattedDuration()
    {
        var timeSpan = TimeSpan.FromSeconds(DurationInSeconds);
        
        if (timeSpan.TotalHours >= 1)
            return timeSpan.ToString(@"h\:mm\:ss");
        
        return timeSpan.ToString(@"m\:ss");
    }

    /// <summary>
    /// Gets the file size in a human-readable format.
    /// </summary>
    /// <returns>Formatted file size string (e.g., "15.2 MB")</returns>
    public string GetFormattedFileSize()
    {
        const long KB = 1024;
        const long MB = KB * 1024;
        const long GB = MB * 1024;

        if (SizeInBytes >= GB)
            return $"{SizeInBytes / (double)GB:F1} GB";
        
        if (SizeInBytes >= MB)
            return $"{SizeInBytes / (double)MB:F1} MB";
        
        if (SizeInBytes >= KB)
            return $"{SizeInBytes / (double)KB:F1} KB";
        
        return $"{SizeInBytes} bytes";
    }

    /// <summary>
    /// Gets the file extension from the URL.
    /// Useful for determining video format.
    /// </summary>
    /// <returns>File extension including the dot (e.g., ".mp4")</returns>
    public string GetFileExtension()
    {
        var lastDotIndex = Url.LastIndexOf('.');
        return lastDotIndex >= 0 ? Url.Substring(lastDotIndex) : string.Empty;
    }

    /// <summary>
    /// Determines if the video is considered short (less than 1 minute).
    /// </summary>
    /// <returns>True if short video, false otherwise</returns>
    public bool IsShortVideo()
    {
        return DurationInSeconds < 60;
    }

    /// <summary>
    /// Determines if the video is considered long (more than 10 minutes).
    /// </summary>
    /// <returns>True if long video, false otherwise</returns>
    public bool IsLongVideo()
    {
        return DurationInSeconds > 600; // 10 minutes
    }

    /// <summary>
    /// Validates that the video meets size and duration constraints.
    /// This can be used for business rule validation.
    /// </summary>
    /// <param name="maxSizeInBytes">Maximum allowed file size</param>
    /// <param name="maxDurationInSeconds">Maximum allowed duration</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidSize(long maxSizeInBytes, int maxDurationInSeconds)
    {
        return SizeInBytes <= maxSizeInBytes && DurationInSeconds <= maxDurationInSeconds;
    }

    /// <summary>
    /// Gets the video bitrate in bits per second.
    /// This is an approximation based on file size and duration.
    /// </summary>
    /// <returns>Approximate bitrate in bits per second</returns>
    public long GetApproximateBitrate()
    {
        if (DurationInSeconds <= 0)
            return 0;

        return (SizeInBytes * 8) / DurationInSeconds; // Convert bytes to bits and divide by duration
    }

    /// <summary>
    /// Gets the video bitrate in a human-readable format.
    /// </summary>
    /// <returns>Formatted bitrate string (e.g., "2.5 Mbps")</returns>
    public string GetFormattedBitrate()
    {
        var bitrate = GetApproximateBitrate();
        
        const long Kbps = 1000;
        const long Mbps = Kbps * 1000;
        const long Gbps = Mbps * 1000;

        if (bitrate >= Gbps)
            return $"{bitrate / (double)Gbps:F1} Gbps";
        
        if (bitrate >= Mbps)
            return $"{bitrate / (double)Mbps:F1} Mbps";
        
        if (bitrate >= Kbps)
            return $"{bitrate / (double)Kbps:F1} Kbps";
        
        return $"{bitrate} bps";
    }

    /// <summary>
    /// Determines if the video has a thumbnail image.
    /// </summary>
    /// <returns>True if thumbnail exists, false otherwise</returns>
    public bool HasThumbnail()
    {
        return !string.IsNullOrWhiteSpace(ThumbnailUrl);
    }
}

