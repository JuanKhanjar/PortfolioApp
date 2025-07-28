using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Domain.Interfaces;

/// <summary>
/// Repository interface for Project entity operations.
/// This interface defines the contract for data access operations related to projects.
/// It includes methods for querying, filtering, and managing project data.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Gets a project by its unique identifier.
    /// </summary>
    /// <param name="id">The project's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The project if found, null otherwise</returns>
    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project with all related media (images and videos) loaded.
    /// This method uses eager loading to include related media data.
    /// </summary>
    /// <param name="id">The project's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The project with media if found, null otherwise</returns>
    Task<Project?> GetWithMediaAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all projects for a specific user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects belonging to the user</returns>
    Task<IEnumerable<Project>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all featured projects for a specific user.
    /// Featured projects are typically displayed prominently on the portfolio.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of featured projects</returns>
    Task<IEnumerable<Project>> GetFeaturedByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects ordered by creation date (most recent first).
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="take">Maximum number of projects to return (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recent projects</returns>
    Task<IEnumerable<Project>> GetRecentByUserIdAsync(int userId, int? take = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects that contain specific technologies.
    /// This method searches within the TechnologiesUsed field.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="technology">Technology name to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects using the specified technology</returns>
    Task<IEnumerable<Project>> GetByTechnologyAsync(int userId, string technology, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches projects by title or description.
    /// This method performs a case-insensitive search across project content.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects matching the search term</returns>
    Task<IEnumerable<Project>> SearchAsync(int userId, string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects within a specific date range.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="startDate">Start of the date range (optional)</param>
    /// <param name="endDate">End of the date range (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects within the date range</returns>
    Task<IEnumerable<Project>> GetByDateRangeAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ongoing projects (projects with start date but no end date).
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of ongoing projects</returns>
    Task<IEnumerable<Project>> GetOngoingByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new project to the repository.
    /// </summary>
    /// <param name="project">The project entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added project with generated ID</returns>
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project in the repository.
    /// </summary>
    /// <param name="project">The project entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated project</returns>
    Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project from the repository.
    /// This will also delete related media files and database records.
    /// </summary>
    /// <param name="id">The ID of the project to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if project not found</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of projects for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of projects</returns>
    Task<int> GetCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of featured projects for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of featured projects</returns>
    Task<int> GetFeaturedCountByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unique technologies used across a user's projects.
    /// This method extracts and returns a distinct list of technologies.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of unique technology names</returns>
    Task<IEnumerable<string>> GetAllTechnologiesByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a project with the specified title already exists for a user.
    /// Useful for validation during project creation.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="title">Project title to check</param>
    /// <param name="excludeProjectId">Project ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if title exists, false otherwise</returns>
    Task<bool> TitleExistsAsync(int userId, string title, int? excludeProjectId = null, CancellationToken cancellationToken = default);
}

