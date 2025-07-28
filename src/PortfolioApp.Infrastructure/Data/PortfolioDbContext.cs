using Microsoft.EntityFrameworkCore;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.ValueObjects;
using PortfolioApp.Infrastructure.Data.Configurations;

namespace PortfolioApp.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the Portfolio application.
/// This class serves as the main data access context, providing access to all entities
/// and configuring the database schema through Entity Framework Core.
/// 
/// The DbContext follows Clean Architecture principles by:
/// - Being located in the Infrastructure layer
/// - Implementing interfaces defined in the Domain layer
/// - Using explicit configuration rather than conventions
/// - Supporting dependency injection and testability
/// </summary>
public class PortfolioDbContext : DbContext
{
    /// <summary>
    /// DbSet for User entities. Represents the Users table in the database.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// DbSet for Project entities. Represents the Projects table in the database.
    /// </summary>
    public DbSet<Project> Projects { get; set; } = null!;

    /// <summary>
    /// DbSet for Image entities. Represents the Images table in the database.
    /// </summary>
    public DbSet<Image> Images { get; set; } = null!;

    /// <summary>
    /// DbSet for Video entities. Represents the Videos table in the database.
    /// </summary>
    public DbSet<Video> Videos { get; set; } = null!;

    /// <summary>
    /// DbSet for ContactMessage entities. Represents the ContactMessages table in the database.
    /// </summary>
    public DbSet<ContactMessage> ContactMessages { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the PortfolioDbContext class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext</param>
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the database schema using Entity Framework's Fluent API.
    /// This method is called by Entity Framework to build the model and configure
    /// entity relationships, constraints, and database-specific settings.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from separate configuration classes
        // This approach keeps the DbContext clean and organizes configuration logic
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ImageConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
        modelBuilder.ApplyConfiguration(new ContactMessageConfiguration());

        // Configure value object conversions
        ConfigureValueObjects(modelBuilder);

        // Apply global query filters if needed
        ConfigureGlobalFilters(modelBuilder);

        // Configure database-specific settings
        ConfigureDatabaseSpecific(modelBuilder);
    }

    /// <summary>
    /// Configures value object conversions for Entity Framework.
    /// This method handles the mapping between domain value objects and database primitives.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Configure Email value object conversion
        // This allows Email objects to be stored as strings in the database
        // while maintaining the rich domain model in the application
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .HasConversion(
                email => email.Value,           // Convert Email to string for database
                value => new Email(value),      // Convert string to Email from database
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Email>(
                    (e1, e2) => e1!.Equals(e2), // Equality comparison
                    e => e.GetHashCode(),       // Hash code generation
                    e => new Email(e.Value)     // Deep copy for change tracking
                )
            );

        modelBuilder.Entity<ContactMessage>()
            .Property(cm => cm.SenderEmail)
            .HasConversion(
                email => email.Value,
                value => new Email(value),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Email>(
                    (e1, e2) => e1!.Equals(e2),
                    e => e.GetHashCode(),
                    e => new Email(e.Value)
                )
            );
    }

    /// <summary>
    /// Configures global query filters for soft delete and other cross-cutting concerns.
    /// This method can be used to implement patterns like soft delete across all entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        // Example: If implementing soft delete, you would add global filters here
        // modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
        
        // For now, no global filters are needed, but this method provides
        // a centralized place to add them in the future
    }

    /// <summary>
    /// Configures database-specific settings and optimizations.
    /// This method handles SQLite-specific configurations and performance optimizations.
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    private void ConfigureDatabaseSpecific(ModelBuilder modelBuilder)
    {
        // SQLite-specific configurations
        // SQLite doesn't support some features, so we need to work around them

        // Configure decimal precision for SQLite
        // SQLite stores decimals as TEXT, so we need to specify precision
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("TEXT");
        }

        // Configure DateTime handling for SQLite
        // Ensure DateTime values are stored in UTC and handled consistently
        foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
        {
            property.SetColumnType("TEXT");
        }
    }

    /// <summary>
    /// Configures the database connection and other options.
    /// This method is called when the DbContext is being configured.
    /// </summary>
    /// <param name="optionsBuilder">The options builder instance</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Additional configuration can be added here if needed
        // However, it's generally better to configure options through dependency injection
        
        // Enable sensitive data logging in development (should be disabled in production)
        #if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        #endif

        // Configure SQLite-specific options
        if (optionsBuilder.IsConfigured && optionsBuilder.Options.Extensions
            .Any(e => e.GetType().Name.Contains("SqliteOptionsExtension")))
        {
            // SQLite-specific optimizations
            optionsBuilder.UseSqlite(options =>
            {
                // Configure command timeout
                options.CommandTimeout(30);
            });
        }
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// This override adds audit functionality and other cross-cutting concerns.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The number of state entries written to the database</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Add audit information before saving
        AddAuditInformation();

        // Validate entities before saving
        ValidateEntities();

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Log the exception and provide more meaningful error messages
            // In a real application, you would use a proper logging framework
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }

    /// <summary>
    /// Synchronous version of SaveChangesAsync.
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    public override int SaveChanges()
    {
        AddAuditInformation();
        ValidateEntities();

        try
        {
            return base.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }

    /// <summary>
    /// Adds audit information to entities before saving.
    /// This method automatically updates CreatedAt and UpdatedAt fields.
    /// </summary>
    private void AddAuditInformation()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            // Handle CreatedAt for new entities
            if (entry.State == EntityState.Added)
            {
                if (entry.Property("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = utcNow;
                }
            }

            // Handle UpdatedAt for modified entities
            if (entry.State == EntityState.Modified)
            {
                if (entry.Property("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = utcNow;
                }

                // Prevent CreatedAt from being modified
                if (entry.Property("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").IsModified = false;
                }
            }
        }
    }

    /// <summary>
    /// Validates entities before saving to ensure business rules are met.
    /// This method provides an additional layer of validation at the data access level.
    /// </summary>
    private void ValidateEntities()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entries)
        {
            // Validate based on entity type
            switch (entity)
            {
                case User user:
                    ValidateUser(user);
                    break;
                case Project project:
                    ValidateProject(project);
                    break;
                case ContactMessage contactMessage:
                    ValidateContactMessage(contactMessage);
                    break;
                case Image image:
                    ValidateImage(image);
                    break;
                case Video video:
                    ValidateVideo(video);
                    break;
            }
        }
    }

    /// <summary>
    /// Validates User entity business rules.
    /// </summary>
    /// <param name="user">User entity to validate</param>
    private void ValidateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName))
            throw new InvalidOperationException("User first name is required.");

        if (string.IsNullOrWhiteSpace(user.LastName))
            throw new InvalidOperationException("User last name is required.");

        if (user.Email == null)
            throw new InvalidOperationException("User email is required.");
    }

    /// <summary>
    /// Validates Project entity business rules.
    /// </summary>
    /// <param name="project">Project entity to validate</param>
    private void ValidateProject(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Title))
            throw new InvalidOperationException("Project title is required.");

        if (string.IsNullOrWhiteSpace(project.Description))
            throw new InvalidOperationException("Project description is required.");

        if (project.UserId <= 0)
            throw new InvalidOperationException("Project must be associated with a valid user.");
    }

    /// <summary>
    /// Validates ContactMessage entity business rules.
    /// </summary>
    /// <param name="contactMessage">ContactMessage entity to validate</param>
    private void ValidateContactMessage(ContactMessage contactMessage)
    {
        if (string.IsNullOrWhiteSpace(contactMessage.SenderName))
            throw new InvalidOperationException("Contact message sender name is required.");

        if (contactMessage.SenderEmail == null)
            throw new InvalidOperationException("Contact message sender email is required.");

        if (string.IsNullOrWhiteSpace(contactMessage.Subject))
            throw new InvalidOperationException("Contact message subject is required.");

        if (string.IsNullOrWhiteSpace(contactMessage.Message))
            throw new InvalidOperationException("Contact message content is required.");
    }

    /// <summary>
    /// Validates Image entity business rules.
    /// </summary>
    /// <param name="image">Image entity to validate</param>
    private void ValidateImage(Image image)
    {
        if (string.IsNullOrWhiteSpace(image.Title))
            throw new InvalidOperationException("Image title is required.");

        if (string.IsNullOrWhiteSpace(image.Url))
            throw new InvalidOperationException("Image URL is required.");

        if (image.ProjectId <= 0)
            throw new InvalidOperationException("Image must be associated with a valid project.");
    }

    /// <summary>
    /// Validates Video entity business rules.
    /// </summary>
    /// <param name="video">Video entity to validate</param>
    private void ValidateVideo(Video video)
    {
        if (string.IsNullOrWhiteSpace(video.Title))
            throw new InvalidOperationException("Video title is required.");

        if (string.IsNullOrWhiteSpace(video.Url))
            throw new InvalidOperationException("Video URL is required.");

        if (video.ProjectId <= 0)
            throw new InvalidOperationException("Video must be associated with a valid project.");
    }
}

