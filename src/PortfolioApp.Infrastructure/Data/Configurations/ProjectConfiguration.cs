using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Project entity.
/// This class defines the database mapping, relationships, and constraints
/// for the Project entity following Clean Architecture principles.
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configures the Project entity mapping to the database.
    /// </summary>
    /// <param name="builder">The entity type builder for Project</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        // Table configuration
        builder.ToTable("Projects");

        // Primary key configuration
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the project");

        // Foreign key configuration
        builder.Property(p => p.UserId)
            .IsRequired()
            .HasComment("Foreign key reference to the user who owns this project");

        // Required string fields with length constraints
        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Project title");

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(5000)
            .HasComment("Detailed project description");

        // Optional string fields
        builder.Property(p => p.TechnologiesUsed)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty)
            .HasComment("Comma-separated list of technologies used");

        builder.Property(p => p.ProjectUrl)
            .HasMaxLength(500)
            .HasComment("URL to the live project or repository");

        // Date fields
        builder.Property(p => p.StartDate)
            .HasComment("Project start date");

        builder.Property(p => p.EndDate)
            .HasComment("Project end date (null for ongoing projects)");

        // Boolean fields
        builder.Property(p => p.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates if project should be featured prominently");

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the project record was created");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the project record was last updated");

        // Indexes for performance optimization
        ConfigureIndexes(builder);

        // Relationships configuration
        ConfigureRelationships(builder);

        // Additional constraints
        ConfigureConstraints(builder);

        // Configure property access
        ConfigurePropertyAccess(builder);
    }

    /// <summary>
    /// Configures database indexes for query performance optimization.
    /// </summary>
    /// <param name="builder">The entity type builder for Project</param>
    private void ConfigureIndexes(EntityTypeBuilder<Project> builder)
    {
        // Index on UserId for efficient user-specific queries
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Projects_UserId");

        // Index on IsFeatured for featured project queries
        builder.HasIndex(p => p.IsFeatured)
            .HasDatabaseName("IX_Projects_IsFeatured");

        // Composite index for user's featured projects
        builder.HasIndex(p => new { p.UserId, p.IsFeatured })
            .HasDatabaseName("IX_Projects_UserId_IsFeatured");

        // Index on CreatedAt for chronological sorting
        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Projects_CreatedAt");

        // Index on StartDate for timeline queries
        builder.HasIndex(p => p.StartDate)
            .HasDatabaseName("IX_Projects_StartDate");

        // Index on EndDate for filtering completed/ongoing projects
        builder.HasIndex(p => p.EndDate)
            .HasDatabaseName("IX_Projects_EndDate");

        // Composite index for date range queries
        builder.HasIndex(p => new { p.StartDate, p.EndDate })
            .HasDatabaseName("IX_Projects_DateRange");

        // Index on Title for search functionality
        builder.HasIndex(p => p.Title)
            .HasDatabaseName("IX_Projects_Title");

        // Unique constraint on Title per User
        builder.HasIndex(p => new { p.UserId, p.Title })
            .IsUnique()
            .HasDatabaseName("IX_Projects_UserId_Title_Unique");
    }

    /// <summary>
    /// Configures relationships between Project and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Project</param>
    private void ConfigureRelationships(EntityTypeBuilder<Project> builder)
    {
        // Many-to-one relationship with User
        builder.HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Projects_Users_UserId");

        // One-to-many relationship with Images
        builder.HasMany(p => p.Images)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Images_Projects_ProjectId");

        // One-to-many relationship with Videos
        builder.HasMany(p => p.Videos)
            .WithOne(v => v.Project)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Videos_Projects_ProjectId");

        // Configure navigation property loading behavior
        builder.Navigation(p => p.User)
            .EnableLazyLoading(false);

        builder.Navigation(p => p.Images)
            .EnableLazyLoading(false)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Navigation(p => p.Videos)
            .EnableLazyLoading(false)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }

    /// <summary>
    /// Configures database constraints and business rules.
    /// </summary>
    /// <param name="builder">The entity type builder for Project</param>
    private void ConfigureConstraints(EntityTypeBuilder<Project> builder)
    {
        // Check constraints for data validation
        
        // Ensure title is not empty
        builder.HasCheckConstraint("CK_Projects_Title_NotEmpty", 
            "LENGTH(TRIM(Title)) > 0");

        // Ensure description is not empty
        builder.HasCheckConstraint("CK_Projects_Description_NotEmpty", 
            "LENGTH(TRIM(Description)) > 0");

        // Ensure UserId is positive
        builder.HasCheckConstraint("CK_Projects_UserId_Positive", 
            "UserId > 0");

        // Ensure EndDate is not before StartDate
        builder.HasCheckConstraint("CK_Projects_DateRange_Valid", 
            "EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate");

        // Ensure CreatedAt is not in the future
        builder.HasCheckConstraint("CK_Projects_CreatedAt_Valid", 
            "CreatedAt <= datetime('now')");

        // Ensure UpdatedAt is not before CreatedAt
        builder.HasCheckConstraint("CK_Projects_UpdatedAt_Valid", 
            "UpdatedAt >= CreatedAt");

        // Ensure ProjectUrl is a valid URL format if provided
        builder.HasCheckConstraint("CK_Projects_ProjectUrl_Format", 
            "ProjectUrl IS NULL OR ProjectUrl LIKE 'http%://%'");
    }

    /// <summary>
    /// Configures property access modes for proper encapsulation.
    /// </summary>
    /// <param name="builder">The entity type builder for Project</param>
    private void ConfigurePropertyAccess(EntityTypeBuilder<Project> builder)
    {
        // Use property access for fields that may have business logic
        builder.Property(p => p.Title)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.Description)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.TechnologiesUsed)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.ProjectUrl)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.StartDate)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.EndDate)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(p => p.IsFeatured)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        // Use field access for audit fields to prevent direct modification
        builder.Property(p => p.CreatedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(p => p.UpdatedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Foreign key should use property access
        builder.Property(p => p.UserId)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}

