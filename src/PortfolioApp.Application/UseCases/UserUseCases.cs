using PortfolioApp.Application.DTOs;
using PortfolioApp.Application.Mappers;
using PortfolioApp.Domain.Interfaces;

namespace PortfolioApp.Application.UseCases;

/// <summary>
/// Contains use cases for User-related operations.
/// This class implements the business logic for user management,
/// following the Use Case Driven Design approach and Clean Architecture principles.
/// </summary>
public class UserUseCases
{
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;

    /// <summary>
    /// Initializes a new instance of the UserUseCases class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="projectRepository">Repository for project data access.</param>
    public UserUseCases(IUserRepository userRepository, IProjectRepository projectRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="dto">The user creation data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The created user DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when email already exists.</exception>
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Validate business rules
        await ValidateUserCreationAsync(dto, cancellationToken);

        // Map DTO to entity
        var user = UserMapper.ToEntity(dto);

        // Save to repository
        var createdUser = await _userRepository.AddAsync(user, cancellationToken);

        // Map back to DTO and return
        return UserMapper.ToDto(createdUser);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="dto">The user update data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated user DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user not found or email conflict.</exception>
    public async Task<UserDto> UpdateUserAsync(UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Get existing user
        var existingUser = await _userRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {dto.Id} not found.");
        }

        // Validate business rules
        await ValidateUserUpdateAsync(dto, cancellationToken);

        // Update entity from DTO
        UserMapper.ToEntity(dto, existingUser);

        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        // Map back to DTO and return
        return UserMapper.ToDto(updatedUser);
    }

    /// <summary>
    /// Updates a user's profile picture.
    /// </summary>
    /// <param name="dto">The profile picture update data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated user DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user not found.</exception>
    public async Task<UserDto> UpdateProfilePictureAsync(UpdateUserProfilePictureDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Get existing user
        var existingUser = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {dto.UserId} not found.");
        }

        // Update profile picture
        existingUser.UpdateProfilePicture(dto.ProfilePictureUrl);

        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        // Map back to DTO and return
        return UserMapper.ToDto(updatedUser);
    }

    /// <summary>
    /// Updates a user's resume.
    /// </summary>
    /// <param name="dto">The resume update data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The updated user DTO.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user not found.</exception>
    public async Task<UserDto> UpdateResumeAsync(UpdateUserResumeDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Get existing user
        var existingUser = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (existingUser == null)
        {
            throw new InvalidOperationException($"User with ID {dto.UserId} not found.");
        }

        // Update resume
        existingUser.UpdateResume(dto.ResumeUrl);

        // Save changes
        var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);

        // Map back to DTO and return
        return UserMapper.ToDto(updatedUser);
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The user DTO if found, null otherwise.</returns>
    public async Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user != null ? UserMapper.ToDto(user) : null;
    }

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The user DTO if found, null otherwise.</returns>
    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return user != null ? UserMapper.ToDto(user) : null;
    }

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of user DTOs.</returns>
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(UserMapper.ToDto).ToList();
    }

    /// <summary>
    /// Gets all users as summary DTOs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Collection of user summary DTOs.</returns>
    public async Task<IEnumerable<UserSummaryDto>> GetAllUsersSummaryAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(UserMapper.ToSummaryDto).ToList();
    }

    /// <summary>
    /// Gets a user's public profile information.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>The user profile DTO if found, null otherwise.</returns>
    public async Task<UserProfileDto?> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        // Get user with projects
        var user = await _userRepository.GetWithProjectsAsync(userId, cancellationToken);
        if (user == null)
        {
            return null;
        }

        // Get featured projects
        var featuredProjects = await _projectRepository.GetFeaturedByUserIdAsync(userId, cancellationToken);

        // Get recent projects (last 6 projects)
        var recentProjects = await _projectRepository.GetRecentByUserIdAsync(userId, 6, cancellationToken);

        // Get all technologies used
        var technologies = await _projectRepository.GetAllTechnologiesByUserIdAsync(userId, cancellationToken);

        // Map to profile DTO
        return UserMapper.ToProfileDto(user, featuredProjects, recentProjects, technologies);
    }

    /// <summary>
    /// Deletes a user and all associated data.
    /// </summary>
    /// <param name="id">The user ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if deleted successfully, false if user not found.</returns>
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        // Check if user exists
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return false;
        }

        // Delete all user's projects first (cascade delete should handle this, but being explicit)
        var userProjects = await _projectRepository.GetByUserIdAsync(id, cancellationToken);
        foreach (var project in userProjects)
        {
            await _projectRepository.DeleteAsync(project.Id, cancellationToken);
        }

        // Delete the user
        return await _userRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Checks if an email address is available for use.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="excludeUserId">User ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if email is available, false if already in use.</returns>
    public async Task<bool> IsEmailAvailableAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        return !await _userRepository.EmailExistsAsync(email, excludeUserId, cancellationToken);
    }

    /// <summary>
    /// Gets the total count of users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>Total number of users.</returns>
    public async Task<int> GetUserCountAsync(CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetCountAsync(cancellationToken);
    }

    /// <summary>
    /// Validates user creation business rules.
    /// </summary>
    /// <param name="dto">The user creation data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private async Task ValidateUserCreationAsync(CreateUserDto dto, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException("First name is required.", nameof(dto.FirstName));
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new ArgumentException("Last name is required.", nameof(dto.LastName));
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email is required.", nameof(dto.Email));
        }

        // Validate email format (basic validation, more detailed validation in Email value object)
        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
        {
            throw new ArgumentException("Invalid email format.", nameof(dto.Email));
        }

        // Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(dto.Email, null, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
        }

        // Validate name lengths
        if (dto.FirstName.Length > 50)
        {
            throw new ArgumentException("First name cannot exceed 50 characters.", nameof(dto.FirstName));
        }

        if (dto.LastName.Length > 50)
        {
            throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(dto.LastName));
        }

        // Validate phone number if provided
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber.Length > 20)
        {
            throw new ArgumentException("Phone number cannot exceed 20 characters.", nameof(dto.PhoneNumber));
        }

        // Validate bio length
        if (dto.Bio.Length > 2000)
        {
            throw new ArgumentException("Bio cannot exceed 2000 characters.", nameof(dto.Bio));
        }
    }

    /// <summary>
    /// Validates user update business rules.
    /// </summary>
    /// <param name="dto">The user update data.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    private async Task ValidateUserUpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException("First name is required.", nameof(dto.FirstName));
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new ArgumentException("Last name is required.", nameof(dto.LastName));
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email is required.", nameof(dto.Email));
        }

        // Validate email format
        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
        {
            throw new ArgumentException("Invalid email format.", nameof(dto.Email));
        }

        // Check if email already exists for another user
        var emailExists = await _userRepository.EmailExistsAsync(dto.Email, dto.Id, cancellationToken);
        if (emailExists)
        {
            throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");
        }

        // Validate name lengths
        if (dto.FirstName.Length > 50)
        {
            throw new ArgumentException("First name cannot exceed 50 characters.", nameof(dto.FirstName));
        }

        if (dto.LastName.Length > 50)
        {
            throw new ArgumentException("Last name cannot exceed 50 characters.", nameof(dto.LastName));
        }

        // Validate phone number if provided
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && dto.PhoneNumber.Length > 20)
        {
            throw new ArgumentException("Phone number cannot exceed 20 characters.", nameof(dto.PhoneNumber));
        }

        // Validate bio length
        if (dto.Bio.Length > 2000)
        {
            throw new ArgumentException("Bio cannot exceed 2000 characters.", nameof(dto.Bio));
        }
    }
}

