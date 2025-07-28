using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Image entity.
/// This class defines the database mapping, relationships, and constraints
/// for the Image entity used in project portfolios.
/// </summary>
public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    /// <summary>
    /// Configures the Image entity mapping to the database.
    /// </summary>
    /// <param name="builder">The entity type builder for Image</param>
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        // Table configuration
        builder.ToTable("Images");

        // Primary key configuration
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the image");

        // Foreign key configuration
        builder.Property(i => i.ProjectId)
            .IsRequired()
            .HasComment("Foreign key reference to the project this image belongs to");

        // Required string fields with length constraints
        builder.Property(i => i.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Display title for the image");

        builder.Property(i => i.Description)
            .HasMaxLength(1000)
            .HasDefaultValue(string.Empty)
            .HasComment("Detailed description of the image content");

        builder.Property(i => i.Url)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Relative URL path to the image file from wwwroot");

        builder.Property(i => i.AltText)
            .HasMaxLength(500)
            .HasDefaultValue(string.Empty)
            .HasComment("Alternative text for accessibility (screen readers)");

        // Numeric fields for image dimensions and size
        builder.Property(i => i.Width)
            .IsRequired()
            .HasComment("Image width in pixels");

        builder.Property(i => i.Height)
            .IsRequired()
            .HasComment("Image height in pixels");

        builder.Property(i => i.SizeInBytes)
            .IsRequired()
            .HasComment("File size in bytes");

        // Audit field
        builder.Property(i => i.UploadedAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the image was uploaded");

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
    /// <param name="builder">The entity type builder for Image</param>
    private void ConfigureIndexes(EntityTypeBuilder<Image> builder)
    {
        // Index on ProjectId for efficient project-specific queries
        builder.HasIndex(i => i.ProjectId)
            .HasDatabaseName("IX_Images_ProjectId");

        // Index on UploadedAt for chronological sorting
        builder.HasIndex(i => i.UploadedAt)
            .HasDatabaseName("IX_Images_UploadedAt");

        // Index on Title for search functionality
        builder.HasIndex(i => i.Title)
            .HasDatabaseName("IX_Images_Title");

        // Index on Url for uniqueness checks and direct access
        builder.HasIndex(i => i.Url)
            .IsUnique()
            .HasDatabaseName("IX_Images_Url_Unique");

        // Composite index for project images ordered by upload date
        builder.HasIndex(i => new { i.ProjectId, i.UploadedAt })
            .HasDatabaseName("IX_Images_ProjectId_UploadedAt");

        // Index on dimensions for filtering by size
        builder.HasIndex(i => new { i.Width, i.Height })
            .HasDatabaseName("IX_Images_Dimensions");

        // Index on file size for storage management queries
        builder.HasIndex(i => i.SizeInBytes)
            .HasDatabaseName("IX_Images_SizeInBytes");
    }

    /// <summary>
    /// Configures relationships between Image and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Image</param>
    private void ConfigureRelationships(EntityTypeBuilder<Image> builder)
    {
        // Many-to-one relationship with Project
        builder.HasOne(i => i.Project)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Images_Projects_ProjectId");

        // Configure navigation property loading behavior
        builder.Navigation(i => i.Project)
            .EnableLazyLoading(false);
    }

    /// <summary>
    /// Configures database constraints and business rules.
    /// </summary>
    /// <param name="builder">The entity type builder for Image</param>
    private void ConfigureConstraints(EntityTypeBuilder<Image> builder)
    {
        // Check constraints for data validation
        
        // Ensure title is not empty
        builder.HasCheckConstraint("CK_Images_Title_NotEmpty", 
            "LENGTH(TRIM(Title)) > 0");

        // Ensure URL is not empty
        builder.HasCheckConstraint("CK_Images_Url_NotEmpty", 
            "LENGTH(TRIM(Url)) > 0");

        // Ensure ProjectId is positive
        builder.HasCheckConstraint("CK_Images_ProjectId_Positive", 
            "ProjectId > 0");

        // Ensure width is positive
        builder.HasCheckConstraint("CK_Images_Width_Positive", 
            "Width > 0");

        // Ensure height is positive
        builder.HasCheckConstraint("CK_Images_Height_Positive", 
            "Height > 0");

        // Ensure file size is positive
        builder.HasCheckConstraint("CK_Images_SizeInBytes_Positive", 
            "SizeInBytes > 0");

        // Ensure reasonable image dimensions (not too large)
        builder.HasCheckConstraint("CK_Images_Width_Reasonable", 
            "Width <= 10000"); // Max 10,000 pixels wide

        builder.HasCheckConstraint("CK_Images_Height_Reasonable", 
            "Height <= 10000"); // Max 10,000 pixels tall

        // Ensure reasonable file size (not too large)
        builder.HasCheckConstraint("CK_Images_SizeInBytes_Reasonable", 
            "SizeInBytes <= 52428800"); // Max 50MB

        // Ensure UploadedAt is not in the future
        builder.HasCheckConstraint("CK_Images_UploadedAt_Valid", 
            "UploadedAt <= datetime('now')");

        // Ensure URL has proper format for relative paths
        builder.HasCheckConstraint("CK_Images_Url_Format", 
            "Url LIKE '/%' OR Url LIKE 'http%://%'");
    }

    /// <summary>
    /// Configures property access modes for proper encapsulation.
    /// </summary>
    /// <param name="builder">The entity type builder for Image</param>
    private void ConfigurePropertyAccess(EntityTypeBuilder<Image> builder)
    {
        // Use property access for fields that may have business logic
        builder.Property(i => i.Title)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.Description)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.Url)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.AltText)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.Width)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.Height)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(i => i.SizeInBytes)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        // Use field access for audit fields to prevent direct modification
        builder.Property(i => i.UploadedAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Foreign key should use property access
        builder.Property(i => i.ProjectId)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}

