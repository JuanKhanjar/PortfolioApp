using Microsoft.EntityFrameworkCore;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.Interfaces;
using PortfolioApp.Infrastructure.Data;

namespace PortfolioApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Project entity operations.
/// This class provides concrete implementation of IProjectRepository interface,
/// handling all data access operations for Project entities using Entity Framework Core.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly PortfolioDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProjectRepository class.
    /// </summary>
    /// <param name="context">The database context for data access</param>
    public ProjectRepository(PortfolioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a project by its unique identifier.
    /// </summary>
    /// <param name="id">The project's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The project if found, null otherwise</returns>
    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving project with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets a project with all related media (images and videos) loaded.
    /// </summary>
    /// <param name="id">The project's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The project with media if found, null otherwise</returns>
    public async Task<Project?> GetWithMediaAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving project with media for ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets all projects for a specific user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects belonging to the user</returns>
    public async Task<IEnumerable<Project>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving projects for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets all featured projects for a specific user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of featured projects</returns>
    public async Task<IEnumerable<Project>> GetFeaturedByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .Where(p => p.UserId == userId && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving featured projects for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets projects ordered by creation date (most recent first).
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="take">Maximum number of projects to return (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recent projects</returns>
    public async Task<IEnumerable<Project>> GetRecentByUserIdAsync(int userId, int? take = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Projects
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking();

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving recent projects for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets projects that contain specific technologies.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="technology">Technology name to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects using the specified technology</returns>
    public async Task<IEnumerable<Project>> GetByTechnologyAsync(int userId, string technology, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(technology))
            throw new ArgumentException("Technology cannot be null or empty", nameof(technology));

        try
        {
            var searchTech = technology.Trim();
            return await _context.Projects
                .Where(p => p.UserId == userId && p.TechnologiesUsed.Contains(searchTech))
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving projects by technology '{technology}' for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Searches projects by title or description.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects matching the search term</returns>
    public async Task<IEnumerable<Project>> SearchAsync(int userId, string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetByUserIdAsync(userId, cancellationToken); // Return all if search term is empty

        try
        {
            var searchLower = searchTerm.Trim().ToLowerInvariant();
            return await _context.Projects
                .Where(p => p.UserId == userId && 
                            (p.Title.ToLower().Contains(searchLower) || 
                             p.Description.ToLower().Contains(searchLower) ||
                             p.TechnologiesUsed.ToLower().Contains(searchLower)))
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error searching projects for user ID {userId} with term '{searchTerm}'", ex);
        }
    }

    /// <summary>
    /// Gets projects within a specific date range.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="startDate">Start of the date range (optional)</param>
    /// <param name="endDate">End of the date range (optional)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of projects within the date range</returns>
    public async Task<IEnumerable<Project>> GetByDateRangeAsync(int userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Projects
                .Where(p => p.UserId == userId)
                .AsNoTracking();

            if (startDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= startDate.Value || p.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= endDate.Value || p.CreatedAt <= endDate.Value);
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving projects by date range for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets ongoing projects (projects with start date but no end date).
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of ongoing projects</returns>
    public async Task<IEnumerable<Project>> GetOngoingByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects
                .Where(p => p.UserId == userId && p.StartDate.HasValue && !p.EndDate.HasValue)
                .OrderByDescending(p => p.StartDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving ongoing projects for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Adds a new project to the repository.
    /// </summary>
    /// <param name="project">The project entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added project with generated ID</returns>
    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        try
        {
            // Check if a project with the same title already exists for this user
            var exists = await TitleExistsAsync(project.UserId, project.Title, null, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"A project with title '{project.Title}' already exists for this user.");

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);
            return project;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error adding project", ex);
        }
    }

    /// <summary>
    /// Updates an existing project in the repository.
    /// </summary>
    /// <param name="project">The project entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated project</returns>
    public async Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        try
        {
            // Check if title is being changed to an existing title for this user
            var exists = await TitleExistsAsync(project.UserId, project.Title, project.Id, cancellationToken);
            if (exists)
                throw new InvalidOperationException($"A project with title '{project.Title}' already exists for this user.");

            _context.Projects.Update(project);
            await _context.SaveChangesAsync(cancellationToken);
            return project;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating project with ID {project.Id}", ex);
        }
    }

    /// <summary>
    /// Deletes a project from the repository.
    /// This will also delete related media files and database records.
    /// </summary>
    /// <param name="id">The ID of the project to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if project not found</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var project = await _context.Projects
                .Include(p => p.Images)
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (project == null)
                return false;

            // Optionally, delete physical files here if not handled by a separate service
            // For now, assume a separate service handles physical file deletion

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error deleting project with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets the total count of projects for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of projects</returns>
    public async Task<int> GetCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects.CountAsync(p => p.UserId == userId, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting project count for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets the count of featured projects for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of featured projects</returns>
    public async Task<int> GetFeaturedCountByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Projects.CountAsync(p => p.UserId == userId && p.IsFeatured, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error getting featured project count for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Gets all unique technologies used across a user's projects.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of unique technology names</returns>
    public async Task<IEnumerable<string>> GetAllTechnologiesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var technologies = await _context.Projects
                .Where(p => p.UserId == userId && !string.IsNullOrWhiteSpace(p.TechnologiesUsed))
                .Select(p => p.TechnologiesUsed)
                .ToListAsync(cancellationToken);

            return technologies
                .SelectMany(t => t.Split(",", StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving technologies for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Checks if a project with the specified title already exists for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="title">Project title to check</param>
    /// <param name="excludeProjectId">Project ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if title exists, false otherwise</returns>
    public async Task<bool> TitleExistsAsync(int userId, string title, int? excludeProjectId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));

        try
        {
            var searchTitle = title.Trim();
            var query = _context.Projects.AsNoTracking()
                .Where(p => p.UserId == userId && p.Title == searchTitle);

            if (excludeProjectId.HasValue)
            {
                query = query.Where(p => p.Id != excludeProjectId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error checking if project title '{title}' exists for user ID {userId}", ex);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _context?.Dispose();
    }
}

