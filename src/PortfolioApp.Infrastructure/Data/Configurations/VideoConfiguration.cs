using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Video entity.
/// This class defines the database mapping, relationships, and constraints
/// for the Video entity used in project portfolios.
/// </summary>
public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    /// <summary>
    /// Configures the Video entity mapping to the database.
    /// </summary>
    /// <param name="builder">The entity type builder for Video</param>
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        // Table configuration
        builder.ToTable("Videos");

        // Primary key configuration
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the video");

        // Foreign key configuration
        builder.Property(v => v.ProjectId)
            .IsRequired()
            .HasComment("Foreign key reference to the project this video belongs to");

        // Required string fields with length constraints
        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Display title for the video");

        builder.Property(v => v.Description)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty)
            .HasComment("Detailed description of the video content");

        builder.Property(v => v.Url)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Relative URL path to the video file from wwwroot");

        builder.Property(v => v.ThumbnailUrl)
            .HasMaxLength(500)
            .HasComment("Relative URL path to the video thumbnail image");

        // Numeric fields for video metadata
        builder.Property(v => v.DurationInSeconds)
            .IsRequired()
            .HasComment("Video duration in seconds");

        builder.Property(v => v.SizeInBytes)
            .IsRequired()
            .HasComment("File size in bytes");

        // Audit field
        builder.Property(v => v.UploadedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the video was uploaded");

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
    /// <param name="builder">The entity type builder for Video</param>
    private void ConfigureIndexes(EntityTypeBuilder<Video> builder)
    {
        // Index on ProjectId for efficient project-specific queries
        builder.HasIndex(v => v.ProjectId)
            .HasDatabaseName("IX_Videos_ProjectId");

        // Index on UploadedAt for chronological sorting
        builder.HasIndex(v => v.UploadedAt)
            .HasDatabaseName("IX_Videos_UploadedAt");

        // Index on Title for search functionality
        builder.HasIndex(v => v.Title)
            .HasDatabaseName("IX_Videos_Title");

        // Index on Url for uniqueness checks and direct access
        builder.HasIndex(v => v.Url)
            .IsUnique()
            .HasDatabaseName("IX_Videos_Url_Unique");

        // Composite index for project videos ordered by upload date
        builder.HasIndex(v => new { v.ProjectId, v.UploadedAt })
            .HasDatabaseName("IX_Videos_ProjectId_UploadedAt");

        // Index on duration for filtering by video length
        builder.HasIndex(v => v.DurationInSeconds)
            .HasDatabaseName("IX_Videos_DurationInSeconds");

        // Index on file size for storage management queries
        builder.HasIndex(v => v.SizeInBytes)
            .HasDatabaseName("IX_Videos_SizeInBytes");

        // Index on ThumbnailUrl for videos with thumbnails
        builder.HasIndex(v => v.ThumbnailUrl)
            .HasDatabaseName("IX_Videos_ThumbnailUrl");
    }

    /// <summary>
    /// Configures relationships between Video and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Video</param>
    private void ConfigureRelationships(EntityTypeBuilder<Video> builder)
    {
        // Many-to-one relationship with Project
        builder.HasOne(v => v.Project)
            .WithMany(p => p.Videos)
            .HasForeignKey(v => v.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Videos_Projects_ProjectId");

        // Configure navigation property loading behavior
        builder.Navigation(v => v.Project)
            .EnableLazyLoading(false);
    }

    /// <summary>
    /// Configures database constraints and business rules.
    /// </summary>
    /// <param name="builder">The entity type builder for Video</param>
    private void ConfigureConstraints(EntityTypeBuilder<Video> builder)
    {
        // Check constraints for data validation
        
        // Ensure title is not empty
        builder.HasCheckConstraint("CK_Videos_Title_NotEmpty", 
            "LENGTH(TRIM(Title)) > 0");

        // Ensure URL is not empty
        builder.HasCheckConstraint("CK_Videos_Url_NotEmpty", 
            "LENGTH(TRIM(Url)) > 0");

        // Ensure ProjectId is positive
        builder.HasCheckConstraint("CK_Videos_ProjectId_Positive", 
            "ProjectId > 0");

        // Ensure duration is not negative
        builder.HasCheckConstraint("CK_Videos_DurationInSeconds_NonNegative", 
            "DurationInSeconds >= 0");

        // Ensure file size is positive
        builder.HasCheckConstraint("CK_Videos_SizeInBytes_Positive", 
            "SizeInBytes > 0");

        // Ensure reasonable video duration (not too long)
        builder.HasCheckConstraint("CK_Videos_DurationInSeconds_Reasonable", 
            "DurationInSeconds <= 14400"); // Max 4 hours (14400 seconds)

        // Ensure reasonable file size (not too large)
        builder.HasCheckConstraint("CK_Videos_SizeInBytes_Reasonable", 
            "SizeInBytes <= 2147483648"); // Max 2GB

        // Ensure UploadedAt is not in the future
        builder.HasCheckConstraint("CK_Videos_UploadedAt_Valid", 
            "UploadedAt <= datetime('now')");

        // Ensure URL has proper format for relative paths
        builder.HasCheckConstraint("CK_Videos_Url_Format", 
            "Url LIKE '/%' OR Url LIKE 'http%://%'");

        // Ensure ThumbnailUrl has proper format if provided
        builder.HasCheckConstraint("CK_Videos_ThumbnailUrl_Format", 
            "ThumbnailUrl IS NULL OR ThumbnailUrl LIKE '/%' OR ThumbnailUrl LIKE 'http%://%'");
    }

    /// <summary>
    /// Configures property access modes for proper encapsulation.
    /// </summary>
    /// <param name="builder">The entity type builder for Video</param>
    private void ConfigurePropertyAccess(EntityTypeBuilder<Video> builder)
    {
        // Use property access for fields that may have business logic
        builder.Property(v => v.Title)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(v => v.Description)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(v => v.Url)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(v => v.ThumbnailUrl)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(v => v.DurationInSeconds)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(v => v.SizeInBytes)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        // Use field access for audit fields to prevent direct modification
        builder.Property(v => v.UploadedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Foreign key should use property access
        builder.Property(v => v.ProjectId)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}

