using Microsoft.EntityFrameworkCore;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.Interfaces;
using PortfolioApp.Infrastructure.Data;

namespace PortfolioApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity operations.
/// This class provides concrete implementation of IUserRepository interface,
/// handling all data access operations for User entities using Entity Framework Core.
/// 
/// The repository follows Clean Architecture principles by:
/// - Implementing interfaces defined in the Domain layer
/// - Encapsulating Entity Framework-specific logic
/// - Providing a clean abstraction over data access
/// - Supporting dependency injection and testability
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly PortfolioDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context for data access</param>
    public UserRepository(PortfolioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking() // Optimize for read-only scenarios
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving user with ID {id}", ex);
        }
    }

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving user with email {email}", ex);
        }
    }

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of all users</returns>
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving all users", ex);
        }
    }

    /// <summary>
    /// Gets a user with their associated projects loaded.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The user with projects if found, null otherwise</returns>
    public async Task<User?> GetWithProjectsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Images) // Include project images
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Videos) // Include project videos
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving user with projects for ID {id}", ex);
        }
    }

    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user entity to add</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The added user with generated ID</returns>
    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            // Check if email already exists
            var existingUser = await GetByEmailAsync(user.Email.Value, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"A user with email {user.Email.Value} already exists");

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error adding user", ex);
        }
    }

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user entity to update</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The updated user</returns>
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        try
        {
            // Check if email is being changed to an existing email
            var existingUserWithEmail = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Value == user.Email.Value && u.Id != user.Id, cancellationToken);

            if (existingUserWithEmail != null)
                throw new InvalidOperationException($"A user with email {user.Email.Value} already exists");

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business logic exceptions
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating user with ID {user.Id}", ex);
        }
    }

    /// <summary>
    /// Deletes a user from the repository.
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if deleted successfully, false if user not found</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error deleting user with ID {id}", ex);
        }
    }

    /// <summary>
    /// Checks if a user with the specified email already exists.
    /// </summary>
    /// <param name="email">Email address to check</param>
    /// <param name="excludeUserId">User ID to exclude from the check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>True if email exists, false otherwise</returns>
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        try
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var query = _context.Users.AsNoTracking()
                .Where(u => u.Email.Value == normalizedEmail);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error checking if email {email} exists", ex);
        }
    }

    /// <summary>
    /// Gets the total count of users in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Total number of users</returns>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Users.CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error getting user count", ex);
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

