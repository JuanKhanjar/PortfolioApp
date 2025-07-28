using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Domain.Interfaces;

/// <summary>
/// Repository interface for User entity operations.
/// This interface defines the contract for data access operations related to users.
/// Following the Repository pattern, this interface is defined in the domain layer
/// but implemented in the infrastructure layer to maintain dependency inversion.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address.
    /// This is useful for authentication and profile lookup scenarios.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users in the system.
    /// Note: For a portfolio application, there's typically only one user,
    /// but this method supports potential future multi-user scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all users</returns>
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user with their associated projects loaded.
    /// This method uses eager loading to include related project data.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user with projects if found, null otherwise</returns>
    Task<User?> GetWithProjectsAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added user with generated ID</returns>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user from the repository.
    /// Note: This should be used carefully as it may affect related data.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if user not found</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email already exists.
    /// Useful for validation during user creation or email updates.
    /// </summary>
    /// <param name="email">Email address to check</param>
    /// <param name="excludeUserId">User ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of users</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

