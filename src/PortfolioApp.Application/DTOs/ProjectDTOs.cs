namespace PortfolioApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new project.
/// This DTO contains all the required information for project creation.
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// ID of the user who owns this project.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Project title. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed project description. Required field.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Shortened version of the description for display purposes.
    /// </summary>
    public string ShortDescription => Description.Length > 150 ? Description.Substring(0, 150) + "..." : Description;

    /// <summary>
    /// Comma-separated list of technologies used in the project.
    /// </summary>
    public string TechnologiesUsed { get; set; } = string.Empty;

    /// <summary>
    /// URL to the live project or repository. Optional field.
    /// </summary>
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Project start date. Optional field.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Project end date. Optional field (null indicates ongoing project).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indicates whether this project should be featured prominently.
    /// </summary>
    public bool IsFeatured { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing project.
/// This DTO contains all the fields that can be updated for a project.
/// </summary>
public class UpdateProjectDto
{
    /// <summary>
    /// Project's unique identifier. Required for updates.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project title. Required field.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed project description. Required field.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Shortened version of the description for display purposes.
    /// </summary>
    public string ShortDescription => Description.Length > 150 ? Description.Substring(0, 150) + "..." : Description;

    /// <summary>
    /// Comma-separated list of technologies used in the project.
    /// </summary>
    public string TechnologiesUsed { get; set; } = string.Empty;

    /// <summary>
    /// URL to the live project or repository. Optional field.
    /// </summary>
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Project start date. Optional field.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Project end date. Optional field (null indicates ongoing project).
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Indicates whether this project should be featured prominently.
    /// </summary>
    public bool IsFeatured { get; set; }
}

/// <summary>
/// Data Transfer Object for project information display.
/// This DTO contains all project information formatted for presentation.
/// </summary>
public class ProjectDto
{
    /// <summary>
    /// Project's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID of the user who owns this project.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Project title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed project description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated list of technologies used.
    /// </summary>
    public string TechnologiesUsed { get; set; } = string.Empty;

    /// <summary>
    /// List of individual technologies for easier display.
    /// </summary>
    public IEnumerable<string> Technologies { get; set; } = new List<string>();

    /// <summary>
    /// URL to the live project or repository.
    /// </summary>
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Project start date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Project end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Formatted date range string for display.
    /// </summary>
    public string DateRange { get; set; } = string.Empty;

    /// <summary>
    /// Project duration in days (if both dates are available).
    /// </summary>
    public int? DurationInDays { get; set; }

    /// <summary>
    /// Indicates whether the project is currently ongoing.
    /// </summary>
    public bool IsOngoing { get; set; }

    /// <summary>
    /// Indicates whether this project is featured.
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Timestamp when the project record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the project record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Collection of images associated with this project.
    /// </summary>
    public IEnumerable<ImageDto> Images { get; set; } = new List<ImageDto>();

    /// <summary>
    /// Collection of videos associated with this project.
    /// </summary>
    public IEnumerable<VideoDto> Videos { get; set; } = new List<VideoDto>();

    /// <summary>
    /// Total number of media items (images + videos).
    /// </summary>
    public int MediaCount { get; set; }
}

/// <summary>
/// Data Transfer Object for project summary information.
/// This DTO contains minimal project information for list displays and cards.
/// </summary>
public class ProjectSummaryDto
{
    /// <summary>
    /// Project's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Brief project description (truncated if necessary).
    /// </summary>
    public string ShortDescription { get; set; } = string.Empty;

    /// <summary>
    /// List of key technologies used.
    /// </summary>
    public IEnumerable<string> KeyTechnologies { get; set; } = new List<string>();

    /// <summary>
    /// URL to the live project or repository.
    /// </summary>
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Formatted date range string for display.
    /// </summary>
    public string DateRange { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the project is currently ongoing.
    /// </summary>
    public bool IsOngoing { get; set; }

    /// <summary>
    /// Indicates whether this project is featured.
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// URL to the primary project image (for thumbnail display).
    /// </summary>
    public string? PrimaryImageUrl { get; set; }

    /// <summary>
    /// Number of images associated with this project.
    /// </summary>
    public int ImageCount { get; set; }

    /// <summary>
    /// Number of videos associated with this project.
    /// </summary>
    public int VideoCount { get; set; }
}

/// <summary>
/// Data Transfer Object for project card display.
/// This DTO is optimized for card-based layouts on the portfolio homepage.
/// </summary>
public class ProjectCardDto
{
    /// <summary>
    /// Project's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Brief project description (limited to card display).
    /// </summary>
    public string CardDescription { get; set; } = string.Empty;

    /// <summary>
    /// Top 3-4 technologies for card display.
    /// </summary>
    public IEnumerable<string> DisplayTechnologies { get; set; } = new List<string>();

    /// <summary>
    /// URL to the live project or repository.
    /// </summary>
    public string? ProjectUrl { get; set; }

    /// <summary>
    /// Formatted date for card display (e.g., "2023" or "2023 - Ongoing").
    /// </summary>
    public string DisplayDate { get; set; } = string.Empty;

    /// <summary>
    /// URL to the card thumbnail image.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Indicates whether this project is featured.
    /// </summary>
    public bool IsFeatured { get; set; }
}

/// <summary>
/// Data Transfer Object for project search and filtering.
/// This DTO contains search criteria and results.
/// </summary>
public class ProjectSearchDto
{
    /// <summary>
    /// Search term for title and description.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Technology filter.
    /// </summary>
    public string? Technology { get; set; }

    /// <summary>
    /// Start date filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Filter for featured projects only.
    /// </summary>
    public bool? FeaturedOnly { get; set; }

    /// <summary>
    /// Filter for ongoing projects only.
    /// </summary>
    public bool? OngoingOnly { get; set; }

    /// <summary>
    /// Number of results to return (for pagination).
    /// </summary>
    public int? Take { get; set; }

    /// <summary>
    /// Number of results to skip (for pagination).
    /// </summary>
    public int? Skip { get; set; }
}

