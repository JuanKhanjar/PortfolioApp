namespace PortfolioApp.Domain.Entities;

/// <summary>
/// Represents an image file associated with a project.
/// This entity manages image metadata and maintains relationships with projects.
/// The actual image file is stored in the file system, while this entity stores metadata.
/// </summary>
public class Image
{
    /// <summary>
    /// Unique identifier for the image. Primary key.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Foreign key reference to the Project this image belongs to.
    /// </summary>
    public int ProjectId { get; private set; }

    /// <summary>
    /// Display title for the image. Used in galleries and captions.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Detailed description of the image content.
    /// Used for context and accessibility.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Relative URL path to the image file from wwwroot.
    /// Example: "/images/projects/project1/screenshot1.jpg"
    /// </summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>
    /// Alternative text for accessibility (screen readers).
    /// Important for web accessibility compliance.
    /// </summary>
    public string AltText { get; private set; } = string.Empty;

    /// <summary>
    /// Image width in pixels. Used for responsive display and layout.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Image height in pixels. Used for responsive display and layout.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// File size in bytes. Used for storage management and display.
    /// </summary>
    public long SizeInBytes { get; private set; }

    /// <summary>
    /// Timestamp when the image was uploaded. Audit field.
    /// </summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// Navigation property to the Project this image belongs to.
    /// </summary>
    public virtual Project Project { get; private set; } = null!;

    /// <summary>
    /// Private constructor for Entity Framework.
    /// </summary>
    private Image() { }

    /// <summary>
    /// Creates a new Image instance with required fields.
    /// This constructor enforces business rules and validates input.
    /// </summary>
    /// <param name="projectId">ID of the project this image belongs to</param>
    /// <param name="title">Display title for the image</param>
    /// <param name="description">Description of the image</param>
    /// <param name="url">Relative URL to the image file</param>
    /// <param name="altText">Alternative text for accessibility</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="sizeInBytes">File size in bytes</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid</exception>
    public Image(int projectId, string title, string description, string url, string altText, 
                int width, int height, long sizeInBytes)
    {
        if (projectId <= 0)
            throw new ArgumentException("Project ID must be positive", nameof(projectId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Image title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be empty", nameof(url));

        if (width <= 0)
            throw new ArgumentException("Image width must be positive", nameof(width));

        if (height <= 0)
            throw new ArgumentException("Image height must be positive", nameof(height));

        if (sizeInBytes <= 0)
            throw new ArgumentException("Image size must be positive", nameof(sizeInBytes));

        ProjectId = projectId;
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        Url = url.Trim();
        AltText = altText?.Trim() ?? string.Empty;
        Width = width;
        Height = height;
        SizeInBytes = sizeInBytes;
        UploadedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the image metadata.
    /// This method allows updating display information without changing the file.
    /// </summary>
    /// <param name="title">New image title</param>
    /// <param name="description">New image description</param>
    /// <param name="altText">New alternative text</param>
    public void UpdateMetadata(string title, string description, string altText)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Image title cannot be empty", nameof(title));

        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
        AltText = altText?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Updates the image file information.
    /// This method is used when the physical file is replaced.
    /// </summary>
    /// <param name="url">New URL to the image file</param>
    /// <param name="width">New image width</param>
    /// <param name="height">New image height</param>
    /// <param name="sizeInBytes">New file size</param>
    public void UpdateFileInfo(string url, int width, int height, long sizeInBytes)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be empty", nameof(url));

        if (width <= 0)
            throw new ArgumentException("Image width must be positive", nameof(width));

        if (height <= 0)
            throw new ArgumentException("Image height must be positive", nameof(height));

        if (sizeInBytes <= 0)
            throw new ArgumentException("Image size must be positive", nameof(sizeInBytes));

        Url = url.Trim();
        Width = width;
        Height = height;
        SizeInBytes = sizeInBytes;
        UploadedAt = DateTime.UtcNow; // Update timestamp when file changes
    }

    /// <summary>
    /// Gets the aspect ratio of the image.
    /// Useful for responsive layout calculations.
    /// </summary>
    /// <returns>Aspect ratio as width/height</returns>
    public double GetAspectRatio()
    {
        return Height > 0 ? (double)Width / Height : 1.0;
    }

    /// <summary>
    /// Gets the file size in a human-readable format.
    /// </summary>
    /// <returns>Formatted file size string (e.g., "1.2 MB")</returns>
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
    /// Gets the image dimensions as a formatted string.
    /// </summary>
    /// <returns>Dimensions string (e.g., "1920×1080")</returns>
    public string GetDimensionsString()
    {
        return $"{Width}×{Height}";
    }

    /// <summary>
    /// Determines if the image is in landscape orientation.
    /// </summary>
    /// <returns>True if landscape, false otherwise</returns>
    public bool IsLandscape()
    {
        return Width > Height;
    }

    /// <summary>
    /// Determines if the image is in portrait orientation.
    /// </summary>
    /// <returns>True if portrait, false otherwise</returns>
    public bool IsPortrait()
    {
        return Height > Width;
    }

    /// <summary>
    /// Determines if the image is square.
    /// </summary>
    /// <returns>True if square, false otherwise</returns>
    public bool IsSquare()
    {
        return Width == Height;
    }

    /// <summary>
    /// Gets the file extension from the URL.
    /// Useful for determining image format.
    /// </summary>
    /// <returns>File extension including the dot (e.g., ".jpg")</returns>
    public string GetFileExtension()
    {
        var lastDotIndex = Url.LastIndexOf('.');
        return lastDotIndex >= 0 ? Url.Substring(lastDotIndex) : string.Empty;
    }

    /// <summary>
    /// Validates that the image meets size constraints.
    /// This can be used for business rule validation.
    /// </summary>
    /// <param name="maxSizeInBytes">Maximum allowed file size</param>
    /// <param name="maxWidth">Maximum allowed width</param>
    /// <param name="maxHeight">Maximum allowed height</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValidSize(long maxSizeInBytes, int maxWidth, int maxHeight)
    {
        return SizeInBytes <= maxSizeInBytes && Width <= maxWidth && Height <= maxHeight;
    }
}

