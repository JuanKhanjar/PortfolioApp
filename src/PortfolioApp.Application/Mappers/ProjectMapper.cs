using PortfolioApp.Application.DTOs;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Application.Mappers;

/// <summary>
/// Provides manual mapping functionalities between Project entities and Project DTOs.
/// This mapper ensures a clear separation of concerns and avoids direct exposure
/// of domain entities to the presentation layer, adhering to Clean Architecture principles.
/// </summary>
public static class ProjectMapper
{
    /// <summary>
    /// Maps a Project entity to a ProjectDto.
    /// This method transforms the domain model into a presentation-friendly DTO.
    /// </summary>
    /// <param name="project">The Project entity to map.</param>
    /// <returns>A new ProjectDto instance.</returns>
    public static ProjectDto ToDto(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        return new ProjectDto
        {
            Id = project.Id,
            UserId = project.UserId,
            Title = project.Title,
            Description = project.Description,
            TechnologiesUsed = project.TechnologiesUsed,
            Technologies = project.GetTechnologiesList(),
            ProjectUrl = project.ProjectUrl,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            DateRange = FormatDateRange(project.StartDate, project.EndDate),
            DurationInDays = project.GetDurationInDays(),
            IsOngoing = project.IsOngoing(),
            IsFeatured = project.IsFeatured,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            Images = project.Images?.Select(ImageMapper.ToDto).ToList() ?? new List<ImageDto>(),
            Videos = project.Videos?.Select(VideoMapper.ToDto).ToList() ?? new List<VideoDto>(),
            MediaCount = (project.Images?.Count ?? 0) + (project.Videos?.Count ?? 0)
        };
    }

    /// <summary>
    /// Maps a CreateProjectDto to a Project entity.
    /// This method transforms a DTO from the presentation layer into a domain entity.
    /// </summary>
    /// <param name="dto">The CreateProjectDto to map.</param>
    /// <returns>A new Project entity instance.</returns>
    public static Project ToEntity(CreateProjectDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var project = new Project(
            dto.UserId,
            dto.Title,
            dto.Description,
            dto.TechnologiesUsed,
            dto.ProjectUrl,
            dto.StartDate,
            dto.EndDate
        );

        if (dto.IsFeatured)
        {
            project.SetFeaturedStatus(true);
        }

        return project;
    }

    /// <summary>
    /// Updates an existing Project entity from an UpdateProjectDto.
    /// This method modifies an existing domain entity based on the DTO.
    /// </summary>
    /// <param name="dto">The UpdateProjectDto containing updated information.</param>
    /// <param name="project">The existing Project entity to update.</param>
    public static void ToEntity(UpdateProjectDto dto, Project project)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        project.UpdateDetails(
            dto.Title,
            dto.Description,
            dto.TechnologiesUsed,
            dto.ProjectUrl,
            dto.StartDate,
            dto.EndDate
        );

        project.SetFeaturedStatus(dto.IsFeatured);
    }

    /// <summary>
    /// Maps a Project entity to a ProjectSummaryDto.
    /// This method provides a lightweight DTO for summary views.
    /// </summary>
    /// <param name="project">The Project entity to map.</param>
    /// <returns>A new ProjectSummaryDto instance.</returns>
    public static ProjectSummaryDto ToSummaryDto(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        var technologies = project.GetTechnologiesList();
        var keyTechnologies = technologies.Take(4).ToList(); // Show only first 4 technologies

        return new ProjectSummaryDto
        {
            Id = project.Id,
            Title = project.Title,
            ShortDescription = TruncateDescription(project.Description, 150),
            KeyTechnologies = keyTechnologies,
            ProjectUrl = project.ProjectUrl,
            DateRange = FormatDateRange(project.StartDate, project.EndDate),
            IsOngoing = project.IsOngoing(),
            IsFeatured = project.IsFeatured,
            PrimaryImageUrl = project.Images?.FirstOrDefault()?.Url,
            ImageCount = project.Images?.Count ?? 0,
            VideoCount = project.Videos?.Count ?? 0
        };
    }

    /// <summary>
    /// Maps a Project entity to a ProjectCardDto.
    /// This method provides a DTO optimized for card-based layouts.
    /// </summary>
    /// <param name="project">The Project entity to map.</param>
    /// <returns>A new ProjectCardDto instance.</returns>
    public static ProjectCardDto ToCardDto(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        var technologies = project.GetTechnologiesList();
        var displayTechnologies = technologies.Take(3).ToList(); // Show only first 3 technologies for cards

        return new ProjectCardDto
        {
            Id = project.Id,
            Title = project.Title,
            CardDescription = TruncateDescription(project.Description, 100),
            DisplayTechnologies = displayTechnologies,
            ProjectUrl = project.ProjectUrl,
            DisplayDate = FormatDisplayDate(project.StartDate, project.EndDate),
            ThumbnailUrl = project.Images?.FirstOrDefault()?.Url,
            IsFeatured = project.IsFeatured
        };
    }

    /// <summary>
    /// Maps a collection of Project entities to ProjectDto collection.
    /// </summary>
    /// <param name="projects">Collection of Project entities to map.</param>
    /// <returns>Collection of ProjectDto instances.</returns>
    public static IEnumerable<ProjectDto> ToDtoCollection(IEnumerable<Project> projects)
    {
        if (projects == null)
        {
            throw new ArgumentNullException(nameof(projects));
        }

        return projects.Select(ToDto).ToList();
    }

    /// <summary>
    /// Maps a collection of Project entities to ProjectSummaryDto collection.
    /// </summary>
    /// <param name="projects">Collection of Project entities to map.</param>
    /// <returns>Collection of ProjectSummaryDto instances.</returns>
    public static IEnumerable<ProjectSummaryDto> ToSummaryDtoCollection(IEnumerable<Project> projects)
    {
        if (projects == null)
        {
            throw new ArgumentNullException(nameof(projects));
        }

        return projects.Select(ToSummaryDto).ToList();
    }

    /// <summary>
    /// Maps a collection of Project entities to ProjectCardDto collection.
    /// </summary>
    /// <param name="projects">Collection of Project entities to map.</param>
    /// <returns>Collection of ProjectCardDto instances.</returns>
    public static IEnumerable<ProjectCardDto> ToCardDtoCollection(IEnumerable<Project> projects)
    {
        if (projects == null)
        {
            throw new ArgumentNullException(nameof(projects));
        }

        return projects.Select(ToCardDto).ToList();
    }

    /// <summary>
    /// Formats a date range for display purposes.
    /// </summary>
    /// <param name="startDate">The start date of the project.</param>
    /// <param name="endDate">The end date of the project.</param>
    /// <returns>A formatted date range string.</returns>
    private static string FormatDateRange(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue && !endDate.HasValue)
        {
            return "Date not specified";
        }

        if (startDate.HasValue && !endDate.HasValue)
        {
            return $"{startDate.Value:MMM yyyy} - Ongoing";
        }

        if (!startDate.HasValue && endDate.HasValue)
        {
            return $"Completed {endDate.Value:MMM yyyy}";
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            if (startDate.Value.Year == endDate.Value.Year && startDate.Value.Month == endDate.Value.Month)
            {
                return $"{startDate.Value:MMM yyyy}";
            }
            else if (startDate.Value.Year == endDate.Value.Year)
            {
                return $"{startDate.Value:MMM} - {endDate.Value:MMM yyyy}";
            }
            else
            {
                return $"{startDate.Value:MMM yyyy} - {endDate.Value:MMM yyyy}";
            }
        }

        return "Date not specified";
    }

    /// <summary>
    /// Formats a display date for card layouts.
    /// </summary>
    /// <param name="startDate">The start date of the project.</param>
    /// <param name="endDate">The end date of the project.</param>
    /// <returns>A formatted display date string.</returns>
    private static string FormatDisplayDate(DateTime? startDate, DateTime? endDate)
    {
        if (!startDate.HasValue && !endDate.HasValue)
        {
            return "";
        }

        if (startDate.HasValue && !endDate.HasValue)
        {
            return $"{startDate.Value.Year} - Ongoing";
        }

        if (!startDate.HasValue && endDate.HasValue)
        {
            return $"{endDate.Value.Year}";
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            if (startDate.Value.Year == endDate.Value.Year)
            {
                return $"{startDate.Value.Year}";
            }
            else
            {
                return $"{startDate.Value.Year} - {endDate.Value.Year}";
            }
        }

        return "";
    }

    /// <summary>
    /// Truncates a description to a specified maximum length.
    /// </summary>
    /// <param name="description">The description to truncate.</param>
    /// <param name="maxLength">The maximum length of the truncated description.</param>
    /// <returns>A truncated description string.</returns>
    private static string TruncateDescription(string description, int maxLength)
    {
        if (string.IsNullOrEmpty(description))
        {
            return string.Empty;
        }

        if (description.Length <= maxLength)
        {
            return description;
        }

        // Find the last space before the max length to avoid cutting words
        var truncated = description.Substring(0, maxLength);
        var lastSpaceIndex = truncated.LastIndexOf(' ');

        if (lastSpaceIndex > 0)
        {
            truncated = truncated.Substring(0, lastSpaceIndex);
        }

        return truncated + "...";
    }
}

