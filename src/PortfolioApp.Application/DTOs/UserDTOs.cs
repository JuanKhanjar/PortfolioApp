namespace PortfolioApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new user.
/// This DTO contains all the required information for user creation
/// and serves as the contract between the presentation and application layers.
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// User's first name. Required field.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name. Required field.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address. Required field.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number. Optional field.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's professional biography. Optional field.
    /// </summary>
    public string Bio { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for updating an existing user.
/// This DTO contains all the fields that can be updated for a user.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// User's unique identifier. Required for updates.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's first name. Required field.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name. Required field.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address. Required field.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number. Optional field.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's professional biography. Optional field.
    /// </summary>
    public string Bio { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for user profile picture updates.
/// This DTO is used specifically for updating profile pictures.
/// </summary>
public class UpdateUserProfilePictureDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// URL to the new profile picture.
    /// </summary>
    public string ProfilePictureUrl { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for user resume updates.
/// This DTO is used specifically for updating resume files.
/// </summary>
public class UpdateUserResumeDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// URL to the new resume file.
    /// </summary>
    public string ResumeUrl { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for user information display.
/// This DTO contains all user information formatted for presentation.
/// </summary>
public class UserDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name (first + last).
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's professional biography.
    /// </summary>
    public string Bio { get; set; } = string.Empty;

    /// <summary>
    /// URL to the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// URL to the user's resume file.
    /// </summary>
    public string? ResumeUrl { get; set; }

    /// <summary>
    /// Timestamp when the user record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the user record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Number of projects associated with this user.
    /// </summary>
    public int ProjectCount { get; set; }

    /// <summary>
    /// Number of featured projects associated with this user.
    /// </summary>
    public int FeaturedProjectCount { get; set; }
}

/// <summary>
/// Data Transfer Object for user summary information.
/// This DTO contains minimal user information for list displays.
/// </summary>
public class UserSummaryDto
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL to the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Number of projects associated with this user.
    /// </summary>
    public int ProjectCount { get; set; }
}

/// <summary>
/// Data Transfer Object for user profile information displayed on the public portfolio.
/// This DTO contains only the information that should be publicly visible.
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// User's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's email address (for contact purposes).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number (if provided and public).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// User's professional biography.
    /// </summary>
    public string Bio { get; set; } = string.Empty;

    /// <summary>
    /// URL to the user's profile picture.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// URL to the user's resume file.
    /// </summary>
    public string? ResumeUrl { get; set; }

    /// <summary>
    /// Collection of featured projects for the portfolio display.
    /// </summary>
    public IEnumerable<ProjectSummaryDto> FeaturedProjects { get; set; } = new List<ProjectSummaryDto>();

    /// <summary>
    /// Collection of recent projects for the portfolio display.
    /// </summary>
    public IEnumerable<ProjectSummaryDto> RecentProjects { get; set; } = new List<ProjectSummaryDto>();

    /// <summary>
    /// Collection of all unique technologies used across projects.
    /// </summary>
    public IEnumerable<string> Technologies { get; set; } = new List<string>();
}

