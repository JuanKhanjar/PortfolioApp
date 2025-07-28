using PortfolioApp.Domain.ValueObjects;

namespace PortfolioApp.Domain.Entities;

/// <summary>
/// Represents the portfolio owner/user entity.
/// This is the root aggregate that contains personal information and manages the portfolio content.
/// Following DDD principles, this entity encapsulates business rules and maintains data consistency.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user. This serves as the primary key.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// User's first name. Required field with business validation.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// User's last name. Required field with business validation.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// User's email address. Implemented as a value object to ensure validation.
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// User's phone number. Optional field with format validation.
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Professional biography or summary. Rich text content for portfolio presentation.
    /// </summary>
    public string Bio { get; private set; } = string.Empty;

    /// <summary>
    /// URL to the user's profile picture. Stored as relative path from wwwroot.
    /// </summary>
    public string? ProfilePictureUrl { get; private set; }

    /// <summary>
    /// URL to the user's resume/CV file. Stored as relative path from wwwroot.
    /// </summary>
    public string? ResumeUrl { get; private set; }

    /// <summary>
    /// Timestamp when the user record was created. Audit field.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Timestamp when the user record was last updated. Audit field.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Collection of projects associated with this user.
    /// Navigation property for Entity Framework.
    /// </summary>
    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    /// <summary>
    /// Private constructor for Entity Framework.
    /// </summary>
    private User() { }

    /// <summary>
    /// Creates a new User instance with required fields.
    /// This constructor enforces business rules and validates input.
    /// </summary>
    /// <param name="firstName">User's first name (required)</param>
    /// <param name="lastName">User's last name (required)</param>
    /// <param name="email">User's email address (required)</param>
    /// <param name="phoneNumber">User's phone number (optional)</param>
    /// <param name="bio">User's biography (optional)</param>
    /// <exception cref="ArgumentException">Thrown when required fields are empty or invalid</exception>
    public User(string firstName, string lastName, Email email, string? phoneNumber = null, string bio = "")
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Bio = bio ?? string.Empty;
        
        var now = DateTime.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Updates the user's personal information.
    /// This method maintains business rules and updates the audit timestamp.
    /// </summary>
    /// <param name="firstName">New first name</param>
    /// <param name="lastName">New last name</param>
    /// <param name="email">New email address</param>
    /// <param name="phoneNumber">New phone number</param>
    /// <param name="bio">New biography</param>
    public void UpdatePersonalInfo(string firstName, string lastName, Email email, string? phoneNumber, string bio)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        Bio = bio ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's profile picture URL.
    /// </summary>
    /// <param name="profilePictureUrl">New profile picture URL</param>
    public void UpdateProfilePicture(string? profilePictureUrl)
    {
        ProfilePictureUrl = string.IsNullOrWhiteSpace(profilePictureUrl) ? null : profilePictureUrl.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the user's resume URL.
    /// </summary>
    /// <param name="resumeUrl">New resume URL</param>
    public void UpdateResume(string? resumeUrl)
    {
        ResumeUrl = string.IsNullOrWhiteSpace(resumeUrl) ? null : resumeUrl.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the user's full name by combining first and last names.
    /// </summary>
    /// <returns>Full name as a single string</returns>
    public string GetFullName() => $"{FirstName} {LastName}";

    /// <summary>
    /// Adds a project to the user's portfolio.
    /// This method maintains the aggregate relationship.
    /// </summary>
    /// <param name="project">Project to add</param>
    public void AddProject(Project project)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        Projects.Add(project);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes a project from the user's portfolio.
    /// </summary>
    /// <param name="project">Project to remove</param>
    public void RemoveProject(Project project)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        Projects.Remove(project);
        UpdatedAt = DateTime.UtcNow;
    }
}

