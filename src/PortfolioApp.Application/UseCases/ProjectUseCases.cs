using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.Mappers;
using PortfolioApp.Domain.Interfaces;

namespace PortfolioApp.Application.UseCases;

/// <summary>
/// Contains use cases for Project-related operations.
/// This class implements the business logic for project management,
/// following the Use Case Driven Design approach and Clean Architecture principles.
/// </summary>
public class ProjectUseCases
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the ProjectUseCases class.
    /// </summary>
    /// <param name="projectRepository">Repository for project data access.</param>
    /// <param name="userRepository">Repository for user data access.</param>
    public ProjectUseCases(IProjectRepository projectRepository, IUserRepository userRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="dto">The project creation data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created project DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user not found or project title already exists.</exception>
    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Validate business rules
        await ValidateProjectCreationAsync(dto, cancellationToken);

        // Map DTO to entity
        var project = ProjectMapper.ToEntity(dto);

        // Save to repository
        var createdProject = await _projectRepository.AddAsync(project, cancellationToken);

        // Map back to DTO and return
        return ProjectMapper.ToDto(createdProject);
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="dto">The project update data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated project DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when project not found or title conflict.</exception>
    public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Get existing project
        var existingProject = await _projectRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingProject == null)
        {
            throw new InvalidOperationException($"Project with ID {dto.Id} not found.");
        }

        // Validate business rules
        await ValidateProjectUpdateAsync(dto, existingProject.UserId, cancellationToken);

        // Update entity from DTO
        ProjectMapper.ToEntity(dto, existingProject);

        // Save changes
        var updatedProject = await _projectRepository.UpdateAsync(existingProject, cancellationToken);

        // Map back to DTO and return
        return ProjectMapper.ToDto(updatedProject);
    }

    /// <summary>
    /// Gets a project by ID with all its media (images and videos).
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The project DTO with media if found, null otherwise.</returns>
    public async Task<ProjectDto?> GetProjectWithMediaAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetWithMediaAsync(id, cancellationToken);
        return project != null ? ProjectMapper.ToDto(project) : null;
    }

    /// <summary>
    /// Gets all projects for a specific user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of project DTOs.</returns>
    public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
        return ProjectMapper.ToDtoCollection(projects);
    }

    /// <summary>
    /// Gets all projects for a specific user as summary DTOs.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of project summary DTOs.</returns>
    public async Task<IEnumerable<ProjectSummaryDto>> GetProjectSummariesByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
        return ProjectMapper.ToSummaryDtoCollection(projects);
    }

    /// <summary>
    /// Gets all projects for a specific user as card DTOs.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of project card DTOs.</returns>
    public async Task<IEnumerable<ProjectCardDto>> GetProjectCardsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
        return ProjectMapper.ToCardDtoCollection(projects);
    }

    /// <summary>
    /// Searches projects based on various criteria.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="searchDto">The search criteria.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of project DTOs matching the criteria.</returns>
    public async Task<IEnumerable<ProjectDto>> SearchProjectsAsync(int userId, ProjectSearchDto searchDto, CancellationToken cancellationToken = default)
    {
        if (searchDto == null)
        {
            throw new ArgumentNullException(nameof(searchDto));
        }

        IEnumerable<Domain.Entities.Project> projects;

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            projects = await _projectRepository.SearchAsync(userId, searchDto.SearchTerm, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(searchDto.Technology))
        {
            projects = await _projectRepository.GetByTechnologyAsync(userId, searchDto.Technology, cancellationToken);
        }
        else if (searchDto.StartDate.HasValue || searchDto.EndDate.HasValue)
        {
            projects = await _projectRepository.GetByDateRangeAsync(userId, searchDto.StartDate, searchDto.EndDate, cancellationToken);
        }
        else if (searchDto.FeaturedOnly == true)
        {
            projects = await _projectRepository.GetFeaturedByUserIdAsync(userId, cancellationToken);
        }
        else if (searchDto.OngoingOnly == true)
        {
            projects = await _projectRepository.GetOngoingByUserIdAsync(userId, cancellationToken);
        }
        else
        {
            projects = await _projectRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        // Apply pagination if requested
        if (searchDto.Skip.HasValue)
        {
            projects = projects.Skip(searchDto.Skip.Value);
        }
        if (searchDto.Take.HasValue)
        {
            projects = projects.Take(searchDto.Take.Value);
        }

        return ProjectMapper.ToDtoCollection(projects);
    }

    /// <summary>
    /// Deletes a project by ID.
    /// </summary>
    /// <param name="id">The project ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if deleted successfully, false if project not found.</returns>
    public async Task<bool> DeleteProjectAsync(int id, CancellationToken cancellationToken = default)
    {
        // Check if project exists
        var project = await _projectRepository.GetByIdAsync(id, cancellationToken);
        if (project == null)
        {
            return false;
        }

        // Delete the project (this should also cascade delete images and videos)
        return await _projectRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Validates project creation business rules.
    /// </summary>
    /// <param name="dto">The project creation data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private async Task ValidateProjectCreationAsync(CreateProjectDto dto, CancellationToken cancellationToken)
    {
        // Check if user exists
        var userExists = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (userExists == null)
        {
            throw new InvalidOperationException($"User with ID {dto.UserId} not found.");
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Project title is required.", nameof(dto.Title));
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new ArgumentException("Project description is required.", nameof(dto.Description));
        }

        // Check for duplicate title for the same user
        var titleExists = await _projectRepository.TitleExistsAsync(dto.UserId, dto.Title, null, cancellationToken);
        if (titleExists)
        {
            throw new InvalidOperationException($"A project with title \'{dto.Title}\' already exists for this user.");
        }

        // Validate date ranges
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
        {
            throw new ArgumentException("End date cannot be before start date.", nameof(dto.EndDate));
        }

        // Validate lengths
        if (dto.Title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(dto.Title));
        }
        if (dto.Description.Length > 5000)
        {
            throw new ArgumentException("Description cannot exceed 5000 characters.", nameof(dto.Description));
        }
        if (!string.IsNullOrWhiteSpace(dto.TechnologiesUsed) && dto.TechnologiesUsed.Length > 500)
        {
            throw new ArgumentException("Technologies used cannot exceed 500 characters.", nameof(dto.TechnologiesUsed));
        }
        if (!string.IsNullOrWhiteSpace(dto.ProjectUrl) && dto.ProjectUrl.Length > 500)
        {
            throw new ArgumentException("Project URL cannot exceed 500 characters.", nameof(dto.ProjectUrl));
        }
    }

    /// <summary>
    /// Validates project update business rules.
    /// </summary>
    /// <param name="dto">The project update data.</param>
    /// <param name="userId">The ID of the user who owns the project.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private async Task ValidateProjectUpdateAsync(UpdateProjectDto dto, int userId, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Project title is required.", nameof(dto.Title));
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new ArgumentException("Project description is required.", nameof(dto.Description));
        }

        // Check for duplicate title for the same user, excluding the current project
        var titleExists = await _projectRepository.TitleExistsAsync(userId, dto.Title, dto.Id, cancellationToken);
        if (titleExists)
        {
            throw new InvalidOperationException($"A project with title \'{dto.Title}\' already exists for this user.");
        }

        // Validate date ranges
        if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
        {
            throw new ArgumentException("End date cannot be before start date.", nameof(dto.EndDate));
        }

        // Validate lengths
        if (dto.Title.Length > 200)
        {
            throw new ArgumentException("Title cannot exceed 200 characters.", nameof(dto.Title));
        }
        if (dto.Description.Length > 5000)
        {
            throw new ArgumentException("Description cannot exceed 5000 characters.", nameof(dto.Description));
        }
        if (!string.IsNullOrWhiteSpace(dto.TechnologiesUsed) && dto.TechnologiesUsed.Length > 500)
        {
            throw new ArgumentException("Technologies used cannot exceed 500 characters.", nameof(dto.TechnologiesUsed));
        }
        if (!string.IsNullOrWhiteSpace(dto.ProjectUrl) && dto.ProjectUrl.Length > 500)
        {
            throw new ArgumentException("Project URL cannot exceed 500 characters.", nameof(dto.ProjectUrl));
        }
    }
}

