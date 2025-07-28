namespace PortfolioApp.Domain.Entities;

/// <summary>
/// Represents a portfolio project entity.
/// This entity encapsulates all information about a specific project including
/// its metadata, relationships, and business rules for project management.
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project. Primary key.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Foreign key reference to the User who owns this project.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Project title. Required field with length validation.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Detailed project description. Supports rich text content.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of technologies used in the project.
    /// This could be enhanced with a separate Technology entity in the future.
    /// </summary>
    public string TechnologiesUsed { get; private set; } = string.Empty;

    /// <summary>
    /// URL to the live project or repository. Optional field.
    /// </summary>
    public string? ProjectUrl { get; private set; }

    /// <summary>
    /// Project start date. Used for timeline display and sorting.
    /// </summary>
    public DateTime? StartDate { get; private set; }

    /// <summary>
    /// Project end date. Null indicates ongoing project.
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Indicates whether this project should be featured prominently.
    /// Used for homepage display and filtering.
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Timestamp when the project record was created. Audit field.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Timestamp when the project record was last updated. Audit field.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Navigation property to the User who owns this project.
    /// </summary>
    public virtual User User { get; private set; } = null!;

    /// <summary>
    /// Collection of images associated with this project.
    /// Navigation property for Entity Framework.
    /// </summary>
    public virtual ICollection<Image> Images { get; private set; } = new List<Image>();

    /// <summary>
    /// Collection of videos associated with this project.
    /// Navigation property for Entity Framework.
    /// </summary>
    public virtual ICollection<Video> Videos { get; private set; } = new List<Video>();

    /// <summary>
    /// Private constructor for Entity Framework.
    /// </summary>
    private Project() { }

    /// <summary>
    /// Creates a new Project instance with required fields.
    /// This constructor enforces business rules and validates input.
    /// </summary>
    /// <param name="userId">ID of the user who owns this project</param>
    /// <param name="title">Project title (required)</param>
    /// <param name="description">Project description (required)</param>
    /// <param name="technologiesUsed">Technologies used in the project</param>
    /// <param name="projectUrl">URL to the project (optional)</param>
    /// <param name="startDate">Project start date (optional)</param>
    /// <param name="endDate">Project end date (optional)</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid</exception>
    public Project(int userId, string title, string description, string technologiesUsed = "", 
                  string? projectUrl = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project description cannot be empty", nameof(description));

        if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        UserId = userId;
        Title = title.Trim();
        Description = description.Trim();
        TechnologiesUsed = technologiesUsed?.Trim() ?? string.Empty;
        ProjectUrl = string.IsNullOrWhiteSpace(projectUrl) ? null : projectUrl.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsFeatured = false; // Default to not featured

        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Updates the project's basic information.
    /// This method maintains business rules and updates the audit timestamp.
    /// </summary>
    /// <param name="title">New project title</param>
    /// <param name="description">New project description</param>
    /// <param name="technologiesUsed">New technologies used</param>
    /// <param name="projectUrl">New project URL</param>
    /// <param name="startDate">New start date</param>
    /// <param name="endDate">New end date</param>
    public void UpdateDetails(string title, string description, string technologiesUsed, 
                             string? projectUrl, DateTime? startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Project title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Project description cannot be empty", nameof(description));

        if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        Title = title.Trim();
        Description = description.Trim();
        TechnologiesUsed = technologiesUsed?.Trim() ?? string.Empty;
        ProjectUrl = string.IsNullOrWhiteSpace(projectUrl) ? null : projectUrl.Trim();
        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the featured status of the project.
    /// Featured projects are displayed prominently on the portfolio.
    /// </summary>
    /// <param name="isFeatured">Whether the project should be featured</param>
    public void SetFeaturedStatus(bool isFeatured)
    {
        IsFeatured = isFeatured;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds an image to the project.
    /// This method maintains the aggregate relationship.
    /// </summary>
    /// <param name="image">Image to add</param>
    public void AddImage(Image image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        Images.Add(image);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an image from the project.
    /// </summary>
    /// <param name="image">Image to remove</param>
    public void RemoveImage(Image image)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        Images.Remove(image);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a video to the project.
    /// This method maintains the aggregate relationship.
    /// </summary>
    /// <param name="video">Video to add</param>
    public void AddVideo(Video video)
    {
        if (video == null)
            throw new ArgumentNullException(nameof(video));

        Videos.Add(video);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes a video from the project.
    /// </summary>
    /// <param name="video">Video to remove</param>
    public void RemoveVideo(Video video)
    {
        if (video == null)
            throw new ArgumentNullException(nameof(video));

        Videos.Remove(video);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the project duration in days.
    /// Returns null if start or end date is not set.
    /// </summary>
    /// <returns>Duration in days or null</returns>
    public int? GetDurationInDays()
    {
        if (!StartDate.HasValue || !EndDate.HasValue)
            return null;

        return (EndDate.Value - StartDate.Value).Days;
    }

    /// <summary>
    /// Determines if the project is currently ongoing.
    /// A project is ongoing if it has a start date but no end date.
    /// </summary>
    /// <returns>True if ongoing, false otherwise</returns>
    public bool IsOngoing()
    {
        return StartDate.HasValue && !EndDate.HasValue;
    }

    /// <summary>
    /// Gets the technologies as a list instead of comma-separated string.
    /// This provides a more convenient way to work with technologies in code.
    /// </summary>
    /// <returns>List of technology names</returns>
    public List<string> GetTechnologiesList()
    {
        if (string.IsNullOrWhiteSpace(TechnologiesUsed))
            return new List<string>();

        return TechnologiesUsed
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(tech => tech.Trim())
            .Where(tech => !string.IsNullOrEmpty(tech))
            .ToList();
    }

    /// <summary>
    /// Sets the technologies from a list of strings.
    /// This provides a more convenient way to set technologies from code.
    /// </summary>
    /// <param name="technologies">List of technology names</param>
    public void SetTechnologies(IEnumerable<string> technologies)
    {
        if (technologies == null)
        {
            TechnologiesUsed = string.Empty;
        }
        else
        {
            var validTechnologies = technologies
                .Where(tech => !string.IsNullOrWhiteSpace(tech))
                .Select(tech => tech.Trim())
                .Distinct();

            TechnologiesUsed = string.Join(", ", validTechnologies);
        }

        UpdatedAt = DateTime.UtcNow;
    }
}

