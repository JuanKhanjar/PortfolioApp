using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the ContactMessage entity.
/// This class defines the database mapping, relationships, and constraints
/// for the ContactMessage entity used for visitor inquiries.
/// </summary>
public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    /// <summary>
    /// Configures the ContactMessage entity mapping to the database.
    /// </summary>
    /// <param name="builder">The entity type builder for ContactMessage</param>
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        // Table configuration
        builder.ToTable("ContactMessages");

        // Primary key configuration
        builder.HasKey(cm => cm.Id);
        builder.Property(cm => cm.Id)
            .ValueGeneratedOnAdd()
            .HasComment("Unique identifier for the contact message");

        // Required string fields with length constraints
        builder.Property(cm => cm.SenderName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Name of the person sending the message");

        // Email configuration - handled as value object conversion in DbContext
        builder.Property(cm => cm.SenderEmail)
            .IsRequired()
            .HasMaxLength(254) // RFC 5321 maximum email length
            .HasComment("Email address of the sender");

        builder.Property(cm => cm.Subject)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Subject line of the message");

        builder.Property(cm => cm.Message)
            .IsRequired()
            .HasMaxLength(5000)
            .HasComment("The actual message content from the sender");

        // Date and boolean fields
        builder.Property(cm => cm.SentAt)
            .IsRequired()
            .HasDefaultValueSql("datetime('now')")
            .HasComment("Timestamp when the message was sent/received");

        builder.Property(cm => cm.IsRead)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Indicates whether the message has been read by an administrator");

        // Indexes for performance optimization
        ConfigureIndexes(builder);

        // Additional constraints
        ConfigureConstraints(builder);

        // Configure property access
        ConfigurePropertyAccess(builder);
    }

    /// <summary>
    /// Configures database indexes for query performance optimization.
    /// </summary>
    /// <param name="builder">The entity type builder for ContactMessage</param>
    private void ConfigureIndexes(EntityTypeBuilder<ContactMessage> builder)
    {
        // Index on SentAt for chronological sorting (most common query)
        builder.HasIndex(cm => cm.SentAt)
            .HasDatabaseName("IX_ContactMessages_SentAt");

        // Index on IsRead for filtering read/unread messages
        builder.HasIndex(cm => cm.IsRead)
            .HasDatabaseName("IX_ContactMessages_IsRead");

        // Composite index for unread messages ordered by date
        builder.HasIndex(cm => new { cm.IsRead, cm.SentAt })
            .HasDatabaseName("IX_ContactMessages_IsRead_SentAt");

        // Index on SenderEmail for tracking communication history
        builder.HasIndex(cm => cm.SenderEmail)
            .HasDatabaseName("IX_ContactMessages_SenderEmail");

        // Index on SenderName for search functionality
        builder.HasIndex(cm => cm.SenderName)
            .HasDatabaseName("IX_ContactMessages_SenderName");

        // Index on Subject for search and categorization
        builder.HasIndex(cm => cm.Subject)
            .HasDatabaseName("IX_ContactMessages_Subject");

        // Composite index for sender-specific message history
        builder.HasIndex(cm => new { cm.SenderEmail, cm.SentAt })
            .HasDatabaseName("IX_ContactMessages_SenderEmail_SentAt");

        // Index for urgent message queries (unread + old)
        builder.HasIndex(cm => new { cm.IsRead, cm.SentAt })
            .HasFilter("IsRead = 0") // Only index unread messages for urgent queries
            .HasDatabaseName("IX_ContactMessages_Urgent");
    }

    /// <summary>
    /// Configures database constraints and business rules.
    /// </summary>
    /// <param name="builder">The entity type builder for ContactMessage</param>
    private void ConfigureConstraints(EntityTypeBuilder<ContactMessage> builder)
    {
        // Check constraints for data validation
        
        // Ensure sender name is not empty
        builder.HasCheckConstraint("CK_ContactMessages_SenderName_NotEmpty", 
            "LENGTH(TRIM(SenderName)) > 0");

        // Ensure sender name is reasonable length
        builder.HasCheckConstraint("CK_ContactMessages_SenderName_Length", 
            "LENGTH(TRIM(SenderName)) >= 2");

        // Ensure subject is not empty
        builder.HasCheckConstraint("CK_ContactMessages_Subject_NotEmpty", 
            "LENGTH(TRIM(Subject)) > 0");

        // Ensure subject is reasonable length
        builder.HasCheckConstraint("CK_ContactMessages_Subject_Length", 
            "LENGTH(TRIM(Subject)) >= 3");

        // Ensure message is not empty
        builder.HasCheckConstraint("CK_ContactMessages_Message_NotEmpty", 
            "LENGTH(TRIM(Message)) > 0");

        // Ensure message is reasonable length
        builder.HasCheckConstraint("CK_ContactMessages_Message_Length", 
            "LENGTH(TRIM(Message)) >= 10");

        // Ensure SentAt is not in the future
        builder.HasCheckConstraint("CK_ContactMessages_SentAt_Valid", 
            "SentAt <= datetime('now')");

        // Ensure email format is valid (basic check)
        builder.HasCheckConstraint("CK_ContactMessages_SenderEmail_Format", 
            "SenderEmail LIKE '%@%.%' AND LENGTH(SenderEmail) > 5");

        // Prevent obviously fake or test emails
        builder.HasCheckConstraint("CK_ContactMessages_SenderEmail_NotTest", 
            "SenderEmail NOT LIKE '%test@test%' AND SenderEmail NOT LIKE '%example.com%'");
    }

    /// <summary>
    /// Configures property access modes for proper encapsulation.
    /// </summary>
    /// <param name="builder">The entity type builder for ContactMessage</param>
    private void ConfigurePropertyAccess(EntityTypeBuilder<ContactMessage> builder)
    {
        // Use property access for fields that may have business logic
        builder.Property(cm => cm.SenderName)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(cm => cm.SenderEmail)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(cm => cm.Subject)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(cm => cm.Message)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(cm => cm.IsRead)
            .UsePropertyAccessMode(PropertyAccessMode.Property);

        // Use field access for audit fields to prevent direct modification
        builder.Property(cm => cm.SentAt)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

