using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the User entity.
/// This class implements IEntityTypeConfiguration to separate entity configuration
/// from the DbContext, promoting better organization and maintainability.
/// 
/// The configuration follows Clean Architecture principles by:
/// - Keeping infrastructure concerns separate from domain logic
/// - Using explicit configuration rather than conventions
/// - Providing clear mapping between domain entities and database schema
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the User entity mapping to the database.
    /// This method defines table structure, relationships, constraints, and indexes.
    /// </summary>
    /// <param name="builder">The entity type builder for User</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("Users");

        // Primary key configuration
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the user");

        // Required string fields with length constraints
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("User's first name");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("User's last name");

        // Email configuration - handled as value object conversion in DbContext
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(254) // RFC 5321 maximum email length
            .HasComment("User's email address");

        // Optional string fields
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .HasComment("User's phone number");

        builder.Property(u => u.Bio)
            .HasMaxLength(2000)
            .HasDefaultValue(string.Empty)
            .HasComment("User's professional biography");

        // URL fields for media references
        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(500)
            .HasComment("Relative URL to user's profile picture");

        builder.Property(u => u.ResumeUrl)
            .HasMaxLength(500)
            .HasComment("Relative URL to user's resume file");

        // Audit fields
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the user record was created");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the user record was last updated");

        // Indexes for performance optimization
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        // Relationships configuration
        ConfigureRelationships(builder);

        // Additional constraints and validations
        ConfigureConstraints(builder);

        // Configure property access and change tracking
        ConfigurePropertyAccess(builder);
    }

    /// <summary>
    /// Configures relationships between User and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for User</param>
    private void ConfigureRelationships(EntityTypeBuilder<User> builder)
    {
        // One-to-many relationship with Projects
        builder.HasMany(u => u.Projects)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade) // Delete projects when user is deleted
            .HasConstraintName("FK_Projects_Users_UserId");

        // Configure navigation property loading behavior
        builder.Navigation(u => u.Projects)
            .EnableLazyLoading(false) // Disable lazy loading for better performance control
            /*.HasField("_projects")*/ // Use backing field if available
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    /// <summary>
    /// Configures additional constraints and business rules at the database level.
    /// </summary>
    /// <param name="builder">The entity type builder for User</param>
    private void ConfigureConstraints(EntityTypeBuilder<User> builder)
    {
        // Check constraints for data validation at database level
        // Note: SQLite has limited support for check constraints, but we can add them for documentation
        
        // Ensure first name is not empty
        builder.HasCheckConstraint("CK_Users_FirstName_NotEmpty", 
            "LENGTH(TRIM(FirstName)) > 0");

        // Ensure last name is not empty
        builder.HasCheckConstraint("CK_Users_LastName_NotEmpty", 
            "LENGTH(TRIM(LastName)) > 0");

        // Ensure email format is valid (basic check)
        builder.HasCheckConstraint("CK_Users_Email_Format", 
            "Email LIKE '%@%.%' AND LENGTH(Email) > 5");

        // Ensure phone number format if provided
        builder.HasCheckConstraint("CK_Users_PhoneNumber_Format", 
            "PhoneNumber IS NULL OR LENGTH(TRIM(PhoneNumber)) >= 10");

        // Ensure CreatedAt is not in the future
        builder.HasCheckConstraint("CK_Users_CreatedAt_Valid", 
            "CreatedAt <= datetime('now')");

        // Ensure UpdatedAt is not before CreatedAt
        builder.HasCheckConstraint("CK_Users_UpdatedAt_Valid", 
            "UpdatedAt >= CreatedAt");
    }

    /// <summary>
    /// Configures property access modes and change tracking behavior.
    /// </summary>
    /// <param name="builder">The entity type builder for User</param>
    private void ConfigurePropertyAccess(EntityTypeBuilder<User> builder)
    {
        // Configure property access modes for encapsulation
        // This ensures that Entity Framework uses the proper property setters
        // which may contain business logic

        builder.Property(u => u.FirstName)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.LastName)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.Email)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.PhoneNumber)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.Bio)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.ProfilePictureUrl)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(u => u.ResumeUrl)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        // Audit fields should use field access to prevent modification
        builder.Property(u => u.CreatedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(u => u.UpdatedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

