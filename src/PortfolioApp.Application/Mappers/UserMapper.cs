using PortfolioApp.Application.DTOs;
using PortfolioApp.Domain.Entities;
using PortfolioApp.Domain.ValueObjects;

namespace PortfolioApp.Application.Mappers;

/// <summary>
/// Provides manual mapping functionalities between User entities and User DTOs.
/// This mapper ensures a clear separation of concerns and avoids direct exposure
/// of domain entities to the presentation layer, adhering to Clean Architecture principles.
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Maps a User entity to a UserDto.
    /// This method transforms the domain model into a presentation-friendly DTO.
    /// </summary>
    /// <param name="user">The User entity to map.</param>
    /// <returns>A new UserDto instance.</returns>
    public static UserDto ToDto(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            Email = user.Email.Value,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ResumeUrl = user.ResumeUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            ProjectCount = user.Projects?.Count ?? 0, // Assuming Projects collection might be null if not loaded
            FeaturedProjectCount = user.Projects?.Count(p => p.IsFeatured) ?? 0
        };
    }

    /// <summary>
    /// Maps a CreateUserDto to a User entity.
    /// This method transforms a DTO from the presentation layer into a domain entity.
    /// </summary>
    /// <param name="dto">The CreateUserDto to map.</param>
    /// <returns>A new User entity instance.</returns>
    public static User ToEntity(CreateUserDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        // Email is a ValueObject, so it needs to be created from the string.
        var email = new Email(dto.Email);

        return new User(
            dto.FirstName,
            dto.LastName,
            email,
            dto.PhoneNumber,
            dto.Bio
        );
    }

    /// <summary>
    /// Updates an existing User entity from an UpdateUserDto.
    /// This method modifies an existing domain entity based on the DTO.
    /// </summary>
    /// <param name="dto">The UpdateUserDto containing updated information.</param>
    /// <param name="user">The existing User entity to update.</param>
    public static void ToEntity(UpdateUserDto dto, User user)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        // Email is a ValueObject, so it needs to be created from the string.
        var email = new Email(dto.Email);

        user.UpdatePersonalInfo(
            dto.FirstName,
            dto.LastName,
            email,
            dto.PhoneNumber,
            dto.Bio
        );
    }

    /// <summary>
    /// Maps a User entity to a UserSummaryDto.
    /// This method provides a lightweight DTO for summary views.
    /// </summary>
    /// <param name="user">The User entity to map.</param>
    /// <returns>A new UserSummaryDto instance.</returns>
    public static UserSummaryDto ToSummaryDto(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return new UserSummaryDto
        {
            Id = user.Id,
            FullName = user.GetFullName(),
            Email = user.Email.Value,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ProjectCount = user.Projects?.Count ?? 0
        };
    }

    /// <summary>
    /// Maps a User entity to a UserProfileDto.
    /// This method transforms the domain model into a public profile-friendly DTO,
    /// including featured and recent projects, and technologies.
    /// </summary>
    /// <param name="user">The User entity to map.</param>
    /// <param name="featuredProjects">Collection of featured projects to include.</param>
    /// <param name="recentProjects">Collection of recent projects to include.</param>
    /// <param name="technologies">Collection of unique technologies to include.</param>
    /// <returns>A new UserProfileDto instance.</returns>
    public static UserProfileDto ToProfileDto(User user, IEnumerable<Project> featuredProjects, IEnumerable<Project> recentProjects, IEnumerable<string> technologies)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        if (featuredProjects == null)
        {
            throw new ArgumentNullException(nameof(featuredProjects));
        }
        if (recentProjects == null)
        {
            throw new ArgumentNullException(nameof(recentProjects));
        }
        if (technologies == null)
        {
            throw new ArgumentNullException(nameof(technologies));
        }

        return new UserProfileDto
        {
            FullName = user.GetFullName(),
            Email = user.Email.Value,
            PhoneNumber = user.PhoneNumber,
            Bio = user.Bio,
            ProfilePictureUrl = user.ProfilePictureUrl,
            ResumeUrl = user.ResumeUrl,
            FeaturedProjects = featuredProjects.Select(ProjectMapper.ToSummaryDto).ToList(),
            RecentProjects = recentProjects.Select(ProjectMapper.ToSummaryDto).ToList(),
            Technologies = technologies.ToList()
        };
    }
}

